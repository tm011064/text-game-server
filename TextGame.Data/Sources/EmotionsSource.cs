using TextGame.Data.Contracts.Emotions;

namespace TextGame.Data.Sources
{
    public class EmotionsSource :
        AbstractGlobalResourceJsonSource<Emotion[]>,
        IGlobalResourceJsonSource<Emotion[]>
    {
        public override string FileName { get; } = "emotions.json";
    }
}

