using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using HtmlAgilityPack;
using ProgresMESRS.Middleware.API.Models;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Jetsons.JetPack;

namespace ProgresMESRS.Middleware.API
{
        public class Worker
        {

        public async Task<int> GetItemsCount()
        {
            var source = await Page.GetSourceAsync();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(source);

            var itemsCount = htmlDoc.DocumentNode
                                            .Descendants("span")
                                            .Where(x => x.HasClass("ui-paginator-current"))
                                            .First()
                                            .InnerText
                                            .Replace("(","")
                                            .Replace(")","")
                                            .Split("sur", StringSplitOptions.None)
                                            .Last()
                                            .ToInt();
            return itemsCount;
        }
        public async Task<string> GetCookies(string url)
        {
            var cookies = await Page.GetCookieManager().VisitUrlCookiesAsync(url, true);

            return cookies.Select(x => $"{x.Name}={x.Value}").Aggregate((a, c) => $"{a}; {c}");
        }
        public static Dictionary<int, string> ExtractRowKeys(string source)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(source);

            var rowKeys = htmlDoc
                .DocumentNode
                .Descendants("update")
                .SelectMany(x => x.Descendants("tr"))
                .Select(x => (x.Descendants("td").ToArray(), x.GetAttributes("data-rk").First().Value))
                .ToDictionary(x => Convert.ToInt32(x.Item1[0].InnerText), x => x.Value);

            return rowKeys;

        }
        public static IEnumerable<string> ExtractJavaViewStatesFromXml(string pageSource)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            var viewStates = htmlDoc
                .DocumentNode
                .Descendants("update")
                .Where(x => x.Id.Contains("javax.faces.ViewState"))
                .SelectMany(x => x.Descendants())
                .Select(x => x.InnerHtml.Replace("<![CDATA[", "").Replace("]]>",""))
                .ToList();

            return viewStates;
        }

        public IEnumerable<string> ExtractJavaViewStates()
        {
            var source = Page.GetSourceAsync().GetAwaiter().GetResult();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(source);

            var viewStates = htmlDoc
                .DocumentNode
                .Descendants("input")
                .Select(x => x.GetAttributes("name", "value"))
                .Where(x => {
                    try
                    {
                        return x.Any(z => z.Value == "javax.faces.ViewState");
                    }
                    catch
                    {
                        return false;
                    }
                })
                .SelectMany(x => x)
                .Where(x => x.Name == "value")
                .Select(x => x.Value)
                .ToList();

            return viewStates;
        }
        public ChromiumWebBrowser Page { get; private set; }
            public RequestContext RequestContext { get; private set; }
            public Worker(string url)
            {
                var settings = new CefSettings()
                {
                    //By default CefSharp will use an in-memory cache, you need to     specify a Cache Folder to persist data
                    CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
                    PersistSessionCookies = true,
                   
                };

                CefSharpSettings.ShutdownOnExit = true;

                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

                RequestContext = new RequestContext();
                Page = new ChromiumWebBrowser(url, null, RequestContext);
                PageInitialize();
                SpinWait.SpinUntil(() => !Page.IsLoading);

            }
        public string GetPageSizeElement(string source)
        {
            var pageSizeSelectId = "form_search:ResultSearchDataTable";

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(source);

            var selectElement = htmlDoc.DocumentNode
                .Descendants("select")
                .Where(x => x.Id.Contains(pageSizeSelectId))
                .First()
                .Id;
            return selectElement;
        }
        public async Task<IEnumerable<UniversityStudent>> PressNextAndGetData(bool firstPage = false)
        {

            StudentsListScrapper scrapper = new StudentsListScrapper();

            var nextButtonClassName = "ui-paginator-next ui-state-default ui-corner-all";
            var searchButtonId = "form_search:btnSearch";


            if (firstPage)
            {
                await ExecuteScript("(function() { document.getElementById('" + searchButtonId + "').click(); })();", "DossiersEtudiants.xhtml");
                await Task.Delay(3000);

                try
                {
                    await ExecuteScript("(function() { document.getElementById('" + GetPageSizeElement(await Page.GetSourceAsync()) + "').selectedIndex = 3; })();", "DossiersEtudiants.xhtml");
                    await Task.Delay(3000);
                }
                catch
                {
                }

            }
            var source = await Page.GetSourceAsync();
            var data = scrapper.ExtractData(source);


            await ExecuteScript("(function() { document.getElementsByClassName('" + nextButtonClassName + "')[0].click(); })();", "DossiersEtudiants.xhtml");

            return data;

        }
        public async Task PressNextButton(bool firstPage = false)
        {
            StudentsListScrapper scrapper = new StudentsListScrapper();
            var nextButtonClassName = "ui-paginator-next ui-state-default ui-corner-all";
            var searchButtonId = "form_search:btnSearch";
            if (firstPage)
            {
                await ExecuteScript("(function() { document.getElementById('" + searchButtonId + "').click(); })();", "DossiersEtudiants.xhtml");
                await Task.Delay(3000);
                try
                {
                    await ExecuteScript("(function() { document.getElementById('" + GetPageSizeElement(await Page.GetSourceAsync()) + "').selectedIndex = 3; })();", "DossiersEtudiants.xhtml");
                    await Task.Delay(3000);
                }
                catch
                {
                }
            }
            await Task.Delay(100);
            await ExecuteScript("(function() { document.getElementsByClassName('" + nextButtonClassName + "')[0].click(); })();", "DossiersEtudiants.xhtml");
        }
        public async Task SearchForStudent(string matricule, string nin)
        {
            var matriculeInputId = "form_search:matriculetudiant";
            var ninInputId = "form_search:ninetudiant";
            var searchButtonId = "form_search:btnSearch";

            await ExecuteScript("(function() { document.getElementById('" + matriculeInputId + "').value = '" + matricule + "'; })();");
            await ExecuteScript("(function() { document.getElementById('" + ninInputId + "').value = '" + nin + "'; })();");
            await ExecuteScript("(function() { document.getElementById('" + searchButtonId + "').click(); })();", "DossiersEtudiants.xhtml");
        }
        public async Task SignIn(string username, string password)
        {
            var usernameFieldId = "loginFrm:j_username";
            var passwordFieldId = "loginFrm:j_password";
            var connectButtonId = "loginFrm:loginBtn";

            await ExecuteScript("(function() { document.getElementById('" + usernameFieldId + "').value = '" + username + "'; })();");
            await ExecuteScript("(function() { document.getElementById('" + passwordFieldId + "').value = '" + password + "'; })();");
            await ExecuteScript("(function() { document.getElementById('" + connectButtonId + "').click(); })();");

        }
        public async Task ExecuteScript(string script, string url = "")
        {
            if(Page.Address.Contains(url, StringComparison.Ordinal) || string.IsNullOrEmpty(url))
            {
                bool executed = false;
                do
                {
                    SpinWait.SpinUntil(() => !Page.IsLoading, TimeSpan.FromSeconds(30));
                    var frame = Page.GetMainFrame();
                    //SpinWait.SpinUntil(() => frame.IsValid && frame.IsMain && frame.IsFocused, TimeSpan.FromSeconds(30));

                    var source = await Page.GetSourceAsync();
                    var response = await frame.EvaluateScriptAsPromiseAsync(script);
                    await Task.Delay(100);
                    executed = response.Success;

                } while (!executed);
            }
        }
        public void StartMonitoring()
        {
            StudentsListScrapper scrapper = new StudentsListScrapper();

            Observable
                .FromEventPattern<LoadingStateChangedEventArgs>(
                x => Page.LoadingStateChanged += x,
                x => Page.LoadingStateChanged -= x)
                .Where(x => Page.IsBrowserInitialized)
                .Do(x => Console.WriteLine(Page.Address))
                .Select(x =>
                {
                    try
                    {
                        var source = Page.GetSourceAsync();
                        Task.WaitAny(source);

                        return scrapper.ExtractData(source.Result);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    var json = JsonConvert.SerializeObject(x, Formatting.Indented);

                    Console.WriteLine(json);
                });
        }           
        public async Task OpenUrl(string url)
        {
            SpinWait.SpinUntil(() => Page.IsBrowserInitialized);

            if (Page.IsBrowserInitialized)
            {
                var pageUri = Page.Address;
                do
                {
                    SpinWait.SpinUntil(() => !Page.IsLoading);
                   
                    var respone = await Page.LoadUrlAsync(url);

                    SpinWait.SpinUntil(() => !Page.IsLoading);

                    pageUri = Page.Address;
                    await Task.Delay(500);

                } while (pageUri != url);
                
            }
        }
        private void PageInitialize()
        {
            SpinWait.SpinUntil(() => Page.IsBrowserInitialized);
        }
   }  
}

