namespace TextGame.Data.Contracts;

public interface IGameAccount
{
    long Id { get; }

    string Key { get; }

    string ProgressJson { get; }

    long UserAccountId { get; }

    long GameId { get; }
}

internal class GameAccountResource : IGameAccount
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public string ProgressJson { get; set; } = null!;

    public long UserAccountId { get; set; }

    public long GameId { get; set; }
}


