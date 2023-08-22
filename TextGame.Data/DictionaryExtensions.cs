namespace TextGame.Data;

public static class DictionaryExtensions
{
    public static TValue GetOrNotFound<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> self,
        TKey key)
    {
        return self.TryGetValue(key, out var value)
            ? value
            : throw new ResourceNotFoundException($"Resource with key {key} does not exist");
    }
}