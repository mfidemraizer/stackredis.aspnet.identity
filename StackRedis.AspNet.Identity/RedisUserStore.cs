/*
    Copyright 2015 Matías Fidemraizer (https://linkedin.com/in/mfidemraizer)
    
    "StackRedis.AspNet.Identity" project (https://github.com/mfidemraizer/StackRedis.AspNet.Identity)

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
 
    You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

namespace StackRedis.AspNet.Identity
{
    using Microsoft.AspNet.Identity;
    using StackExchange.Redis;
    using System;
    using System.Configuration;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Implements a Redis-based ASP.NET Identity custom store on top of StackExchange.Redis.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public partial class RedisUserStore<TUser>
        where TUser : class, IUser, IIdentityUser
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public RedisUserStore(ConnectionMultiplexer connectionMultiplexer)
        {
            Contract.Assert(!string.IsNullOrEmpty(UserHashByIdKey), "Application configuration file has not provided ASP.NET Identity user hash by id Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserHashByNameKey), "Application configuration file has not provided ASP.NET Identity user hash by name Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserLoginHashKey), "Application configuration file has not provided ASP.NET Identity user hash by name Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserLoginFailCountHashKey), "Application configuration file has not provided ASP.NET Identity user login fail count hash Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserLockSetKey), "Application configuration file has not provided ASP.NET Identity user lock set Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserLockDateHashKey), "Application configuration file has not provided ASP.NET Identity user lock date Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserConfirmedEmailSetKey), "Application configuration file has not provided ASP.NET Identity user confirmed email set Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserConfirmedPhoneNumberSetKey), "Application configuration file has not provided ASP.NET Identity user confirmed phone number set Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserRoleSetKey), "Application configuration file has not provided ASP.NET Identity user role set Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserLoginSetKey), "Application configuration file has not provided ASP.NET Identity user login set Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserClaimSetKey), "Application configuration file has not provided ASP.NET Identity user claim set Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(TwoFactorEnabledSetKey), "Application configuration file has not provided ASP.NET Identity two factor enabled hash Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(UserIdKey), "Application configuration file has not provided ASP.NET Identity data hash Redis key name");
            Contract.Assert(!string.IsNullOrEmpty(ConfigurationManager.AppSettings["aspNet:identity:redis:db"]), "Application configuration file has not provided ASP.NET Identity Redis database number");

            _connectionMultiplexer = connectionMultiplexer;
        }

        private ConnectionMultiplexer ConnectionMultiplexer { get { return _connectionMultiplexer; } }
        private string UserHashByIdKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userHashByIdKey"]; } }
        private string UserHashByNameKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userHashByNameKey"]; } }
        private string UserLoginHashKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userLoginHashKey"]; } }
        private string UserLoginFailCountHashKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userLoginFailCountHashKey"]; } }
        private string UserLockSetKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userLockSetKey"]; } }
        private string UserLockDateHashKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userLockDateHashKey"]; } }
        private string UserConfirmedEmailSetKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userConfirmedEmailSetKey"]; } }
        private string UserConfirmedPhoneNumberSetKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userConfirmedPhoneNumberSetKey"]; } }
        private string UserRoleSetKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userRoleSetKey"]; } }
        private string UserLoginSetKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userLoginSetKey"]; } }
        private string UserClaimSetKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userClaimSetKey"]; } }
        private string TwoFactorEnabledSetKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:twoFactorEnabledSetKey"]; } }
        private string UserIdKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userIdKey"]; } }

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

        protected virtual ITransaction CreateTransaction()
        {
            return Database.CreateTransaction();
        }

        public virtual void Dispose()
        {
        }
    }
}