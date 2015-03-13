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
    using System.Threading.Tasks;

    public partial class RedisUserStore<TUser> : IUserLockoutStore<TUser, string>
    {
        public virtual async Task<int> GetAccessFailedCountAsync(TUser user)
        {
            IDatabase db = Database;

            int count;

            int.TryParse(await db.HashGetAsync(UserLoginFailCountHashKey, ((IUser)user).Id), out count);

            return count;
        }

        public virtual Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult(true);
        }

        public virtual async Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return DateTimeOffset.FromFileTime(long.Parse(await Database.HashGetAsync(UserLockDateHashKey, ((IUser)user).Id)));
        }

        public virtual async Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return (int)await Database.HashIncrementAsync(UserLoginFailCountHashKey, ((IUser)user).Id);
        }

        public virtual Task ResetAccessFailedCountAsync(TUser user)
        {
            return Database.HashDeleteAsync(UserLoginFailCountHashKey, ((IUser)user).Id);
        }

        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (string.IsNullOrEmpty(((IUser)user).Id))
            {
                return Task.FromResult(false);
            }

            return Database.SetAddAsync(UserLockSetKey, ((IUser)user).Id);
        }

        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            return Database.HashSetAsync(UserLockDateHashKey, new[] { new HashEntry(((IUser)user).Id, lockoutEnd.ToFileTime()) });
        }
    }
}