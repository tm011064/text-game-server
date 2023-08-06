namespace TextGame.Core.Chapters;

using TextGame.Data.Contracts.Chapters;

public interface IChapterProvider
{
    Task<IChapter> GetChapter(string chapterKey, string locale = GameSettings.DefaultLocale);
}

