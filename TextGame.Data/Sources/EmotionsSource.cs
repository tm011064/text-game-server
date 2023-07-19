using TextGame.Data.Contracts;

namespace TextGame.Data.Sources
{
    public class EmotionsSource :
        AbstractGameContextItemJsonSource<Emotion[]>,
        IGameContextItemJsonSource<Emotion[]>
    {
        public override string FileName { get; } = "emotions.json";
    }
}

