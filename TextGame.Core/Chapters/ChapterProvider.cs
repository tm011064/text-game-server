using LazyCache;
using TextGame.Data;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Sources;

namespace TextGame.Core.Chapters;

public class ChapterProvider : IChapterProvider
{
    private static readonly string CachePrefix = Guid.NewGuid().ToString();

    private readonly IAppCache cache;

    private readonly IGameResourceJsonSource<IChapter[]> source;

    private readonly IGameSource gameSource;

    public ChapterProvider(
        IGameResourceJsonSource<IChapter[]> source,
        IGameSource gameSource,
        IAppCache cache)
    {
        this.cache = cache;
        this.source = source;
        this.gameSource = gameSource;
    }

    private IReadOnlyDictionary<string, IChapter> GetChaptersByKey(string locale) => cache.GetOrAdd(
        $"{CachePrefix}-{locale}",
        () => gameSource.GetKeys()
            .SelectMany(x => source.Get(x, locale))
            .ToDictionary(x => x.GetCompositeKey()),
        TimeSpan.FromDays(1));

    public Task<IChapter> GetChapter(string chapterKey, string locale) => Task.FromResult(
        GetChaptersByKey(locale).TryGetValue(chapterKey, out var chapter)
            ? chapter
            : throw new ResourceNotFoundException());
}
