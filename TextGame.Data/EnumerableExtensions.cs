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

    public static IReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(
        this IEnumerable<TSource> source)
    {
        return source.ToArray();
    }

    public static IEnumerable<TRecord> LeftMerge<TRecord, TKey>(
        this IEnumerable<TRecord> source,
        IEnumerable<TRecord> overrides,
        Func<TRecord, TKey> selectKey)
    {
        return source
            .GroupJoin(
                overrides,
                selectKey,
                selectKey,
                (x, y) => new { Record = x, Overrides = y })
            .SelectMany(
                x => x.Overrides.DefaultIfEmpty(),
                (x, y) => y ?? x.Record);
    }
}
