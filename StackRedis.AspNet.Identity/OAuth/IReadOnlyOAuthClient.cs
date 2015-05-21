namespace StackRedis.AspNet.Identity.OAuth
{
    using System;

    public interface IReadOnlyOAuthClient
    {
        string Id { get; }
        string Name { get; }
        string Secret { get; }
        string SecretHash { get; }
        OAuthGrantType GrantType { get; }
        DateTimeOffset DateAdded { get; }
    }
}
