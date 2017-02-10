using System;
using System.Net;

namespace DonaldTracker
{
    public class CookieExpiring
    {
        private CookieCollection cookie;

        public CookieExpiring()
        {
            cookie = new CookieCollection();
        }

        public CookieCollection Cookie
        {
            get
            {
                return cookie;
            }
            set
            {
                cookie = value;
                date = DateTime.UtcNow;
            }
        }

        public bool IsTokenValid()
        {
            return Cookie.Count > 0 && DateTime.UtcNow - date < timeToLive;
        }

        private TimeSpan timeToLive = TimeSpan.Minutes(10);
        private DateTime date;
    }
}
