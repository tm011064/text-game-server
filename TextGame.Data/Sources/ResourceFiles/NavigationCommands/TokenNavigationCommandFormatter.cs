using System.Text.Json;
using TextGame.Data.Contracts.Navigation;

namespace TextGame.Data.Sources.ResourceFiles.NavigationCommands;

public class TokenNavigationCommandFormatter : INavigationCommandFormatterStrategy
{
    public bool Applies(NavigationCommand navigationCommand) => navigationCommand.Tokens.Any();

    public IEnumerable<(string Locale, NavigationCommand NavigationCommand)> Format(
        NavigationCommand navigationCommand,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> localeMap)
    {
        yield return (LocaleSettings.DefaultLocale, navigationCommand);

        var prefix = navigationCommand.Type.ToString().ToLowerInvariant();
        var suffix = navigationCommand.ChapterKey;

        var key = $"{prefix}-{suffix}";

        foreach (var (locale, lookup) in localeMap)
        {
            yield return (
                locale,
                navigationCommand with
                {
                    Tokens = lookup.TryGetValue(key, out var value)
                        ? value is JsonElement jsonElement
                            ? jsonElement.Deserialize<string[]>()!
                            : throw new InvalidCastException()
                        : navigationCommand.Tokens
                });
        }
    }
}