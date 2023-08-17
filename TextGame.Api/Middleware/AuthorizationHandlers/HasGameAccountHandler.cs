using Microsoft.AspNetCore.Authorization;
using TextGame.Api.Auth;

namespace TextGame.Api.Middleware.AuthorizationHandlers;

public record HasGameAccountRequirement : IAuthorizationRequirement;

public class HasGameAccountHandler : AuthorizationHandler<HasGameAccountRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public HasGameAccountHandler(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HasGameAccountRequirement requirement)
    {
        if (context.User is not UserPrincipal userPrincipal
            || httpContextAccessor.HttpContext == null)
        {
            return;
        }

        var body = await httpContextAccessor.HttpContext!.Request.ReadBody<Body>();

        if (userPrincipal.UserIdentity.GameAccountKeys.Contains(body.GameAccountId ?? ""))
        {
            context.Succeed(requirement);
        }
    }

    private record Body(string? GameAccountId);
}