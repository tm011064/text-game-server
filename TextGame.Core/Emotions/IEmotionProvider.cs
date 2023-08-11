using TextGame.Data.Contracts.Emotions;

namespace TextGame.Core.Emotions;

public interface IEmotionProvider
{
    Task<IReadOnlyCollection<Emotion>> Get(string locale = GameSettings.DefaultLocale);
}