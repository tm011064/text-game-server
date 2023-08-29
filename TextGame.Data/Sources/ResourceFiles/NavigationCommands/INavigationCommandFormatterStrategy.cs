using TextGame.Data.Contracts.Navigation;

namespace TextGame.Data.Sources.ResourceFiles.NavigationCommands;

public interface INavigationCommandFormatter
{
    IEnumerable<(string Locale, NavigationCommand NavigationCommand)> Format(
        NavigationCommand navigationCommand,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> localeMap);
}

public interface INavigationCommandFormatterStrategy : INavigationCommandFormatter
{
    bool Applies(NavigationCommand navigationCommand);
}
