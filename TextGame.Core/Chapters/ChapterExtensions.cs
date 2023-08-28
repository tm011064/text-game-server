using TextGame.Data.Contracts.Chapters;

namespace TextGame.Core.Chapters;

public static class ChapterExtensions
{
    public static string GetCompositeKey(this IChapter self)
    {
        return $"{self.Game.Key}-{self.Key}";
    }
}

