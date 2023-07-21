namespace TextGame.Core.Emotions;

using TextGame.Data.Contracts;

public interface IEmotionProvider
{
    Task<IReadOnlyCollection<Emotion>> Get(string locale = "en-US");
}