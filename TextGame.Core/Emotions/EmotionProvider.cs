using LazyCache;
using TextGame.Data;
using TextGame.Data.Sources;

namespace TextGame.Core.Emotions;

public interface IEmotionProvider
{
    TwoWayLookup<string, string> Get(string locale);
}

public class EmotionProvider : IEmotionProvider
{
    private static readonly string CacheKey = Guid.NewGuid().ToString();

    private readonly IAppCache cache;

    private readonly IEmotionsSource source;

    public EmotionProvider(
        IEmotionsSource source,
        IAppCache cache)
    {
        this.cache = cache;
        this.source = source;
    }

    private LocalizedContentProvider<TwoWayLookup<string, string>> GetCached() => cache.GetOrAdd(
        CacheKey,
        source.Get,
        TimeSpan.FromDays(1));

    public TwoWayLookup<string, string> Get(string locale) => GetCached().Get(locale)!;
}
