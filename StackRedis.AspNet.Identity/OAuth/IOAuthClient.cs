namespace StackRedis.AspNet.Identity.OAuth
{
    using System;

    public interface IOAuthClient
    {
        string Id { get; set; }
        string Name { get; set; }
        string Secret { get; set; }
        string SecretHash { get; set; }
        OAuthGrantType GrantType { get; set; }
        DateTimeOffset DateAdded { get; set; }
    }
}
