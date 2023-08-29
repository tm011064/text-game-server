namespace TextGame.Data.Sources;

public class LocalizedContentProvider<TValue>
{
    public static readonly LocalizedContentProvider<TValue> Empty = new(null);

    private readonly IReadOnlyDictionary<string, TValue>? map;

    public LocalizedContentProvider(IReadOnlyDictionary<string, TValue>? map)
    {
        this.map = map;
    }

    public bool IsEmpty()
    {
        return map == null;
    }

    public TValue? First()
    {
        return IsEmpty()
            ? default
            : map!.First().Value;
    }

    public TValue? Get(string locale)
    {
        return IsEmpty()
            ? default
            : map!.GetOrNotFound(locale);
    }

    public TValue Get(string locale, TValue defaultValue)
    {
        return IsEmpty()
            ? defaultValue
            : map!.GetOrNotFound(locale);
    }
}