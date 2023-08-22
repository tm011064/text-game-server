using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Resources;

namespace TextGame.Data.Sources.ResourceFiles;

internal static class LocaleSettings
{
    public const string DefaultLocale = "en-US";
}

public class ChallengeFormatter
{
    private static readonly IReadOnlyDictionary<ChallengeType, IChallengeFormatter> challengeTypeParsers = new Dictionary<ChallengeType, IChallengeFormatter>
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

public interface IChallengeFormatter
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
    IChallengeFormatter
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
    IChallengeFormatter
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

public partial class ChaptersSource : IGameResourceJsonSource<IChapter[]>
{
    private const string FOLDER_NAME = ".chapters.";

    private static readonly ChallengeFormatter challengeFormatter = new();

    private static readonly Regex regex = Regex();

    [GeneratedRegex("^([a-zA-Z0-9\\-\\.]*).([a-z]{2})-([a-zA-Z]{2}).json$", RegexOptions.Compiled)]
    private static partial Regex Regex();

    public IChapter[] Get(string gameKey)
    {
        var (keyValueMaps, files) = ResourceService.ResourceNames[gameKey]
            .Where(x => x.Contains(FOLDER_NAME))
            .Select(x =>
            {
                var match = regex.Match(x);

                return match.Success
                    ? new
                    {
                        ResourceName = x,
                        FileName = match.Groups[1].Value,
                        IsKeyValueMap = true,
                        Locale = $"{match.Groups[2].Value}-{match.Groups[3].Value}"
                    }
                    : new
                    {
                        ResourceName = x,
                        FileName = x[..x.LastIndexOf('.')],
                        IsKeyValueMap = false,
                        Locale = ""
                    };
            })
            .Split(x => x.IsKeyValueMap);

        var keyValueMapsByFileName = keyValueMaps.ToLookup(x => x.FileName);

        return files
            .Select(file =>
            {
                var map = keyValueMapsByFileName[file.FileName];

                return Load(
                    file.ResourceName,
                    gameKey,
                    map.ToDictionary(x => x.Locale, x => LoadKeyValueMap(x.ResourceName)));
            })
            .ToArray();
    }

    private static IReadOnlyDictionary<string, object> LoadKeyValueMap(string resourceName)
    {
        using var stream = ResourceService.ResourceAssembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);

        var json = reader.ReadToEnd();

        var items = JsonSerializer.Deserialize<KeyValueMapItem[]>(json, JsonOptions.Default)
            ?? throw new Exception();

        return items.ToDictionary(x => x.Key, x => x.Value);
    }

    private record KeyValueMapItem(string Key, object Value);

    private static ChapterBuilder Load(string resourceName, string gameKey,
          IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> keyValueMapByLocale)
    {
        using var stream = ResourceService.ResourceAssembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);

        var json = reader.ReadToEnd();

        var builder = JsonSerializer.Deserialize<ChapterBuilder>(json, JsonOptions.Default)
            ?? throw new Exception();

        var fileName = resourceName[(resourceName.IndexOf(FOLDER_NAME) + FOLDER_NAME.Length)..];
        var chapterKey = fileName[..fileName.LastIndexOf('.')];
        var paragraphsByLocale = builder.Paragraphs?.ToDictionary(x => x.Locale, x => x.Paragraphs);

        return builder with
        {
            Key = chapterKey,
            GameKey = gameKey,
            LocalizedParagraphs = new LocalizedContentProvider<IReadOnlyCollection<Paragraph>>(paragraphsByLocale),
            LocalizedChallenges = new LocalizedContentProvider<Challenge>(
                builder.Challenge?.Let(x => challengeFormatter.Format(x, keyValueMapByLocale).ToDictionary(x => x.Locale, x => x.Challenge)))
        };
    }

    private record ChapterBuilder(
        string Key,
        string GameKey,
        string ForwardChapterKey,
        [property: JsonPropertyName("localizedParagraphs")] LocalizedParagraph[] Paragraphs,
        Challenge? Challenge) : IChapter
    {
        public IReadOnlyCollection<NavigationCommand> NavigationCommands { get; init; } = Array.Empty<NavigationCommand>();

        [JsonIgnore]
        public LocalizedContentProvider<IReadOnlyCollection<Paragraph>> LocalizedParagraphs { get; init; } = null!;

        [JsonIgnore]
        public LocalizedContentProvider<Challenge> LocalizedChallenges { get; init; } = null!;
    }
}

public class LocalizedContentProvider<TValue>
{
    public static readonly LocalizedContentProvider<TValue> Empty = new(null);

    private readonly IReadOnlyDictionary<string, TValue>? map;

    public LocalizedContentProvider(IReadOnlyDictionary<string, TValue>? map)
    {
        this.map = map;
    }

    public TValue? First()
    {
        return map == null
            ? default
            : map!.First().Value;
    }

    public TValue? Get(string locale)
    {
        return map == null
            ? default
            : map!.GetOrNotFound(locale);
    }

    public TValue Get(string locale, TValue defaultValue)
    {
        return map == null
            ? defaultValue
            : map!.GetOrNotFound(locale);
    }
}