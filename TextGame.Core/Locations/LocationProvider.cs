using LazyCache;
using TextGame.Core.Games;
using TextGame.Data;
using TextGame.Data.Contracts.Games;
using TextGame.Data.Contracts.Locations;
using TextGame.Data.Sources.ResourceFiles;

namespace TextGame.Core.Locations;

public static class LocationExtensions
{
    public static string GetCompositeKey(this ILocation self)
    {
        return $"{self.Game.Key}-{self.Key}";
    }
}

public interface ILocationProvider
{
    Task<ILocation> GetLocation(string key);

    Task<IReadOnlyDictionary<string, ILocation>> GetMap();
}

public class LocationProvider : ILocationProvider
{
    private static readonly string CacheKey = Guid.NewGuid().ToString();

    private readonly IAppCache cache;

    private readonly ILocationsSource source;

    private readonly IGameProvider gameProvider;

    public LocationProvider(
        ILocationsSource source,
        IGameProvider gameProvider,
        IAppCache cache)
    {
        this.cache = cache;
        this.source = source;
        this.gameProvider = gameProvider;
    }

    private async Task<IReadOnlyDictionary<string, ILocation>> GetLocationsByKey() => await cache.GetOrAddAsync(
        CacheKey,
        async () => (await gameProvider.GetMap()).Values
            .SelectMany(source.Get)
            .ToDictionary(x => x.GetCompositeKey()),
        TimeSpan.FromDays(1));

    public async Task<ILocation> GetLocation(string chapterKey) => (await GetLocationsByKey()).GetOrNotFound(chapterKey);

    public Task<IReadOnlyDictionary<string, ILocation>> GetMap() => GetLocationsByKey();
}
