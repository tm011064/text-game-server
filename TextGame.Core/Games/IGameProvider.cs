using TextGame.Data.Contracts.Games;

namespace TextGame.Core.Games;

public interface IGameProvider
{
    Task<IGame> Get(string key);
}