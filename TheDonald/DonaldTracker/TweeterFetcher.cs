using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace DonaldTracker
{
    public class TweeterFetcher
    {
        public List<ITweet> GetLastTweets()
        {
            Auth.SetUserCredentials(// credentials here
                "","","",""
              );
            IUser user = User.GetUserFromScreenName("realDonaldTrump");
            List<ITweet> tweets = user.GetUserTimeline().ToList();
            return tweets;
        }

       
    }
}
