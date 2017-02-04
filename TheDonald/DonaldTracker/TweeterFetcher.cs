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
            Auth.SetUserCredentials(
                "jpYb1C86BqRTVPJ2T2HEV3bnP",
                "CEgWnoFu73O1vyaHgnAK2HiGEor6rl0VzY6TUxKpiyBDqWUMVY",
                "416369686-ueCGvZ4sA2npJWnjPhnKU08dN09WRNe4Ymc9WnpK",
                "XepCv9BdnzgZ9ktLNs83eKySFL3JdFNytlpC30c1a74yb");
            IUser user = User.GetUserFromScreenName("realDonaldTrump");
            List<ITweet> tweets = user.GetUserTimeline().ToList();
            return tweets;
        }

       
    }
}
