using LazyCache;
using TextGame.Data;
using TextGame.Data.Contracts.TerminalCommands;
using TextGame.Data.Sources;

namespace TextGame.Core.TerminalCommands;

public interface ITerminalCommandProvider
{
    TwoWayLookup<TerminalCommandType, string> Get(string locale);
}

public class TerminalCommandProvider : ITerminalCommandProvider
{
    private static readonly string CacheKey = Guid.NewGuid().ToString();

    private readonly IAppCache cache;

    private readonly ITerminalCommandsSource source;

    public TerminalCommandProvider(
        ITerminalCommandsSource source,
        IAppCache cache)
    {
        this.cache = cache;
        this.source = source;
    }

    private LocalizedContentProvider<TwoWayLookup<TerminalCommandType, string>> GetCached() => cache.GetOrAdd(
        CacheKey,
        source.Get,
        TimeSpan.FromDays(1));

    public TwoWayLookup<TerminalCommandType, string> Get(string locale) => GetCached().Get(locale)!;
}
