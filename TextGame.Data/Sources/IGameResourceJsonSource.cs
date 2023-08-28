using TextGame.Data.Contracts.Games;

namespace TextGame.Data.Sources;

public interface IGameResourceJsonSource<TRecord>
{
    TRecord Get(IGame game);
}

