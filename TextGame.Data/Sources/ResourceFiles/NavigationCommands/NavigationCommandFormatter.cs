using TextGame.Data.Contracts.Navigation;

namespace TextGame.Data.Sources.ResourceFiles.NavigationCommands;

public class NavigationCommandFormatter
{
    private static readonly INavigationCommandFormatter defaultFormatter = new PassThroughCommandFormatter();

    private static readonly IReadOnlyCollection<INavigationCommandFormatterStrategy> formatters = new[]
    {
        new TokenNavigationCommandFormatter()
    };

    public IEnumerable<(string Locale, NavigationCommand NavigationCommand)> Format(
        NavigationCommand navigationCommand,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> localeMap)
    {
        var formatter = formatters.SingleOrDefault(x => x.Applies(navigationCommand))
            ?? defaultFormatter;

        return formatter.Format(navigationCommand, localeMap);
    }
}
