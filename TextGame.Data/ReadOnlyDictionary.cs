namespace TextGame.Data;

public static class ReadOnlyDictionary
{
    public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() where TKey : notnull => EmptyRecord<TKey, TValue>.Value;

    private static class EmptyRecord<TKey, TValue> where TKey : notnull
    {
        public static readonly Dictionary<TKey, TValue> Value = new(0);
    }
}
