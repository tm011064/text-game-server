using Microsoft.AspNetCore.Authorization;
using TextGame.Api.Auth;

namespace TextGame.Api.Middleware.AuthorizationHandlers;

public record CanManageUsersRequirement : IAuthorizationRequirement;

public class CanManageUsersHandler : AuthorizationHandler<CanManageUsersRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanManageUsersRequirement requirement)
    {
        if (context.User is UserPrincipal userPrincipal
            && userPrincipal.UserIdentity.IsGameAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
