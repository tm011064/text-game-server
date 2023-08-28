using TextGame.Data.Sources.ResourceFiles;

namespace TextGame.Data.Sources;

public interface IGlobalResourceJsonSource<TKey, TValue>
    where TValue : notnull
    where TKey : notnull
{
    LocalizedContentProvider<TwoWayLookup<TKey, TValue>> Get();
}