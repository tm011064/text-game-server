namespace TextGame.Data;

public static class EnumerableExtensions
{
    public static IEnumerable<TSource> WhereNot<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate)
    {
        return source.Where(x => !predicate(x));
    }
}