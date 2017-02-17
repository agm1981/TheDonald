using System;
using Tweetinvi.Core.Extensions;

namespace TheDonaldWorker
{
    public class AccessTokenExpiring
    {
        private string accessToken;

        public AccessTokenExpiring()
        {
            accessToken = string.Empty;
        }

        public string AccessToken
        {
            get
            {
                return accessToken;
            }
            set
            {
                accessToken = value;
                date = DateTime.UtcNow;
            }
        }

        public bool IsTokenValid()
        {
            return !accessToken.IsNullOrEmpty()  && DateTime.UtcNow - date < timeToLive;
        }

        private readonly TimeSpan timeToLive = TimeSpan.FromMinutes(30);
        private DateTime date;
    }
}