using System.Text.Json;
using TextGame.Data.Contracts.Chapters;

namespace TextGame.Data.Sources.ResourceFiles;

public class ChallengeFormatter
{
    private static readonly IReadOnlyDictionary<ChallengeType, IChallengeConfigurationFormatter> challengeTypeParsers = new Dictionary<ChallengeType, IChallengeConfigurationFormatter>
    {
        { ChallengeType.TypeKeys, new TypeKeysConfigurationChallengeFormatter() },
        { ChallengeType.LearnTabKey, new LearnTabKeyChallengeFormatter() }
    };

    public IEnumerable<(string Locale, Challenge Challenge)> Format(
        Challenge challenge,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> keyValueMapByLocale)
    {
        var formatter = challengeTypeParsers.GetOrNotFound(challenge.Type);

        foreach (var (locale, configuration) in formatter.Format(challenge.Configuration, keyValueMapByLocale))
        {
            yield return (
                locale,
                challenge with
                {
                    SuccessMessage = keyValueMapByLocale.GetValueOrDefault("success-message")?.ToString() ?? challenge.SuccessMessage,
                    Configuration = configuration
                });
        }
    }
}

public interface IChallengeConfigurationFormatter
{
    IEnumerable<(string Locale, object Configuration)> Format(
        object value,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> keyValueMapByLocale);
}

public abstract class AbstractChallengeFormatter<TValue>
{
    protected abstract TValue DoFormat(TValue source, string locale, IReadOnlyDictionary<string, object> map);

    public IEnumerable<(string Locale, object Configuration)> Format(
        object value,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> keyValueMapByLocale) => FormatValue(
            JsonSerializer.Deserialize<TValue>(value.ToString()!, JsonOptions.Default)!,
            keyValueMapByLocale);

    private IEnumerable<(string Locale, object Configuration)> FormatValue(
        TValue value,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> keyValueMapByLocale)
    {
        yield return (LocaleSettings.DefaultLocale, value!);

        foreach (var (locale, map) in keyValueMapByLocale)
        {
            yield return (
                locale,
                DoFormat(value, locale, map)!);
        }
    }
}

public class LearnTabKeyChallengeFormatter :
    AbstractChallengeFormatter<LearnTabKeyConfiguration>,
    IChallengeConfigurationFormatter
{
    protected override LearnTabKeyConfiguration DoFormat(LearnTabKeyConfiguration source, string locale, IReadOnlyDictionary<string, object> map)
    {
        return source with
        {
            Items = source.Items
                .Select((item, index) => item with
                {
                    Description = map.GetValueOrDefault($"description-{index + 1}")?.ToString() ?? item.Description
                })
                .ToArray()
        };
    }
}

public class TypeKeysConfigurationChallengeFormatter :
    AbstractChallengeFormatter<TypeKeysConfiguration>,
    IChallengeConfigurationFormatter
{
    protected override TypeKeysConfiguration DoFormat(TypeKeysConfiguration source, string locale, IReadOnlyDictionary<string, object> map)
    {
        return source with
        {
            Items = source.Items
                .Select((item, index) => item with
                {
                    Description = map.GetValueOrDefault($"description-{index + 1}")?.ToString() ?? item.Description,
                    Keys = map.GetValueOrDefault($"description-{index + 1}-keys")
                        ?.Let(x => JsonSerializer.Deserialize<string[]>(x.ToString()!, JsonOptions.Default))
                        ?? item.Keys
                })
                .ToArray()
        };
    }
}
