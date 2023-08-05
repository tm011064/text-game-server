namespace TextGame.Api.Auth;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TextGame.Data;
using TextGame.Data.Contracts;

public class JwtMiddleware
{
    private readonly RequestDelegate next;

    public JwtMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, IJwtTokenValidator validator)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!token.IsNullOrWhitespace())
        {
            var result = validator.Validate(token!);

            if (result.IsSuccess)
            {
                context.User.AddIdentity(new ClaimsIdentity(result.Value.Claims));
            }
        }

        await next(context);
    }
}
