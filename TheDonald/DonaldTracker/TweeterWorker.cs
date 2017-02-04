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

            var dataToPublish = tweets.Where(c => data.All(v => v != c.IdStr)).OrderBy(c => c.Id).ToList();

            foreach (ITweet tweetId in dataToPublish)
            {
                ForumWorker fw = new ForumWorker();
                fw.PostTweet(tweetId.IdStr, tweetId.FullText);
                File.AppendAllLines("publishedTweets.txt", new List<string>{ tweetId.IdStr
    });
                Thread.Sleep(new TimeSpan(0, 0, 40));
            }

        }

        internal class teew{
        }
    }
}
