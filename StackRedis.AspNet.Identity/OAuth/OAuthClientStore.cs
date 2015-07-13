namespace StackRedis.AspNet.Identity.OAuth
{
    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using System.Collections.Immutable;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    public class OAuthClientStore<TClient> : IDisposable
        where TClient : IOAuthClient, new()
    {
        private readonly ConnectionMultiplexer _multiplexer;

        public OAuthClientStore(ConnectionMultiplexer multiplexer)
        {
            Contract.Requires(multiplexer != null, "ConnectionMultiplexer is mandatory");
            Contract.Assert(!string.IsNullOrEmpty(ClientHashKey), "Application configuration file has not provided OAuth client store hash Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(ClientsByUserSetKey), "Application configuration file has not provided OAuth client store hash Redis key name");

            _multiplexer = multiplexer;
        }

        private ConnectionMultiplexer ConnectionMultiplexer { get { return _multiplexer; } }
        private string ClientHashKey { get { return ConfigurationManager.AppSettings["aspNet:identity:oauth:redis:clientHashKey"]; } }
        private string ClientsByUserSetKey { get { return ConfigurationManager.AppSettings["aspNet:identity:oauth:redis:clientsByUserSetKey"]; } }

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

        public virtual async Task<TClient> RegisterClientAsync(string ownerUserName, string name, OAuthGrantType grantType, ITransaction currentTransaction = null)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));

            TClient client = new TClient();

            client.GrantType = grantType;

            using (RijndaelManaged cryptoManager = new RijndaelManaged())
            {
                cryptoManager.GenerateKey();
                client.Id = BitConverter.ToString(cryptoManager.Key).Replace("-", string.Empty).ToLowerInvariant();

                cryptoManager.GenerateKey();
                client.Secret = BitConverter.ToString(cryptoManager.Key).Replace("-", string.Empty).ToLowerInvariant();
            }

            client.Name = name;
            client.SecretHash = new PasswordHasher().HashPassword(client.Secret);
            client.DateAdded = DateTimeOffset.Now;

            ITransaction transaction = currentTransaction ?? Database.CreateTransaction();

            transaction.HashSetAsync(ClientHashKey, new[] { new HashEntry(client.Id, JsonConvert.SerializeObject(client)) });
            transaction.SetAddAsync(string.Format(ClientsByUserSetKey, ownerUserName), client.Id);

            if (currentTransaction == null)
                await transaction.ExecuteAsync();

            return client;
        }

        public virtual async Task<TClient> GetClientByIdAsync(string id)
        {
            return JsonConvert.DeserializeObject<TClient>(await Database.HashGetAsync(ClientHashKey, id));
        }

        public virtual async Task<ImmutableHashSet<TClient>> GetAllClientsByOwnerUserNameAsync(string ownerUserName)
        {
            RedisValue[] clientIds = await Database.SetMembersAsync(string.Format(ClientsByUserSetKey, ownerUserName));

            return (await Database.HashGetAsync(ClientHashKey, clientIds))
                        .Select(rawClient => JsonConvert.DeserializeObject<TClient>(rawClient))
                        .ToImmutableHashSet(new OAuthClient.OAuthClientEqualityComparer<TClient>());
        }

        public void Dispose()
        {
        }
    }
}
