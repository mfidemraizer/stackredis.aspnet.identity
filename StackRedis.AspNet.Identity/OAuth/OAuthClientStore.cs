namespace StackRedis.AspNet.Identity.OAuth
{
    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class OAuthClientManager<TClient>
        where TClient : IOAuthClient, new()
    {
        private readonly ConnectionMultiplexer _multiplexer;

        public OAuthClientManager(ConnectionMultiplexer multiplexer)
        {
            Contract.Requires(multiplexer != null, "ConnectionMultiplexer is mandatory");
            Contract.Assert(!string.IsNullOrEmpty(ClientHash), "Application configuration file has not provided OAuth client store hash Redis key name");

            _multiplexer = multiplexer;
        }

        private ConnectionMultiplexer ConnectionMultiplexer { get { return _multiplexer; } }
        private string ClientHash { get { return ConfigurationManager.AppSettings["aspNet:identity:oauth:redis:clientHash"]; } }

        protected virtual int DbNumber
        {
            get
            {
                int dbNumber;

                int.TryParse(ConfigurationManager.AppSettings["aspNet:identity:redis:db"], out dbNumber);

                return dbNumber;
            }
        }

        protected virtual IDatabase Database
        {
            get { return ConnectionMultiplexer.GetDatabase(DbNumber); }
        }

        public virtual async Task<TClient> RegisterClientAsync(string name, OAuthGrantType grantType, Func<string> getSecretWordToBuildClientHash)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(getSecretWordToBuildClientHash != null);

            TClient client = new TClient();

            using (RijndaelManaged cryptoManager = new RijndaelManaged())
            {
                cryptoManager.GenerateKey();

                client.Id = Encoding.UTF8.GetString(cryptoManager.Key);
            }

            client.Name = name;
            client.ClientSecretHash = new PasswordHasher().HashPassword(getSecretWordToBuildClientHash());
            client.DateAdded = DateTimeOffset.Now;

            await Database.HashSetAsync(ClientHash, new[] { new HashEntry(client.Id, JsonConvert.SerializeObject(client)) });

            return client;
        }

        public virtual async Task<TClient> GetClientById(string id)
        {
            return JsonConvert.DeserializeObject<TClient>(await Database.HashGetAsync(ClientHash, id));
        }
    }
}
