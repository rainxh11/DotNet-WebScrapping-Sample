using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CefSharp;
using CefSharp.OffScreen;
using Newtonsoft.Json;

namespace ProgresMESRS.Middleware.API
{
    public class ApiClient
    {
        public async Task<string> GetStudentDetailsPOST(ChromiumWebBrowser Page, string javaState, string studentRowKey, string cookies, string matricule = "", string nin = "", string url = "https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml?idf=607250")
        {
            var requestString = @"{
                            ""javax.faces.partial.ajax"":""true"",
                            ""javax.faces.source"":""form_search:ResultSearchDataTable"",
                            ""javax.faces.partial.execute"":""form_search:ResultSearchDataTable"",
                            ""javax.faces.partial.render"":""form_view"",
                            ""javax.faces.behavior.event"":""rowSelect"",
                            ""javax.faces.partial.event"":""rowSelect"",
                            ""form_search:ResultSearchDataTable_instantSelectedRowKey"":""#ROWKEY"",
                            ""form_search"":""form_search"",
                            ""form_search:matriculetudiant"":""#MATRICULE"",
                            ""form_search:ninetudiant"":""#NIN"",
                            ""form_search:ResultSearchDataTable_rppDD"":""100"",
                            ""form_search:ResultSearchDataTable_selection"":""#ROWKEY"",
                            ""javax.faces.ViewState"":""#VIEWSTATE""}"
                            .Replace("#ROWKEY", studentRowKey)
                            .Replace("#VIEWSTATE", javaState)
                            .Replace("#MATRICULE", matricule)
                            .Replace("#NIN", nin);

            var requestDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestString);
            var content = new FormUrlEncodedContent(requestDict);
            var postData = await content.ReadAsByteArrayAsync();

            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                                { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded; charset=UTF-8" },
                                { HttpRequestHeader.Accept.ToString(), "application/xml, text/xml, */*; q=0.01" },
                                { HttpRequestHeader.AcceptEncoding.ToString(), "gzip, deflate, br" },
                                { HttpRequestHeader.Connection.ToString(), "keep-alive" },
                                { HttpRequestHeader.Cookie.ToString(), cookies },
                                { "DNT", "1"},
                                {"Pragma", "no-cache"},
                                {"Cache-Control", "no-cache"},
                                { "Faces-Request", "partial/ajax" },
                                { "X-Requested-With", "XMLHttpRequest" },
                                { "Referer", "https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml?idf=607250"},
                                {"Sec-Fetch-Site", "same-origin"},
                                {"Sec-Fetch-Mode", "cors"},
                                {"Sec-Fetch-Dest", "empty"},
                                {"sec-ch-ua-platform", "Windows"},
                                {"Origin", "https://progres.mesrs.dz"},
                                {"sec-ch-ua-mobile", "?0"},
                                {"Host", "progres.mesrs.dz"}
            };
            var requestHeaders = new System.Collections.Specialized.NameValueCollection();
            foreach(var dict in headers)
            {
                requestHeaders.Add(dict.Key, dict.Value);
            }

            var frame = Page.GetMainFrame();
            var request = frame.CreateRequest();
            request.InitializePostData();

            request.Headers = requestHeaders;
            request.Url = url;
            request.Method = "POST";

            Page.LoadUrlWithPostData(url, postData, "application/x-www-form-urlencoded; charset=UTF-8");
            //SpinWait.SpinUntil(() => !Page.IsLoading);
            await Task.Run(async () =>
            {
                while (!Page.IsLoading)
                {
                    await Task.Delay(250);
                }
            });


            return await Page.GetSourceAsync();
        }
        public async Task<string> GetStudentDetails(string javaState, string studentRowKey, string cookies, string matricule = "", string nin = "", string url = "https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml?idf=607250")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(120);
                    var requestString = @"{
                            ""javax.faces.partial.ajax"":""true"",
                            ""javax.faces.source"":""form_search:ResultSearchDataTable"",
                            ""javax.faces.partial.execute"":""form_search:ResultSearchDataTable"",
                            ""javax.faces.partial.render"":""form_view"",
                            ""javax.faces.behavior.event"":""rowSelect"",
                            ""javax.faces.partial.event"":""rowSelect"",
                            ""form_search:ResultSearchDataTable_instantSelectedRowKey"":""#ROWKEY"",
                            ""form_search"":""form_search"",
                            ""form_search:matriculetudiant"":""#MATRICULE"",
                            ""form_search:ninetudiant"":""#NIN"",
                            ""form_search:ResultSearchDataTable_rppDD"":""100"",
                            ""form_search:ResultSearchDataTable_selection"":""#ROWKEY"",
                            ""javax.faces.ViewState"":""#VIEWSTATE""}"
                            .Replace("#ROWKEY", studentRowKey)
                            .Replace("#VIEWSTATE", javaState)
                            .Replace("#MATRICULE", matricule)
                            .Replace("#NIN", nin);

                    var requestDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestString);

                    var content = new FormUrlEncodedContent(requestDict);

                    var httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(url),
                        Headers =
                            {
                                { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded; charset=UTF-8" },
                                { HttpRequestHeader.Accept.ToString(), "application/xml, text/xml, */*; q=0.01" },
                                { HttpRequestHeader.AcceptEncoding.ToString(), "gzip, deflate, br" },
                                { HttpRequestHeader.Connection.ToString(), "keep-alive" },
                                { HttpRequestHeader.Cookie.ToString(), cookies },
                                { "DNT", "1"},
                                {"Pragma", "no-cache"},
                                {"Cache-Control", "no-cache"},
                                { "Faces-Request", "partial/ajax" },
                                { "X-Requested-With", "XMLHttpRequest" },
                                { "Referer", "https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml?idf=607250"},
                                {"Sec-Fetch-Site", "same-origin"},
                                {"Sec-Fetch-Mode", "cors"},
                                {"Sec-Fetch-Dest", "empty"},
                                {"sec-ch-ua-platform", "Windows"},
                                {"Origin", "https://progres.mesrs.dz"},
                                {"sec-ch-ua-mobile", "?0"},
                                {"Host", "progres.mesrs.dz"},
                        },
                        Content = content
                    };

                    var response = client.Send(httpRequestMessage);

                    var source =  await response.Content.ReadAsStringAsync();
                    return source;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return String.Empty;
            }
        }
        public async Task<string> GetStudentList(string javaState, int pageSize, int pagePosition, string cookies, string matricule = "", string nin = "", string url = "https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml")
        {
            try
            {
                using(var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    var requestString = @"{""javax.faces.partial.ajax"":""true"",
                            ""javax.faces.source"":""form_search:ResultSearchDataTable"",
                            ""javax.faces.partial.execute"":""form_search:ResultSearchDataTable"",
                            ""javax.faces.partial.render"":""form_search:ResultSearchDataTable"",
                            ""javax.faces.behavior.event"":""page"",
                            ""javax.faces.partial.event"":""page"",
                            ""form_search:ResultSearchDataTable_pagination"":""true"",
                            ""form_search:ResultSearchDataTable_first"":""#PAGEPOSITION"",
                            ""form_search:ResultSearchDataTable_rows"":""#PAGESIZE"",
                            ""form_search:ResultSearchDataTable_skipChildren"":""true"",
                            ""form_search:ResultSearchDataTable_encodeFeature"":""true"",
                            ""form_search"":""form_search"",
                            ""form_search:matriculetudiant"":""#MATRICULE"",
                            ""form_search:ninetudiant"":""#NIN"",
                            ""form_search:ResultSearchDataTable_rppDD"":""#PAGESIZE"",
                            ""form_search:ResultSearchDataTable_selection"":"""",
                            ""javax.faces.ViewState"":""#VIEWSTATE""}"
                            .Replace("#PAGESIZE", pageSize.ToString())
                            .Replace("#PAGEPOSITION", pagePosition.ToString())
                            .Replace("#VIEWSTATE", javaState)
                            .Replace("#MATRICULE", matricule)
                            .Replace("#NIN", nin);

                    var requestDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestString);

                    var content = new FormUrlEncodedContent(requestDict);

                    var httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(url),
                        Headers =
                {
                    { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
                    { HttpRequestHeader.Accept.ToString(), "*/*" },
                    { HttpRequestHeader.AcceptEncoding.ToString(), "gzip, deflate, br" },
                    { HttpRequestHeader.Connection.ToString(), "keep-alive" },
                    { HttpRequestHeader.Cookie.ToString(), cookies },
                    { "Faces-Request", "partial/ajax" },
                    { "X-Requested-With", "XMLHttpRequest" }
                },
                        Content = content
                    };

                    var response = client.Send(httpRequestMessage);

                    return await response.Content.ReadAsStringAsync();
                }             
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return String.Empty;
            }
            
        }
        public async Task<string> GetStudentSearchList(string javaState, int pageSize, string cookies, string matricule = "", string nin = "", string url = "https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    var requestString = @"{""javax.faces.partial.ajax"":""true"",
                            ""javax.faces.source"":""form_search:btnSearch"",
                            ""javax.faces.partial.execute"":""form_search"",
                            ""javax.faces.partial.render"":""form_search:ResultSearchDataTable form_view"",
							""form_search:btnSearch"":""form_search:btnSearch"",
                            ""form_search"":""form_search"",
                            ""form_search:matriculetudiant"":""#MATRICULE"",
                            ""form_search:ninetudiant"":""#NIN"",
                            ""form_search:ResultSearchDataTable_rppDD"":""#PAGESIZE"",
                            ""form_search:ResultSearchDataTable_selection"":"""",
                            ""javax.faces.ViewState"":""#VIEWSTATE""}"
                            .Replace("#PAGESIZE", pageSize.ToString())
                            .Replace("#VIEWSTATE", javaState)
                            .Replace("#MATRICULE", matricule)
                            .Replace("#NIN", nin);

                    var requestDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestString);

                    var content = new FormUrlEncodedContent(requestDict);

                    var httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(url),
                        Headers =
                {
                    { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
                    { HttpRequestHeader.Accept.ToString(), "*/*" },
                    { HttpRequestHeader.AcceptEncoding.ToString(), "gzip, deflate, br" },
                    { HttpRequestHeader.Connection.ToString(), "keep-alive" },
                    { HttpRequestHeader.Cookie.ToString(), cookies },
                    { "Faces-Request", "partial/ajax" },
                    { "X-Requested-With", "XMLHttpRequest" }
                },
                        Content = content
                    };

                    var response = client.Send(httpRequestMessage);

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return String.Empty;
            }

        }

    }
}
