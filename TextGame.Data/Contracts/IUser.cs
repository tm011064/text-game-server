namespace TextGame.Data.Contracts;

public interface IUserIdentity
{
    string Key { get; }
}

public interface IUser : IUserIdentity
{
    long Id { get; }

    string Email { get; }

    IReadOnlySet<string> Roles { get; }
}