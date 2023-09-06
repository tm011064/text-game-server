namespace TextGame.Data.Contracts.Games;

public interface IGame
{
    public long Id { get; }

    public string Key { get; }
}

internal class GameResource : IGame
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;
}

public static class GameExtensions
{
    public static string GetCompositeKey(this IGame self, string key)
    {
        return $"{self.Key}-{key}";
    }
}
