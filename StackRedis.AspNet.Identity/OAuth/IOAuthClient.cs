using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.AspNet.Identity.OAuth
{
    public interface IOAuthClient
    {
        string Id { get; set; }
        string Name { get; set; }
        string ClientSecretHash { get; set; }
        OAuthGrantType GrantType { get; set; }
        DateTimeOffset DateAdded { get; set; }
    }
}
