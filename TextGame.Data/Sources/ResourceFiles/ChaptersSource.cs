using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Contracts.Navigation;
using TextGame.Data.Resources;

namespace TextGame.Data.Sources.ResourceFiles;

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

    private static ChapterBuilder Load(
        string resourceName,
        string gameKey,
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
        string LocationKey,
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