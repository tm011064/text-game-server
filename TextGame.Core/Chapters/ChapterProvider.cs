using LazyCache;
using TextGame.Core.Games;
using TextGame.Data;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Sources;

namespace TextGame.Core.Chapters;

public interface IChapterProvider
{
    Task<bool> Exists(string chapterKey);

    Task<IChapter> GetChapter(string chapterKey);

    Task<IReadOnlyDictionary<string, IChapter>> GetChaptersMap(IReadOnlySet<string> keys);
}

public class ChapterProvider : IChapterProvider
{
    private static readonly string CacheKey = Guid.NewGuid().ToString();

    private readonly IAppCache cache;

    private readonly IGameResourceJsonSource<IChapter[]> source;

    private readonly IGameProvider gameProvider;

    public ChapterProvider(
        IGameResourceJsonSource<IChapter[]> source,
        IGameProvider gameProvider,
        IAppCache cache)
    {
        this.cache = cache;
        this.source = source;
        this.gameProvider = gameProvider;
    }

    private async Task<IReadOnlyDictionary<string, IChapter>> GetChaptersByKey() => await cache.GetOrAddAsync(
        CacheKey,
        async () => (await gameProvider.GetMap()).Values
            .SelectMany(source.Get)
            .ToDictionary(x => x.GetCompositeKey()),
        TimeSpan.FromDays(1));

    public async Task<IChapter> GetChapter(string chapterKey) => (await GetChaptersByKey()).GetOrNotFound(chapterKey);

    public async Task<IReadOnlyDictionary<string, IChapter>> GetChaptersMap(IReadOnlySet<string> keys)
    {
        var dictionary = await GetChaptersByKey();

        return keys
            .Select(key => dictionary.TryGetValue(key, out var chapter)
                ? chapter
                : throw new ResourceNotFoundException())
            .ToDictionary(x => x.GetCompositeKey());
    }

    public async Task<bool> Exists(string chapterKey)
    {
        return (await GetChaptersByKey()).ContainsKey(chapterKey);
    }
}
