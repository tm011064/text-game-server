using LazyCache;
using TextGame.Core.Games;
using TextGame.Core.Locations;
using TextGame.Data;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Sources.ResourceFiles;

namespace TextGame.Core.Chapters;

public static class ChapterExtensions
{
    public static string GetCompositeKey(this IChapter self)
    {
        return $"{self.Game.Key}-{self.Key}";
    }
}

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

    private readonly IChaptersSource source;

    private readonly IGameProvider gameProvider;

    private readonly ILocationProvider locationProvider;

    public ChapterProvider(
        IChaptersSource source,
        IGameProvider gameProvider,
        ILocationProvider locationProvider,
        IAppCache cache)
    {
        this.cache = cache;
        this.source = source;
        this.gameProvider = gameProvider;
        this.locationProvider = locationProvider;
    }

    private async Task<IReadOnlyDictionary<string, IChapter>> GetChaptersByKey()
    {
        var locationMap = await locationProvider.GetMap();

        return await cache.GetOrAddAsync(
            CacheKey,
            async () => (await gameProvider.GetMap()).Values
                .SelectMany(game => source.Get(game, locationMap))
                .ToDictionary(x => x.GetCompositeKey()),
            TimeSpan.FromDays(1));
    }

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
