using TextGame.Data.Contracts.Games;

namespace TextGame.Data.Contracts;

public interface IUserAccount
{
    long Id { get; }

    string Key { get; }

    string Name { get; }

    long UserId { get; }
}

internal class UserAccountResource : IUserAccount
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public string Name { get; set; } = null!;

    public long UserId { get; set; }
}


