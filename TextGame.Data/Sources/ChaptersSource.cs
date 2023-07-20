using TextGame.Data.Contracts;

namespace TextGame.Data.Sources
{
    public class ChaptersSource :
        AbstractGameResourceJsonSource<Chapter[]>,
        IGameResourceJsonSource<Chapter[]>
    {
        public override string FileName { get; } = "chapters.json";
    }
}

