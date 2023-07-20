using TextGame.Data.Contracts;

namespace TextGame.Core.Emotions
{
    public interface IEmotionProvider
    {
        Task<IReadOnlyCollection<Emotion>> Get(string locale = "en-US");
    }
}