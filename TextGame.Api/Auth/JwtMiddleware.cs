namespace TextGame.Api.Auth;

using System;
using TextGame.Api.Controllers.Users;
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

        var userId = validator.Validate(token);

        if (userId != null)
        {
            var user = await queryService.Run(new GetUserById(userId.Value));
            context.Items["User"] = user;
            context.Items["AuthTicket"] = new AuthTicket(DateTimeOffset.UtcNow, user.Key);
        }

        await next(context);
    }
}
