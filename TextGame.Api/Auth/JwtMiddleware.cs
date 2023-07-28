namespace TextGame.Api.Auth;

using System;
using System.IdentityModel.Tokens.Jwt;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.Users;

public class JwtMiddleware
{
    private readonly RequestDelegate next;

    public JwtMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, IJwtTokenValidator validator, IQueryService queryService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            var result = validator.Validate(token!);

            if (result.IsSuccess)
            {
                var userKey = result.Value.Claims.GetClaimOrThrow(JwtRegisteredClaimNames.Sub);

                context.Items["AuthTicket"] = new AuthTicket(DateTimeOffset.UtcNow, userKey);
            }
        }

        await next(context);
    }
}
