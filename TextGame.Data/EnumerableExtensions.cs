namespace TextGame.Data;

public static class EnumerableExtensions
{
    public static IEnumerable<TSource> WhereNot<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
    {
        return source.Where(x => !predicate(x));
    }

    public static (IEnumerable<TSource> Success, IEnumerable<TSource> Failed) Split<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
    {
        var lookup = source.ToLookup(predicate);

        return (lookup[true], lookup[false]);
    }
}
