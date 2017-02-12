using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Tweetinvi.Core.Extensions;
//using Tweetinvi.Core.Helpers;

namespace DonaldTracker
{
    public class FiresOfHeavenSite : Site
    {
        private string UrlLogin = @"https://www.firesofheaven.org/api/index.php?oauth/token";
        private string postEndpoint = "https://www.firesofheaven.org/api/index.php?posts";
        //private static string loginCookie = "";

        private static object mylock = new object();

        private static AccessTokenExpiring token = new AccessTokenExpiring();

        protected string GetAuthorizationToken()
        {
            string tokenVal;
            lock (mylock)
            {

                if (token.IsTokenValid())
                {
                    tokenVal = token.AccessToken;
                }
                else
                {
                    HttpClient wc = SetupClient();
                    wc.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                    wc.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, sdch");
                    wc.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8");
                    wc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.94 Safari/537.36");

                    HttpResponseMessage result = StartSessionAsync(wc).Result;
                    string json = result.Content.ReadAsStringAsync().Result;
                    dynamic d = JObject.Parse(json);
                    tokenVal = d.access_token;
                    token.AccessToken = tokenVal;

                }
            }
            return tokenVal;
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
            const string apiKey = "32wfy7i91d";
            const string apiSecret = "9yof9rdfs518hvi";

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", apiKey),
                new KeyValuePair<string, string>("client_secret", apiSecret),
                new KeyValuePair<string, string>("username", "Donald Trump"),
                new KeyValuePair<string, string>("password", "MAGA2020")
            });
            HttpResponseMessage result = await wc.PostAsync(UrlLogin, formContent);
            //string code = result.Content.ReadAsStringAsync().Result;
            return result;
        }

        public async Task PostTweets(IEnumerable<TweetData> data)
        {
            using (HttpClient wc = SetupClient())
            {
                //HttpResponseMessage home = await wc.GetAsync(homePage);
                string accessToken = GetAuthorizationToken();
                if (accessToken.IsNullOrEmpty())
                {
                    throw new Exception("Unable to log on");
                }

                foreach (TweetData tweetData in data)
                {
                    await PostTweetInner(wc, tweetData.TweetId, tweetData.FullText, accessToken);
                    Console.WriteLine(tweetData);
                    Thread.Sleep(new TimeSpan(0, 0, 1));

                }


            }
        }

        public async Task<HttpResponseMessage> PostTweetInner(HttpClient wc, string tweetId, string tweetText, string bearerToken)
        {
            wc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            int threadid = 7113;
            //outerPost post = new outerPost
            //{
            //    post = new FohPost
            //    {
            //        Thread_id = threadid.ToString(),
            //        Post_body = $"<p>[MEDIA=twitter]{tweetId}[/MEDIA]</p><br /> <p>{tweetText}</p>",
            //    }
            //};

            // donald is 10192
            //int forum_id = 20;

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("thread_id", threadid.ToString()),
                //new KeyValuePair<string, string>("forum_id", 20.ToString()),
                //new KeyValuePair<string, string>("user_id", 10192.ToString()),
                new KeyValuePair<string, string>("post_body", $@"[MEDIA=twitter]{tweetId}[/MEDIA]


                {tweetText}")
            });
          


          

            HttpResponseMessage result = await wc.PostAsync(postEndpoint, formContent);
            
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