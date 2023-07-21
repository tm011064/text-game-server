namespace TextGame.Core.Emotions;

using LazyCache;
using TextGame.Data.Contracts;
using TextGame.Data.Sources;

public class EmotionProvider : IEmotionProvider
{
    private static readonly string CachePrefix = Guid.NewGuid().ToString();

    private readonly IAppCache cache;

    private readonly IGlobalResourceJsonSource<Emotion[]> source;

    public EmotionProvider(
        IGlobalResourceJsonSource<Emotion[]> source,
        IAppCache cache)
    {
        this.cache = cache;
        this.source = source;
    }

    private IReadOnlyCollection<Emotion> GetCached(string locale) => cache.GetOrAdd(
        $"{CachePrefix}-{locale}",
        () => source.Get(locale),
        TimeSpan.FromDays(1));

    public Task<IReadOnlyCollection<Emotion>> Get(string locale) => Task.FromResult(GetCached(locale));
}

