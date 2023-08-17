using TextGame.Core.Users;

namespace TextGame.Data.Contracts;

public readonly record struct AuthTicket(
    DateTimeOffset CreatedAt,
    string CreatedBy)
{
    public static AuthTicket System => new(DateTimeOffset.UtcNow, SystemUserIdentity.Instance.Key);
}

