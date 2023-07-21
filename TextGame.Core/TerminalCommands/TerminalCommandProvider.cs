using LazyCache;
using TextGame.Data.Contracts;
using TextGame.Data.Sources;

namespace TextGame.Core.TerminalCommands
{
    public class TerminalCommandProvider : ITerminalCommandProvider
    {
        private static readonly string CachePrefix = Guid.NewGuid().ToString();

        private readonly IAppCache cache;

        private readonly IGlobalResourceJsonSource<TerminalCommand[]> source;

        public TerminalCommandProvider(
            IGlobalResourceJsonSource<TerminalCommand[]> source,
            IAppCache cache)
        {
            this.cache = cache;
            this.source = source;
        }

        private IReadOnlyCollection<TerminalCommand> GetCached(string locale) => cache.GetOrAdd(
            $"{CachePrefix}-{locale}",
            () => source.Get(locale),
            TimeSpan.FromDays(1));

        public Task<IReadOnlyCollection<TerminalCommand>> Get(string locale) => Task.FromResult(GetCached(locale));
    }
}

