using TextGame.Data.Contracts;

namespace TextGame.Core.Users;

public record SystemUserIdentity : IUserIdentity
{
    public static readonly SystemUserIdentity Instance = new();

    public string Key { get; init; } = "system";
}
