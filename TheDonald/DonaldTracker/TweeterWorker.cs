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
            List<string> tweets = tf.GetLastTweets().OrderBy(c=>c.Id).Select(c => c.IdStr).ToList();
            // get all already published tweets
            List<string> data = File.ReadAllLines("publishedTweets.txt").ToList();

            List<string> dataToPublish = tweets.Except(data).OrderBy(c=>c).ToList();
            
            foreach (string tweetId in dataToPublish)
            {
                ForumWorker fw = new ForumWorker();
                fw.PostTweet(tweetId);
                File.AppendAllLines("publishedTweets.txt", new List<string>{ tweetId});
                Thread.Sleep(new TimeSpan(0, 0, 40));
            }

        }
    }
}
