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
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class RedisUserStore<TUser> : IUserLoginStore<TUser>, IUserStore<TUser>
    {
        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            return AddManyLoginsAsync(user, new[] { login });
        }

        protected virtual Task AddManyLoginsAsync(TUser user, IEnumerable<UserLoginInfo> logins, ITransaction transaction = null)
        {
            Contract.Requires(logins != null && logins.Count() > 0);

            IDatabaseAsync db = transaction == null ? Database : (IDatabaseAsync)transaction;

            Task setTask = db.SetAddAsync
            (
                string.Format(UserLoginSetKey, ((IUser)user).Id),
                logins.Select(login => (RedisValue)JsonConvert.SerializeObject(login)).ToArray()
            );

            Task hashTask = db.HashSetAsync
            (
                UserLoginHashKey,
                logins.Select(login => new HashEntry(string.Format("{0}:{1}", login.LoginProvider, login.ProviderKey), ((IUser)user).Id)).ToArray()
            );

            return Task.WhenAll(setTask, hashTask);
        }

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return (IList<UserLoginInfo>)(await Database.SetMembersAsync(string.Format(UserLoginSetKey, ((IUser)user).Id)))
                               .Select(rawLogin => JsonConvert.DeserializeObject<UserLoginInfo>(rawLogin))
                               .ToList();
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            return Database.SetRemoveAsync(string.Format(UserLoginSetKey, ((IUser)user).Id), JsonConvert.SerializeObject(login));
        }
    }
}