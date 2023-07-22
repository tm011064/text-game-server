namespace TextGame.Data.Contracts;

public interface IUser
{
    int Id { get; }

    string Key { get; }

    string Email { get; }
}

