using System.Text.Json;

namespace TextGame.Data.Sources.ResourceFiles.Challenges;

public interface IChallengeConfigurationFormatter
{
    IEnumerable<(string Locale, object Configuration)> Format(
        object value,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> localeMap);
}

public abstract class AbstractChallengeConfigurationFormatter<TValue>
{
    protected abstract TValue DoFormat(TValue source, string locale, IReadOnlyDictionary<string, object> map);

    public IEnumerable<(string Locale, object Configuration)> Format(
        object value,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> localeMap) => FormatValue(
            JsonSerializer.Deserialize<TValue>(value.ToString()!, JsonOptions.Default)!,
            localeMap);

    private IEnumerable<(string Locale, object Configuration)> FormatValue(
        TValue value,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> localeMap)
    {
        yield return (LocaleSettings.DefaultLocale, value!);

        foreach (var (locale, map) in localeMap)
        {
            yield return (
                locale,
                DoFormat(value, locale, map)!);
        }
    }
}
