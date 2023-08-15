namespace TextGame.Data.Contracts;

public interface IGameAccount
{
    long Id { get; }

    string Key { get; }

    string GameStateJson { get; }

    long UserAccountId { get; }

    long GameId { get; }

    long Version { get; }
}

internal class GameAccountResource : IGameAccount
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public string GameStateJson { get; set; } = null!;

    public long UserAccountId { get; set; }

    public long GameId { get; set; }

    public long Version { get; set; }
}


