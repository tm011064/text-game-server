using Microsoft.AspNetCore.Authorization;
using TextGame.Api.Auth;

using TextGame.Api.Middleware;

namespace TextGame.Api.Controllers.GameAccounts.Policies;

public record CanViewGameAccountRequirement : IAuthorizationRequirement;

public class CanViewGameAccountHandler : AuthorizationHandler<CanViewGameAccountRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CanViewGameAccountHandler(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanViewGameAccountRequirement requirement)
    {
        if (context.User is not UserPrincipal userPrincipal
            || httpContextAccessor.HttpContext == null)
        {
            return;
        }

        if (userPrincipal.UserIdentity.IsGameAdmin)
        {
            context.Succeed(requirement);
            return;
        }

        var body = await httpContextAccessor.HttpContext!.Request.ReadBody<Body>();

        if (userPrincipal.UserIdentity.Key == body.UserId
            && userPrincipal.UserIdentity.GameKeys.Contains(body.GameId ?? ""))
        {
            context.Succeed(requirement);
        }
    }

    private record Body(string? UserId, string? GameId);
}
