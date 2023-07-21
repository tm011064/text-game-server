namespace TextGame.Api.Auth;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{
    //https://jasonwatmore.com/net-7-csharp-jwt-authentication-tutorial-without-aspnet-core-identity
}
