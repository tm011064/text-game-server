using TextGame.Data.Contracts;

namespace TextGame.Api.Auth;

public record UserIdentity(
    string Key,
    bool IsGameAdmin,
    IReadOnlySet<string> GameKeys,
    IReadOnlySet<string> GameAccountKeys) : IUserIdentity;