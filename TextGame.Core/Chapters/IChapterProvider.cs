using TextGame.Data.Contracts.Chapters;

namespace TextGame.Core.Chapters;

public interface IChapterProvider
{
    Task<IChapter> GetChapter(string chapterKey, string locale = GameSettings.DefaultLocale);
}

