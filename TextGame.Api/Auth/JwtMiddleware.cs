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

        if (token == null)
        {
            await next(context);
            return;
        }

        var result = validator.Validate(token!);

        if (result.IsSuccess)
        {
            var userId = int.Parse(result.Value.Claims.First(x => x.Type == "id").Value);

            var user = await queryService.Run(GetUser.ById(userId)); // TODO (Roman): this doesn't need to be done at each request
            context.Items["User"] = user;

            context.Items["AuthTicket"] = new AuthTicket(DateTimeOffset.UtcNow, user.Key);
        }

        await next(context);
    }
}
