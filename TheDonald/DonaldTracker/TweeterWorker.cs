using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace DonaldTracker
{
    public class TweeterWorker
    {
        public void GetAndPublish()
        {
            // Ok first is get all tweets
            TweeterFetcher tf = new TweeterFetcher();
            List<ITweet> tweets = tf.GetLastTweets().OrderBy(c => c.Id).ToList();

            // get all already published tweets
            var data = File.ReadAllLines("publishedTweets.txt").ToList();

            List<ITweet> dataToPublish = tweets.Where(c => data.All(v => v != c.IdStr)).OrderBy(c => c.Id).ToList();
            ForumWorker fw = new ForumWorker();
            fw.PostTweets(
                dataToPublish.Select(c => new TweetData
                {
                    FullText = c.FullText,
                    TweetId = c.IdStr
                }));
                

            
                File.AppendAllLines("publishedTweets.txt", dataToPublish.Select(c=>c.IdStr));
                Thread.Sleep(new TimeSpan(0, 0, 40));
            

        }

        
    }
    public class TweetData
    {
        public string TweetId
        {
            get;
            set;
        }
        public string FullText
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"{nameof(TweetId)}: {TweetId}, {nameof(FullText)}: {FullText}";
        }
    }
}
