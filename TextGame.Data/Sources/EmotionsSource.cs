using TextGame.Data.Contracts;

namespace TextGame.Data.Sources
{
    public class EmotionsSource :
        AbstractJsonSource<Emotion[]>,
        IGameContextItemJsonSource<Emotion[]>
    {
        public override string FileName { get; } = "emotions.json";
    }

}

