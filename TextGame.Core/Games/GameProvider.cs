using LazyCache;
using System;
using TextGame.Core.Emotions;
using TextGame.Data.Contracts.Emotions;
using TextGame.Data.Sources;

namespace TextGame.Core.Games;

using LazyCache;
using TextGame.Data;
using TextGame.Data.Contracts.Emotions;
using TextGame.Data.Contracts.Games;
using TextGame.Data.Queries.Games;
using TextGame.Data.Resources;
using TextGame.Data.Sources;

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

    private async Task<IGame> GetCached(string key) => await cache.GetOrAdd(
        $"{CachePrefix}-{key}",
        async () =>
        {
            return ResourceService.GameKeys.Contains(key)
                ? await queryService.Run(GetGame.ByKey(key))
                : throw new InvalidOperationException(); // TODO (Roman): fix this
        },
        TimeSpan.FromDays(1));

    public async Task<IGame> Get(string key) => await GetCached(key);
}