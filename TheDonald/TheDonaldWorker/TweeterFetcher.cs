using System.Collections.Generic;
using System.Linq;
using Tweetinvi;
using Tweetinvi.Models;

namespace TheDonaldWorker
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
