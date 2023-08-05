namespace TextGame.Data.Contracts;

using System;

public readonly record struct AuthTicket(
    DateTimeOffset CreatedAt,
    IUserIdentity User)
{
    public string CreatedBy { get; } = User.Key;
}

