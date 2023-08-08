using TextGame.Data.Contracts.Games;

namespace TextGame.Core.Games;

public static class GameExtensions
{
    public static string GetCompositeChapterKey(this IGame self, string chapterKey)
    {
        return $"{self.Key}-{chapterKey}";
    }
}
