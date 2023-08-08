namespace TextGame.Data.Contracts;

public interface IUserIdentity
{
    string Key { get; }
}

public interface IUser : IUserIdentity
{
    int Id { get; }

    string Email { get; }
}