namespace TextGame.Data.Sources;

public interface IGameResourceJsonSource<TRecord>
{
    TRecord Get(string gameKey);
}

