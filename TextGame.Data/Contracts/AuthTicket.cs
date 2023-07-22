namespace TextGame.Data.Contracts;

using System;

public readonly record struct AuthTicket(
    DateTimeOffset CreatedAt,
    string Identity);

