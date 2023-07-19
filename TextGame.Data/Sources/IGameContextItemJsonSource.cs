namespace TextGame.Data.Sources
{
    public interface IGameContextItemJsonSource<TRecord>
    {
        TRecord Get(string gameKey, string locale);
    }

}

