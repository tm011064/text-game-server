namespace TextGame.Core.Chapters;

using TextGame.Data.Contracts;

public interface IChapterProvider
{
    Task<Chapter> GetChapter(string chapterKey, string locale = GameSettings.DefaultLocale);
}

