namespace TextGame.Data.Sources;

public interface IGlobalResourceJsonSource<TRecord>
{
    TRecord Get(string locale);
}

