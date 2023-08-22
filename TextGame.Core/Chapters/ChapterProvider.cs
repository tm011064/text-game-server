using LazyCache;
using TextGame.Data;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Sources;

namespace TextGame.Core.Chapters;

public interface IChapterProvider
{
    bool Exists(string chapterKey);

    Task<IChapter> GetChapter(string chapterKey);

    IReadOnlyDictionary<string, IChapter> GetChaptersMap(IReadOnlySet<string> keys);
}

public class ChapterProvider : IChapterProvider
{
    private static readonly string CacheKey = Guid.NewGuid().ToString();

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

    private IReadOnlyDictionary<string, IChapter> GetChaptersByKey() => cache.GetOrAdd(
        CacheKey,
        () => gameSource.GetKeys()
            .SelectMany(source.Get)
            .ToDictionary(x => x.GetCompositeKey()),
        TimeSpan.FromDays(1));

    public Task<IChapter> GetChapter(string chapterKey) => Task.FromResult(
        GetChaptersByKey().GetOrNotFound(chapterKey));

    public IReadOnlyDictionary<string, IChapter> GetChaptersMap(IReadOnlySet<string> keys)
    {
        var dictionary = GetChaptersByKey();

        return keys
            .Select(key => dictionary.TryGetValue(key, out var chapter)
                ? chapter
                : throw new ResourceNotFoundException())
            .ToDictionary(x => x.GetCompositeKey());
    }

    public bool Exists(string chapterKey)
    {
        return GetChaptersByKey().ContainsKey(chapterKey);
    }
}
