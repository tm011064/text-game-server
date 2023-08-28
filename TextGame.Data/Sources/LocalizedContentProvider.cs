namespace TextGame.Data.Sources;

public class LocalizedContentProvider<TValue>
{
    public static readonly LocalizedContentProvider<TValue> Empty = new(null);

    private readonly IReadOnlyDictionary<string, TValue>? map;

    public LocalizedContentProvider(IReadOnlyDictionary<string, TValue>? map)
    {
        this.map = map;
    }

    public TValue? First()
    {
        return map == null
            ? default
            : map!.First().Value;
    }

    public TValue? Get(string locale)
    {
        return map == null
            ? default
            : map!.GetOrNotFound(locale);
    }

    public TValue Get(string locale, TValue defaultValue)
    {
        return map == null
            ? defaultValue
            : map!.GetOrNotFound(locale);
    }
}