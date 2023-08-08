using System;

namespace TextGame.Data.Contracts.Games;

public interface IGame
{
    public int Id { get; }

    public string Key { get; }
}

public class GameResource : IGame
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;
}

