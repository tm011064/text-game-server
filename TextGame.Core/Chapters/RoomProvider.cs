using System;
using LazyCache;
using TextGame.Data.Contracts;
using TextGame.Data.Sources;

namespace TextGame.Core.Rooms
{
    public class ChapterProvider : IChapterProvider
    {
        private readonly IGameContextProvider gameContextProvider;

        public ChapterProvider(IGameContextProvider gameContextProvider)
        {
            this.gameContextProvider = gameContextProvider;
        }

        public async Task<Chapter> GetChapter(string gameKey, string chapterKey, string locale)
        {
            var gameContext = await gameContextProvider.Get(gameKey, locale);

            return gameContext.Chapters
                .Single(x => string.Equals(x.Key, chapterKey) == true);
        }
    }

    public interface IChapterProvider
    {
        Task<Chapter> GetChapter(string gameKey, string chapterKey, string locale = "en-US");
    }

    public class GameContextProvider : IGameContextProvider
    {
        private readonly IAppCache cache;

        private readonly IGameContextSource gameContextSource;

        public GameContextProvider(IAppCache cache, IGameContextSource gameContextSource)
        {
            this.cache = cache;
            this.gameContextSource = gameContextSource;
        }

        public async Task<GameContext> Get(string gameKey, string locale)
        {
            return await cache.GetOrAddAsync(
                gameKey,
                () => Task.FromResult(gameContextSource.Get(gameKey, locale)));
        }
    }

    public interface IGameContextProvider
    {
        Task<GameContext> Get(string gameKey, string locale = "en-US");
    }
}

