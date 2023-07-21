using TextGame.Data.Contracts;

namespace TextGame.Core.Chapters;

public static class ChapterExtensions
{
    public static string GetCompositeKey(this Chapter self)
    {
        return $"{self.GameKey}-{self.Key}";
    }
}

