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
    using System.Threading.Tasks;

    public partial class RedisUserStore<TUser> : IUserTwoFactorStore<TUser, string>
    {
        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Database.SetContainsAsync(TwoFactorEnabledSetKey, ((IUser)user).Id);
        }

        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if(enabled)
            {
                return Database.SetAddAsync(TwoFactorEnabledSetKey, ((IUser)user).Id);
            }
            else
            {
                return Database.SetRemoveAsync(TwoFactorEnabledSetKey, ((IUser)user).Id);
            }
        }
    }
}