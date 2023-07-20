using TextGame.Data.Contracts;

namespace TextGame.Core.Chapters
{
    public interface IChapterProvider
    {
        Task<Chapter> GetChapter(string chapterKey, string locale = "en-US");
    }
}

