using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DonaldTracker
{
    public class ForumWorker
    {
        public void PostTweet(string tweet, string tweetText)
        {
            FiresOfHeavenSite site = new FiresOfHeavenSite();
            Task r = site.PostTweet(tweet, tweetText);
            Task.WaitAll(r);
        }
    }

    public class FiresOfHeavenSite : Site
    {
        private string UrlLogin = @"https://www.firesofheaven.org/login/login";
        private string homePage = "https://www.firesofheaven.org/login/?&_xfRequestUri=%2F&_xfNoRedirect=1&_xfResponseType=json";
        private static string loginCookie = "";

        private static object mylock = new object();

        private static CookieExpiring cookie = new CookieExpiring();
        protected CookieCollection GetAuthorizationCookie()
        {
            lock (mylock)
            {

                if (cookie.IsTokenValid())
                {
                    return cookie.Cookie;
                }

                var cookies = new CookieContainer();
                var handler = new HttpClientHandler
                {
                    CookieContainer = cookies,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    AllowAutoRedirect = true
                };
                HttpClient wc = SetupClient(handler);
                wc.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                wc.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, sdch");
                wc.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8");
                wc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.94 Safari/537.36");


                HttpResponseMessage result = StartSessionAsync(wc).Result;
                var data = cookies.GetCookies(new Uri("https://www.firesofheaven.org"));

                cookie.Cookie = data;
                return data;
            }
        }



        protected internal override HttpClient SetupClient()
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true
            };
            HttpClient wc = SetupClient(handler);
            wc.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            wc.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, sdch");
            wc.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8");
            wc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
            return wc;
        }

        protected internal async Task<HttpResponseMessage> StartSessionAsync(HttpClient wc)
        {
            const string username = "Donald Trump";
            const string password = "MAGA2017";

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("login", username),
                new KeyValuePair<string, string>("register", "0"),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("cookie_check", "1"),
                new KeyValuePair<string, string>("remember", "1"),
                new KeyValuePair<string, string>("_xfToken", ""),
                new KeyValuePair<string, string>("redirect", "https://www.firesofheaven.org/")
            });
            HttpResponseMessage result = await wc.PostAsync(UrlLogin, formContent);
            //string code = result.Content.ReadAsStringAsync().Result;
            return result;
        }


        //public override async Task<TaskResult> ExecuteAgentTask(int attachmentId)
        //{
        //    string url = $"http://www.rerolled.org/attachment.php?attachmentid={attachmentId}";
        //    try
        //    {

        //        CookieCollection cookies = GetAuthorizationCookie();
        //        if (cookies.Count <= 2)
        //        {
        //            throw new Exception("Could not start session ");

        //        }
        //        // now add cookie ? ithink, maybe not
        //        HttpClientHandler handler = new HttpClientHandler();
        //        handler.CookieContainer.Add(cookies);
        //        using (var wc = SetupClient(handler))
        //        {
        //            wc.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        //            wc.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, sdch");
        //            wc.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8");
        //            wc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.94 Safari/537.36");
        //            HttpResponseMessage message = await wc.GetAsync(url);
        //            var result = new TaskResult
        //            {
        //                FileBytes = await message.Content.ReadAsByteArrayAsync(),
        //                MimeType = message.Content.Headers.ContentType,
        //                FileName = message.Content.Headers.ContentDisposition.FileName,
        //                Success = true,
        //                Errors = null,
        //                Url = url
        //            };

        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new TaskResult
        //        {
        //            Errors = ex,
        //            Success = false,
        //            Url = url
        //        };
        //    }
        //}

        public async Task PostTweet(string tweet, string tweetText)
        {
            using (HttpClient wc = SetupClient())
            {
                HttpResponseMessage home = await wc.GetAsync(homePage);
                HttpResponseMessage logged = await StartSessionAsync(wc);

                string html = await logged.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                string token = null;
                List<HtmlNode> inputs = doc.DocumentNode.SelectNodes("//input").ToList();
                foreach (HtmlNode htmlNode in inputs)
                {
                    if (htmlNode.Attributes["name"]?.Value == "_xfToken")
                    {
                        token = htmlNode.Attributes["value"].Value;
                        break;
                    }

                }
                HttpResponseMessage mediaData = await PostTweetInner(wc, tweet, token, tweetText);
                Thread.Sleep(new TimeSpan(0, 0, 40));
                HttpResponseMessage postData = await PostProfilePost(wc, tweet, token, tweetText);
            }
        }

        public async Task<HttpResponseMessage> PostTweetInner(HttpClient wc, string tweetId, string token, string tweetText)
        {
            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("message_html", $"<p>[MEDIA=twitter]{tweetId}[/MEDIA]</p><br /> <p>{tweetText}</p>"),
                new KeyValuePair<string, string>("_xfRelativeResolver", "https://www.firesofheaven.org/threads/politics-thread.7113/page-10117"),

                new KeyValuePair<string, string>("last_date", ToUnixTimestamp(DateTime.UtcNow).ToString()),
                new KeyValuePair<string, string>("last_known_date", ToUnixTimestamp(DateTime.UtcNow).ToString()),
                new KeyValuePair<string, string>("_xfToken", token),

                new KeyValuePair<string, string>("_xfRequestUri", "https://www.firesofheaven.org/threads/politics-thread.7113/page-10117"),
                new KeyValuePair<string, string>("_xfNoRedirect", "1"),
                new KeyValuePair<string, string>("_xfResponseType", "json")
            });
            HttpResponseMessage result = await wc.PostAsync("https://www.firesofheaven.org/threads/politics-thread.7113/add-reply", formContent);

            return result;


        }

        public async Task<HttpResponseMessage> PostProfilePost(HttpClient wc, string tweetId, string token, string tweetText)
        {
            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("message", $"{tweetText}"),
                new KeyValuePair<string, string>("_xfToken", token),
                new KeyValuePair<string, string>("simple", "1"),
                new KeyValuePair<string, string>("_xfRequestUri", "/"),
                new KeyValuePair<string, string>("_xfNoRedirect", "1"),
                new KeyValuePair<string, string>("_xfResponseType", "json")//donald-trump.10192
            });
            HttpResponseMessage result = await wc.PostAsync("https://www.firesofheaven.org/members/donald-trump.10192/post", formContent);

            return result;


        }
        public static long ToUnixTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
        }
    }
}
