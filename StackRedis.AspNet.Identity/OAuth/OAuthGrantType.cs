using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.AspNet.Identity.OAuth
{
    public enum OAuthGrantType
    {
        Code,
        Implicit,
        ResourceOwner,
        Client
    }
}
