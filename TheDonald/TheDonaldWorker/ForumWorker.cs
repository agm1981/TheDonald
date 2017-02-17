using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheDonaldWorker
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
