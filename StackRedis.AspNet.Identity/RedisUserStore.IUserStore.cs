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
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class RedisUserStore<TUser> : IUserStore<TUser>
    {
        public virtual async Task<TUser> FindAsync(UserLoginInfo login)
        {
            IDatabase db = Database;

            string userId = await db.HashGetAsync(UserLoginHashKey, string.Format("{0}:{1}", login.LoginProvider, login.ProviderKey));

            if (!string.IsNullOrEmpty(userId))
            {
                return JsonConvert.DeserializeObject<TUser>(await db.HashGetAsync(UserHashByIdKey, userId));
            }
            else
            {
                return null;
            }
        }

#pragma warning disable 4014
        public virtual async Task CreateAsync(TUser user)
        {
            IIdentityUser identityUser = user;

            IDatabase db = Database;
            ITransaction transaction = CreateTransaction();

            if (string.IsNullOrEmpty(identityUser.Id))
            {
                identityUser.Id = (await db.StringIncrementAsync(UserIdKey)).ToString();
            }

            transaction.HashSetAsync
            (
                UserHashByIdKey,
                new[] 
                { 
                    new HashEntry(identityUser.Id, JsonConvert.SerializeObject(user))
                }
            );

            transaction.HashSetAsync
            (
                UserHashByNameKey,
                new[] 
                { 
                    new HashEntry(user.UserName, identityUser.Id)
                }
            );

            await transaction.ExecuteAsync();
        }

        public virtual async Task DeleteAsync(TUser user)
        {
            IList<UserLoginInfo> logins = await GetLoginsAsync(user);
            string userId = ((IUser)user).Id;

            ITransaction transaction = CreateTransaction();

            // Hashes
            transaction.HashDeleteAsync(UserHashByIdKey, userId);
            transaction.HashDeleteAsync(UserHashByNameKey, user.UserName);
            transaction.HashDeleteAsync(UserLoginHashKey, logins.Select(login => (RedisValue)string.Format("{0}:{1}", login.LoginProvider, login.ProviderKey)).ToArray());
            transaction.HashDeleteAsync(UserLoginFailCountHashKey, userId);
            transaction.HashDeleteAsync(UserLockDateHashKey, userId);

            // Sets
            transaction.KeyDeleteAsync(UserLoginSetKey);
            transaction.KeyDeleteAsync(UserClaimSetKey);
            if (null != user.Email)
            {
                transaction.SetRemoveAsync(UserConfirmedEmailSetKey, user.Email);
            }
            if (null != user.PhoneNumber)
            {
                transaction.SetRemoveAsync(UserConfirmedPhoneNumberSetKey, user.PhoneNumber);
            }
            transaction.SetRemoveAsync(UserClaimSetKey, userId);
            transaction.SetRemoveAsync(UserLockSetKey, userId);

            await transaction.ExecuteAsync();
        }
#pragma warning restore 4014

        public virtual async Task<TUser> FindByIdAsync(string userId)
        {
            IDatabase db = Database;

            return JsonConvert.DeserializeObject<TUser>(await db.HashGetAsync(UserHashByIdKey, userId));
        }

        public virtual async Task<TUser> FindByNameAsync(string userName)
        {
            IDatabase db = Database;

            string userId = await db.HashGetAsync(UserHashByNameKey, userName);

            if (!string.IsNullOrEmpty(userId))
            {
                return JsonConvert.DeserializeObject<TUser>(await db.HashGetAsync(UserHashByIdKey, userId));
            }
            else
            {
                return null;
            }
        }

        public virtual Task UpdateAsync(TUser user)
        {
            return CreateAsync(user);
        }
    }
}