namespace TextGame.Core.TerminalCommands;

using LazyCache;
using TextGame.Data.Contracts.TerminalCommands;
using TextGame.Data.Sources;

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

    private IReadOnlyDictionary<TerminalCommandType, TerminalCommand> GetCached(string locale) => cache.GetOrAdd(
        $"{CachePrefix}-{locale}",
        () => source.Get(locale).ToDictionary(x => x.Key),
        TimeSpan.FromDays(1));

    public Task<IReadOnlyDictionary<TerminalCommandType, TerminalCommand>> Get(string locale) => Task.FromResult(GetCached(locale));
}

