namespace TextGame.Data;

public class TwoWayLookup
{
    public static TwoWayLookup<TKey, TValue> Create<TRecord, TKey, TValue>(
        IEnumerable<TRecord> records,
        Func<TRecord, TKey> selectKey,
        Func<TRecord, IEnumerable<TValue>> selectValues,
        IEqualityComparer<TValue>? valueComparer = null,
        IEqualityComparer<TKey>? keyComparer = null)
        where TValue : notnull
        where TKey : notnull
    {
        var keyValuePairs = records
            .SelectMany(record => selectValues(record)
                .Select(value => new { Key = selectKey(record), Value = value }));

        var valuesByKey = keyValuePairs.ToLookup(x => x.Key, x => x.Value, keyComparer);
        var keysByValue = keyValuePairs.ToDictionary(x => x.Value, x => x.Key, valueComparer);

        return new TwoWayLookup<TKey, TValue>(valuesByKey, keysByValue);
    }
}

public class TwoWayLookup<TKey, TValue>
    where TValue : notnull
    where TKey : notnull
{
    public ILookup<TKey, TValue> valuesByKey;

    private readonly IReadOnlyDictionary<TValue, TKey> keysByValue;

    public TwoWayLookup(ILookup<TKey, TValue> valuesByKey, IReadOnlyDictionary<TValue, TKey> keysByValue)
    {
        this.valuesByKey = valuesByKey;
        this.keysByValue = keysByValue;
    }

    public bool HasKey(TKey key) => valuesByKey.Contains(key);

    public IEnumerable<TValue> GetValuesByKey(TKey key) => valuesByKey[key];

    public IEnumerable<TValue> GetValues() => keysByValue.Keys;

    public bool TryGetValues(TKey key, out IEnumerable<TValue> values)
    {
        values = valuesByKey[key];
        return valuesByKey.Contains(key);
    }

    public IEnumerable<TKey> GetKeys() => valuesByKey.Select(x => x.Key);

    public ILookup<TKey, TValue> GetAll() => valuesByKey;

    public bool TryGetKey(TValue value, out TKey? key) => keysByValue.TryGetValue(value, out key);
}
