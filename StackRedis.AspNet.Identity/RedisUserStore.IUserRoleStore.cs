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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class RedisUserStore<TUser> : IUserRoleStore<TUser>
    {
        public virtual Task<bool> HasRole(TUser user, string roleName)
        {
            return Database.SetContainsAsync(string.Format(UserRoleSetKey, ((IUser)user).Id), roleName);
        }

        public virtual Task AddToRoleAsync(TUser user, string roleName)
        {
            return Database.SetAddAsync(string.Format(UserRoleSetKey, ((IUser)user).Id), roleName);
        }

        public virtual async Task<IList<string>> GetRolesAsync(TUser user)
        {
            return (IList<string>)(await Database.SetMembersAsync(string.Format(UserRoleSetKey, ((IUser)user).Id)))
                                                            .Select(rawRole => (string)rawRole)
                                                            .ToList();
        }

        public virtual Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return Database.SetContainsAsync(string.Format(UserRoleSetKey, ((IUser)user).Id), roleName);
        }

        public virtual Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            return Database.SetRemoveAsync(string.Format(UserRoleSetKey, ((IUser)user).Id), roleName);
        }
    }
}