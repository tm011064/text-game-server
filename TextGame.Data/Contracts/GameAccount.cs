﻿namespace TextGame.Data.Contracts;

public interface IGameAccount
{
    long Id { get; }

    string Key { get; }

    string GameStateJson { get; }

    long UserAccountId { get; }

    string UserAccountKey { get; }

    long UserId { get; }

    string UserKey { get; }

    long GameId { get; }

    string GameKey { get; }

    long Version { get; }
}

internal class GameAccountResource : IGameAccount
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public string GameStateJson { get; set; } = null!;

    public long UserAccountId { get; set; }

    public string UserAccountKey { get; set; } = null!;

    public long GameId { get; set; }

    public string GameKey { get; set; } = null!;

    public long UserId { get; set; }

    public string UserKey { get; set; } = null!;

    public long Version { get; set; }
}


