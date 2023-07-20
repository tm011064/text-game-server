using TextGame.Data.Contracts;

namespace TextGame.Data.Sources
{
    public class EmotionsSource :
        AbstractGlobalResourceJsonSource<Emotion[]>,
        IGlobalResourceJsonSource<Emotion[]>
    {
        public override string FileName { get; } = "emotions.json";
    }
}

