using System.Collections.Generic;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace DonaldTracker
{
    public class ForumWorker
    {
        public void PostTweets(IEnumerable<TweetData> data)
        {
            FiresOfHeavenSite site = new FiresOfHeavenSite();
            Task r = site.PostTweets(data);
            Task.WaitAll(r);
        }
    }
}
