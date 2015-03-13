<img src="http://mfidemraizer.github.io/stackredis.aspnet.identity/images/stackredis.aspnet.identity.png">

# Welcome to StackRedis.AspNet.Identity!

## INDEX

1. [Introduction](#sr-aspnet-identity-i)
2. [Highlights and features](#sr-aspnet-identity-ii)
3. [How to install it](#sr-aspnet-identity-iii)
4. [How to integrate it in both ASP.NET MVC 5 and ASP.NET Web API 2.2 template](#sr-aspnet-identity-iv)
5. [Why I need to provide my own `ConnectionMultiplexer` to StackRedis.AspNet.Identity? Even better... What's a `ConnectionMultiplexer`?](#sr-aspnet-identity-v)
6. [Configuration](#sr-aspnet-identity-vi)
7. [Architecture](#sr-aspnet-identity-vii)
8. [Who's behind this project?](#sr-aspnet-identity-viii)


<span id="sr-aspnet-identity-i"></span>
## I. Introduction

This is an open source ASP.NET Identity 2.x-compliant custom user store implementation to *Redis* using the fancy [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) library.

**Contributions are absolutely welcomed and they're valuable!**. Create your own fork and do a *pull request* to merge your enhancements, fixes or new features!

Also, if you find any problem while using this library, please fill a new issue here in GitHub's repository and it'll be addressed as soon as possible.

Finally, if you want to contact the author directly, you can do it on LinkedIn - add *Matías Fidemraizer* to your contacts -.

<span id="sr-aspnet-identity-ii"></span>
## II. Higlights and features

- It's built on top of free and open source [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) library empowering StackExchange *Redis* connectivity, made by Marc Gravell.
- It takes advantage of *Redis* data structures like sets and hashes to ASP.NET Identity user and login data using *Redis*-specific operations.
- As it uses *Redis* as primary data store, ASP.NET Identitity meets the **lightspeed**.
- It seamlessly-integrates with both ASP.NET MVC 5 MVC and WebAPI templates.

<span id="sr-aspnet-identity-iii"></span>
## III. How to install it

*StackRedis.AspNet.Identity* is delivered as a *NuGet* package. 

- Install it using Visual Studio's Package Manager Console: `Install-Package StackRedis.AspNet.Identity`.
- Install it using Visual Studio's GUI (go to *Manage NuGet Packages* in your Visual Studio solution).


<span id="sr-aspnet-identity-iv"></span>
## IV. How to integrate it in both ASP.NET MVC 5 and ASP.NET Web API 2.2 template

If you create a new ASP.NET MVC 5 project and you choose either to create a Web site or a Web API, and you select *"Individual user accounts"* as authentication method, integrating *StackRedis.AspNet.Identity* becomes extremely easy:

1. Install *StackRedis.AspNet.Identity* NuGet package in your ASP.NET project.
2. Create a new ASP.NET MVC 5 project (either a Web site or Web API).
3. Once project has been already generated, go to Visual Studio's *Solution Explorer* and locate and open `App_Start\IdentityConfig.cs` file.
4. Locate the following code line in generated `ApplicationUserManager` class:

    var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
5. Replace `new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));` with `new RedisUserStore<ApplicationUser>()`.
6. Now there's a compilation error, as `RedisUserStore` has a constructor which requires a `ConnectionMultiplexer` instance as argument. You should read how to provide it [here](#sr-aspnet-identity-v).
7. Go to `Models\IdentityModels.cs`, open it and locate `ApplicationUser` class.
8. `ApplicationUser` needs to implement `StackRedis.AspNet.Identity.IIdentityUser` interface to work with *StackRedis.AspNet.Identity*. In the other hand, this library provides a class which can be used as base class `StackRedis.AspNet.Identity.IdentityUser` which both implements `IIdentityUser` and `IUser` from ASP.NET Identity.
9. Finally, there are some application settings that should be provided in application's configuration file (i.e. `Web.config`, `App.config`...). See [VI. Configuration](#sr-aspnet-identity-vi).

<span id="sr-aspnet-identity-v"></span>
## V. Why I need to provide my own `ConnectionMultiplexer` to StackRedis.AspNet.Identity? Even better... What's a `ConnectionMultiplexer`?

**To the first question** (*Why I need to provide my own `ConnectionMultiplexer` to StackRedis.AspNet.Identity?*), the answer is *because **StackExchange.Redis** library uses an aggressive Redis connectivity approach and only one `ConnectionMultiplexer` should exist during an application life-cycle*.

Thus, *StackRedis.AspNet.Identity* doesn't create an own `ConnectionMultiplexer` and enforces developers to share the same one as the entire application. In other words: this is what you need to provide during **step 6** of [*IV. How to integrate [...]*](#sr-aspnet-identity-iv) part of current document.

Learn more about *why* to use `ConnectionMultiplexer` this way and actually what's this class and how to configure a connection to Redis [here (StackExchange.Redis - Basic usage)](https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Basics.md).

<span id="sr-aspnet-identity-vi"></span>
## VI. Configuration

*StackRedis.AspNet.Redis* requires some `<appSettings>` to be provided in your `App.config` or `Web.config`. There're no default values to any of required settings excluding `aspNet:identity:redis:db`:

- Provides the Redis database number. Example: `<add key="aspNet:identity:redis:db" value="3"/>`.
- Provides the Redis key name to key-value string that stores the last auto-generated integer user identifier assigned to a registered user. Example: `<add key="aspNet:identity:redis:userIdKey" value="stackredis:identity:user:id"/>`.
- Provides the Redis hash key name to store users' data by their identifier. Example: `<add key="aspNet:identity:redis:userHashByIdKey" value="stackredis:identity:users:byid"/>`.
- Provides the Redis hash key name to store users' identifiers by their name. Example: `<add key="aspNet:identity:redis:userHashByNameKey" value="stackredis:identity:users:byname"/>`.
- Provides the Redis hash key name to store users' logins. Example: `<add key="aspNet:identity:redis:userLoginHashKey" value="stackredis:identity:users:logins"/>`.
- Provides the Redis hash key name to store users' login fail counts by their identifier. Example `<add key="aspNet:identity:redis:userLoginFailCountHashKey" value="stackredis:identity:logins:fails"/>`.
- Provides the Redis set key name to store locked users' identifiers. Example: `<add key="aspNet:identity:redis:userLockSetKey" value="stackredis:identity:users:logins:locks"/>`.
- Provides the Redis hash key name to store when locked users were locked out last time. Example: `<add key="aspNet:identity:redis:userLockDateHashKey" value="stackredis:identity:users:logins:locks:dates"/>`.
- Provides the Redis set key name to store users' confirmed email addresses. Example: `<add key="aspNet:identity:redis:userConfirmedEmailSetKey" value="stackredis:identity:users:confirmedmails"/>`.
- Provides the Redis set key name to store users' confirmed phone numbers. Example: `<add key="aspNet:identity:redis:userConfirmedPhoneNumberSetKey" value="stackredis:identity:users:confirmedphonenumbers"/>`.
- Provides the Redis set key name to store each user's assigned security roles. `{0}` placeholder gets some user identifier. Example: `<add key="aspNet:identity:redis:userRoleSetKey" value="stackredis:identity:users:{0}:roles"/>`.
- Provides the Redis set key name to store each user's assigned logins. `{0}` placeholder gets some user identifier. Example: `<add key="aspNet:identity:redis:userLoginSetKey" value="stackredis:identity:users:{0}:logins"/>`.
- Provides the Redis set key name to store each user's assigned security claims. `{0}` placeholder gets some user identifier. Example: `<add key="aspNet:identity:redis:userClaimSetKey" value="stackredis:identity:users:{0}:claims"/>`.
- Provides the Redis hash key name to store if an user has two-factor authentication enabled. Example: `<add key="aspNet:identity:redis:twoFactorEnabledHashKey" value="stackredis:identity:users:twofactorauthenabled"/>`.

Full configuration sample:

	<appSettings>
      <add key="aspNet:identity:redis:db" value="3"/>
      <add key="aspNet:identity:redis:userIdKey" value="stackredis:identity:user:id"/>
      <add key="aspNet:identity:redis:userHashByIdKey" value="stackredis:identity:users:byid"/>
      <add key="aspNet:identity:redis:userHashByNameKey" value="stackredis:identity:users:byname"/>
      <add key="aspNet:identity:redis:userLoginHashKey" value="stackredis:identity:users:logins"/>
      <add key="aspNet:identity:redis:userLoginFailCountHashKey" value="stackredis:identity:logins:fails"/>
      <add key="aspNet:identity:redis:userLockSetKey" value="stackredis:identity:users:logins:locks"/>
      <add key="aspNet:identity:redis:userLockDateHashKey" value="stackredis:identity:users:logins:locks:dates"/>
      <add key="aspNet:identity:redis:userConfirmedEmailSetKey" value="stackredis:identity:users:confirmedmails"/>
      <add key="aspNet:identity:redis:userConfirmedPhoneNumberSetKey" value="stackredis:identity:users:confirmedphonenumbers"/>
      <add key="aspNet:identity:redis:userRoleSetKey" value="stackredis:identity:users:{0}:roles"/>
      <add key="aspNet:identity:redis:userLoginSetKey" value="stackredis:identity:users:{0}:logins"/>
      <add key="aspNet:identity:redis:userClaimSetKey" value="stackredis:identity:users:{0}:claims"/>
      <add key="aspNet:identity:redis:twoFactorEnabledHashKey" value="stackredis:identity:users:twofactorauthenabled"/>
	</appSettings>

<span id="sr-aspnet-identity-vii"></span>
## VII. Architecture

*StackRedis.AspNet.Identity* stores data using Redis-specific data structures.

### How user data is stored?

User data (actually an `IUser` implementation) is stored in a Redis hash (basically, it's like a .NET dictionary).

The key receives the name defined as value of `aspNet:identity:redis:userHashByIdKey` application setting. 

`aspNet:identity:redis:userHashByIdKey` sub-keys are users' identifiers and their values, the users' data.

Also, because ASP.NET Identity requires obtaining users by their name, *StackRedis.AspNet.Identity* provides another hash with the key name provided by `aspNet:identity:redis:userHashByNameKey`, which stores sub-keys where the keys are user names and values the corresponding user's id. That is, when ASP.NET Identity requests an user by name, *StackRedis.AspNet.Identity* implementation first looks what user identifier is associated for a given user name, and then retrieves the so-called user's data using the `aspNet:identity:redis:userHashByIdKey` hash retrieving it by the user's identifier.

In addition, when ASP.NET Identity creates an user, as it requires an incremented user identifier, *StackRedis.Aspnet.Identity* uses the key name defined by the application setting `aspNet:identity:redis:db` to increment its value by 1 and return the resulting incremented number as the new user's identifier.

### How user logins and related data are stored?

#### Logins
Instead of storing users' logins in a `IUser` implementation property (for example a `List<T>` of logins), *StackRedis.AspNet.Redis* stores them in a Redis hash (key name defined by `aspNet:identity:redis:userLoginHashKey` where the key on this hash is a concatenation of  `Microsoft.AspNet.Identity.UserLoginInfo`'s `LoginProvider` and `ProviderKey` properties separated by a colon (`:`), and the value is the user identifier of the user to be associated to the so-called login.

Also, logins for some given user are stored in a Redis set where its key name is defined by the `aspNet:identity:redis:userLoginSetKey`.

Why a hash and a set? Good question.

- A hash, because when ASP.NET Identity needs to authenticate an user by its login information, it will use the hash, as the hash keys are the concatenation of that information and it's easy to locate an user identifier this way.

- A set, because ASP.NET Identity needs to get all logins and their information by user identifier.

#### Login fail count
User login count is also stored in a Redis hash with the key name defined by `userLoginFailCountHashKey`, where the keys are user identifiers and values the login fail count.

#### Locked users

Locked users are stored in a Redis set. This set stores user identifiers.

Thus, when ASP.NET Identity needs to check if an user is locked, *StackRedis.AspNet.Identity* checks if user's identifier is member of this set.

The key name is defined by `aspNet:identity:redis:userLockSetKey` application setting.

#### When users were locked

Locked date for locked users is stored in a Redis hash, where keys are user identifiers and values the locked date in a intenger in Windows file format.

The key name is defined by `aspNet:identity:redis:userLockDateHashKey` application setting.

#### User roles and claims

User roles and claims are stored in separate Redis sets. Each user has its own set of roles and claims. 

Sets' key names are defined by `aspNet:identity:redis:userRoleSetKey` and `aspNet:identity:redis:userClaimSetKey` application settings.

#### User email and phone number confirmations

Users' email addresses and phone numbers are confirmed by adding them to two Redis sets: one for email addresses and other for phone numbers.

When ASP.NET Identity needs to check if user's email or phone number are confirmed, *StackRedis.AspNet.Identity* just checks if the whole email or phone number are members of these Redis sets.

Sets' key names are defined by `aspNet:identity:redis:userConfirmedEmailSetKey` and `aspNet:identity:redis:userConfirmedPhoneNumberSetKey` application settings.

#### User has enabled two-factor authentication

This user setting is stored in a Redis set. This set contains the user identifiers of users that have enabled two-factor authentication.

When ASP.NET Identity requests if this setting is enabled, *StackRedis.AspNet.Identity* checks if user's identifier is a member of the so-called set.

Set key name is defined by `aspNet:identity:redis:twoFactorEnabledSetKey` application setting.

<span id="sr-aspnet-identity-viii"></span>
## VIII. Who's behind this project?

Project itself is entirely maintained by [Matías Fidemraizer (follow this link to contact me on LinkedIn)](https://linkedin.com/in/mfidemraizer).

Also, I'm a big fan of [StackOverflow](http://stackoverflow.com) and I'll enjoy answering any question about this library!

<a href="http://stackoverflow.com/users/411632/mat%c3%adas-fidemraizer">
<img src="http://stackoverflow.com/users/flair/411632.png?theme=clean" width="208" height="58" alt="profile for Mat&#237;as Fidemraizer at Stack Overflow, Q&amp;A for professional and enthusiast programmers" title="profile for Mat&#237;as Fidemraizer at Stack Overflow, Q&amp;A for professional and enthusiast programmers">
</a>
