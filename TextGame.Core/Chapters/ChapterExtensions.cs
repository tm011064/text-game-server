namespace TextGame.Core.Chapters;

using TextGame.Data.Contracts;

public static class ChapterExtensions
{
    public static string GetCompositeKey(this Chapter self)
    {
        return $"{self.GameKey}-{self.Key}";
    }
}

