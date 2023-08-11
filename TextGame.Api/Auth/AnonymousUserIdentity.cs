using TextGame.Data.Contracts;

namespace TextGame.Api.Auth;

public record AnonymousUserIdentity : IUserIdentity
{
    public static readonly AnonymousUserIdentity Instance = new();

    public string Key { get; init; } = Guid.Empty.ToString();
}
