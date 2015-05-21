namespace StackRedis.AspNet.Identity.OAuth
{
    using System;
    using System.Collections.Generic;

    public class OAuthClient : IOAuthClient, IReadOnlyOAuthClient
    {
        internal class OAuthClientEqualityComparer<TClient> : IEqualityComparer<TClient>
            where TClient : IOAuthClient
        {
            public bool Equals(TClient x, TClient y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(TClient obj)
            {
                return obj.GetHashCode();
            }
        }

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

        public string Secret
        {
            get;
            set;
        }

        public string SecretHash
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