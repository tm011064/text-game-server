using LazyCache;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Games;
using TextGame.Data.Queries.Games;

namespace TextGame.Core.Games;

public interface IGameProvider
{
    Task<IGame> Get(string key);

    Task<IGame> GetById(long id);

    Task<IReadOnlyDictionary<long, IGame>> GetMap();
}

public class GameProvider : IGameProvider
{
    private static readonly string CachePrefix = Guid.NewGuid().ToString();

    private readonly IQueryService queryService;

    private readonly IAppCache cache;

    public GameProvider(IQueryService queryService, IAppCache cache)
    {
        this.queryService = queryService;
        this.cache = cache;
    }

    private async Task<GameCache> GetGameCache() => await cache.GetOrAdd(
        CachePrefix,
        async () =>
        {
            var records = await queryService.Run(new SearchGames(), AuthTicket.System);
            return new GameCache(
                records.ToDictionary(x => x.Id),
                records.ToDictionary(x => x.Key));
        },
        TimeSpan.FromHours(1));

    public async Task<IGame> Get(string key) => (await GetGameCache()).GamesByKey.GetOrNotFound(key);

    public async Task<IGame> GetById(long id) => (await GetGameCache()).GamesById.GetOrNotFound(id);

    public async Task<IReadOnlyDictionary<long, IGame>> GetMap() => (await GetGameCache()).GamesById;

    private record GameCache(
        IReadOnlyDictionary<long, IGame> GamesById,
        IReadOnlyDictionary<string, IGame> GamesByKey);
}