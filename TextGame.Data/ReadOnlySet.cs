namespace TextGame.Data;

public static class ReadOnlySet
{
    public static IReadOnlySet<TValue> Empty<TValue>() => EmptyRecord<TValue>.Value;

    private static class EmptyRecord<TValue>
    {
        public static readonly HashSet<TValue> Value = new(0);
    }
}