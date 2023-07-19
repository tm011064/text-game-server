using TextGame.Data.Contracts;

namespace TextGame.Data.Sources
{
    public interface IGameContextSource
    {
        GameContext Get(string gameKey, string locale);
    }

}

