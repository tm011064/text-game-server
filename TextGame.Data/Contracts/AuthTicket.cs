namespace TextGame.Data.Contracts;

using System;
using TextGame.Core.Users;

public readonly record struct AuthTicket(
    DateTimeOffset CreatedAt,
    IUserIdentity User)
{
    public static AuthTicket System => new(DateTimeOffset.UtcNow, SystemUserIdentity.Instance);

    public string CreatedBy { get; } = User.Key;
}

