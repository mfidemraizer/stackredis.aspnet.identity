namespace StackRedis.AspNet.Identity.OAuth
{
    using System;

    public class OAuthClient : IOAuthClient
    {
        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string ClientSecretHash
        {
            get;
            set;
        }

        public OAuthGrantType GrantType
        {
            get;
            set;
        }

        public DateTimeOffset DateAdded
        {
            get;
            set;
        }
    }
}