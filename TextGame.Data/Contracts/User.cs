namespace TextGame.Data.Contracts;

using System;

public readonly record struct User(
    int Id,
    Guid Key,
    string Email);

public readonly record struct AuthTicket(
    DateTimeOffset CreatedAt,
    string Identity);

