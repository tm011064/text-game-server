using TextGame.Data.Contracts;

namespace TextGame.Core.Rooms
{
    public interface IChapterProvider
    {
        IEnumerable<Chapter> GetChapters();
    }
}