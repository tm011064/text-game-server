using TextGame.Data.Contracts;

namespace TextGame.Data.Sources
{
    public class ChaptersSource :
        AbstractJsonSource<Chapter[]>,
        IGameContextItemJsonSource<Chapter[]>
    {
        public override string FileName { get; } = "chapters.json";
    }

}

