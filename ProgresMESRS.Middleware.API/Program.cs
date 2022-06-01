using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CefSharp;
using CefSharp.Handler;
using CefSharp.Event;
using CefSharp.Web;
using CefSharp.Fluent;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Threading.Tasks;
using System.Threading;
using LiteDB;
using LiteDB.Async;
using ProgresMESRS.Middleware.API.Models;
using HtmlAgilityPack;
using Jetsons.JetPack;
using Akavache;
using Akavache.Sqlite3;
using MoreLinq;

namespace ProgresMESRS.Middleware.API
{
    public class ProgresDownloadHandler : IDownloadHandler
    {
        public event EventHandler<DownloadItem> OnBeforeDownloadFired;

        public event EventHandler<DownloadItem> OnDownloadUpdatedFired;

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            OnBeforeDownloadFired?.Invoke(this, downloadItem);

            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    callback.Continue($@"D:\ProgresStudents\{Path.GetRandomFileName()}.jpg", showDialog: false);
                }
            }
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            OnDownloadUpdatedFired?.Invoke(this, downloadItem);

            if(downloadItem.IsComplete && downloadItem.IsValid)
            {
                try
                {
                    var bytes = File.ReadAllBytes(downloadItem.FullPath);
                    Akavache.BlobCache.LocalMachine.Insert(downloadItem.Url, bytes);
                    //File.Delete(downloadItem.FullPath);
                }
                catch
                {

                }       
            }
        }
    }
    static class Program
    {
        public static Worker browser;
        public static Mutex Mutex = new Mutex(false);
        public static async Task Main(string[] args)
        {
            BlobCache.LocalMachine = new SqlRawPersistentBlobCache(AppContext.BaseDirectory + "imagecache.db");

            Akavache.Registrations.Start("ProgresImageCache");


            var db = new LiteDatabaseAsync(@"Filename=D:\ProgresDb.db;Connection=shared");

            StudentsListScrapperXml scrapper = new StudentsListScrapperXml();
            StudentDetailScrapperXml detailsScrapper  = new StudentDetailScrapperXml();

            /*var source = File.ReadAllText(@"D:\WebProjects\University-APP\PageSamples\students\498students_.txt");

            //var rowKeys = Worker.ExtractRowKeys(source);
            var rows = Worker.ExtractRowKeys(source);


            Console.ReadKey();*/

            /*StudentsListScrapperXml scrapper = new StudentsListScrapperXml();
            var files = new DirectoryInfo(@"D:\WebProjects\University-APP\PageSamples\students")
                .GetFiles("*.txt");
            var pages = files.ToList().Select(file =>
            {
                return File.ReadAllText(file.FullName);
            });

            //var htmlSource = File.ReadAllText(@"D:\WebProjects\University-APP\PageSamples\students");
            var data = pages.SelectMany(page => scrapper.ExtractData(page)).ToList().Where(x => x != null).OrderBy(x => x.Index).Distinct().ToList();
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(@"D:\WebProjects\University-APP\PageSamples\students_filtered.json", json);
            db.GetCollection<UniversityStudent>().InsertBulk(data);
            Console.ReadKey();*/


            browser = new Worker("https://progres.mesrs.dz/webfve");
            browser.Page.DownloadHandler = new ProgresDownloadHandler();

            ApiClient apiClient = new ApiClient();

            //browser.StartMonitoring();
            await browser.OpenUrl("https://progres.mesrs.dz/webfve/login.xhtml");
            await browser.SignIn("[Username]", "[Password]");

            await browser.OpenUrl("https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml");
            var cookies = await browser.GetCookies("https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml");
            await browser.PressNextAndGetData(true);

            var javaViewState = browser.ExtractJavaViewStates();
            int pageSize = 100;
            int itemsCount = await browser.GetItemsCount();
            int pageCount = itemsCount / pageSize;
            var positions = Enumerable.Range(1, pageCount).Select(x => x * 100);

            var students = new ConcurrentBag<IEnumerable<UniversityStudent>>();
            var rowKeys = new ConcurrentBag<IEnumerable<KeyValuePair<int, string>>>();

            var keys = await db.GetCollection<RowKeyModel>().FindAllAsync();

            Parallel.ForEach(Enumerable.Range(0, pageCount).Reverse().Shuffle(), new ParallelOptions() { MaxDegreeOfParallelism = 4 }, async page =>
            {
                try
                {
                    var pageSource = await apiClient.GetStudentList(javaViewState.First(), 100, 100 * page, cookies);
                    var students =  scrapper.ExtractData(pageSource);
                    var keys = Worker.ExtractRowKeys(pageSource);
                    var viewSate = Worker.ExtractJavaViewStatesFromXml(pageSource);

                    Parallel.ForEach(keys,new ParallelOptions() { MaxDegreeOfParallelism = 16 }, async key =>
                    {
                        try
                        {
                            Console.WriteLine(JsonConvert.SerializeObject(key, Formatting.Indented));

                            var pageResponse = await apiClient.GetStudentDetails(javaViewState.First(), key.Value, cookies);
                            var details = detailsScrapper.ExtractSingleData(pageResponse, "");

                            if(details != null)
                            {
                                details.Index = students.ToList().Find(x => x.Matricule.ToUpper() == details.Matricule.ToUpper() && x.NIN.ToUpper() == details.NIN.ToUpper()).Index;

                                Console.WriteLine(JsonConvert.SerializeObject(details, Formatting.Indented));

                                await db.GetCollection<StudentDetail>().UpsertAsync(details);
                            }                         
                        }
                        catch{}                        
                    });           
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message); 
                }
            });
            /*
            Parallel.ForEach(positions.ToList(), async x =>
            {
                try
                {
                    var pageResponse = await apiClient.GetStudentList(javaViewState.First(), pageSize, x, cookies);
                    var studentsResult = scrapper.ExtractData(pageResponse);
                    var keysResult = Worker.ExtractRowKeys(pageResponse);
                    rowKeys.Add(keysResult.AsEnumerable());

                    Task.Run(() =>
                    {
                        foreach (var s in studentsResult)
                        {
                            try
                            {
                                var json = JsonConvert.SerializeObject(s, Formatting.Indented);
                                Console.WriteLine(json);

                                db.GetCollection<UniversityStudent>().Upsert(s);
                            }
                            catch
                            {

                            }
                        }
                    });
                    Task.Run(() =>
                    {
                        foreach (var key in keysResult)
                        {
                            try
                            {
                                db.GetCollection<RowKeyModel>().Upsert(new RowKeyModel()
                                {
                                    Index = key.Key,
                                    RowKey = key.Value
                                });
                            }
                            catch
                            {

                            }
                            
                        }
                    });

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }            
            });

            Parallel.ForEach(rowKeys.SelectMany(x => x).ToList(), async key =>
            {
                try
                {
                    

                    Console.WriteLine(JsonConvert.SerializeObject(key, Formatting.Indented));

                    var pageResponse = await apiClient.GetStudentDetails(javaViewState.First(), key.Value, cookies);
                    db.GetCollection<StudentDetailsResponse>().Insert(new StudentDetailsResponse()
                    {
                        Index = key.Key,
                        RowKey = key.Value,
                        Response = pageResponse
                    });
                }
                catch
                {

                }
            });

            */
            Console.ReadKey();

            //OLD WAY
            /*
            await browser.PressNextAndGetData(true);
            Observable
                .Interval(TimeSpan.FromSeconds(0.5))
                .Repeat()
                .Do(async x => await browser.PressNextButton())
                .Subscribe();
                /*
                .Select(x => browser.PressNextAndGetData().GetAwaiter().GetResult())
                .Where(x => x != null && x.Count() > 0)
                .SelectMany(x => x)
                .Do(x=>
                {
                    try
                    {
                        db.GetCollection<UniversityStudent>().Insert(x);
                    }
                    catch
                    {

                    }
                })
                .Subscribe(x =>
                {
                    var json = JsonConvert.SerializeObject(x, Formatting.Indented);

                    Console.WriteLine(json);
                });*/

            /*Observable
                .Interval(TimeSpan.FromMilliseconds(10))
                .ObserveOn(TaskPoolScheduler.Default)
                .Select(x => Observable.FromAsync(() => browser.Page.GetSourceAsync()))
                .Concat()
                .Repeat()
                .Buffer(1000)
                .SelectMany(x => scrapper.ExtractData(x).Distinct())
                .Do(x =>
                {
                    try
                    {
                        db.GetCollection<UniversityStudent>().Insert(x);
                    }
                    catch
                    {

                    }
                })
                .Subscribe(x =>
                {
                    var json = JsonConvert.SerializeObject(x, Formatting.Indented);

                    Console.WriteLine(json);
                });
            */

            Console.ReadKey();
        }
    }
}
