using TextGame.Data.Contracts.Navigation;

namespace TextGame.Data.Sources.ResourceFiles.NavigationCommands;

public class PassThroughCommandFormatter : INavigationCommandFormatter
{
    public IEnumerable<(string Locale, NavigationCommand NavigationCommand)> Format(
        NavigationCommand navigationCommand,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> localeMap)
    {
        yield return (LocaleSettings.DefaultLocale, navigationCommand);

        foreach (var locale in localeMap.Keys)
        {
            yield return (locale, navigationCommand);
        }
    }
}
