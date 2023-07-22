namespace TextGame.Data.Contracts;

using System;

public readonly record struct AuthTicket(
    DateTimeOffset CreatedAt,
    string Identity);

public interface IUser
{
    int Id { get; }

    string Key { get; }

    string Email { get; }
}

internal class UserModel : IUser
{
    public int Id { get; set; }

    public string Key { get; set; }

    public string Email { get; set; }
}

