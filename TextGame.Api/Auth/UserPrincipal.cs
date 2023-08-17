using System.Security.Principal;
using TextGame.Data.Contracts;

namespace TextGame.Api.Auth;

public class UserPrincipal : GenericPrincipal
{
    public UserPrincipal(IIdentity identity, UserIdentity userIdentity)
        : base(identity, Array.Empty<string>())
    {
        UserIdentity = userIdentity;
    }

    public UserIdentity UserIdentity { get; }
}