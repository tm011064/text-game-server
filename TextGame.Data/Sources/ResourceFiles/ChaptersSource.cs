using System.Text.Json;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Resources;

namespace TextGame.Data.Sources.ResourceFiles;

public class ChaptersSource : IGameResourceJsonSource<IChapter[]>
{
    private const string FOLDER_NAME = ".chapters.";

    public IChapter[] Get(string gameKey)
    {
        return ResourceService.ResourceNames[gameKey]
            .Where(x => x.Contains(FOLDER_NAME))
            .Select(x => Load(x, gameKey))
            .ToArray();
    }

    private static ChapterBuilder Load(string resourceName, string gameKey)
    {
        using var stream = ResourceService.ResourceAssembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);

        var json = reader.ReadToEnd();

        var builder = JsonSerializer.Deserialize<ChapterBuilder>(json, JsonOptions.Default)
            ?? throw new Exception();

        var fileName = resourceName[(resourceName.IndexOf(FOLDER_NAME) + FOLDER_NAME.Length)..];
        var chapterKey = fileName[..fileName.LastIndexOf('.')];
        var paragraphsByLocale = builder.LocalizedParagraphs?.ToDictionary(x => x.Locale, x => x.Paragraphs);

        return builder with
        {
            Key = chapterKey,
            GameKey = gameKey,
            ParagraphsByLocale = new LocalizedContentProvider<IReadOnlyCollection<Paragraph>>(paragraphsByLocale)
        };
    }

    private record ChapterBuilder(
        string Key,
        string GameKey,
        string ForwardChapterKey,
        LocalizedParagraph[] LocalizedParagraphs,
        Challenge? Challenge) : IChapter
    {
        public IReadOnlyCollection<NavigationCommand> NavigationCommands { get; init; } = Array.Empty<NavigationCommand>();

        public LocalizedContentProvider<IReadOnlyCollection<Paragraph>> ParagraphsByLocale { get; init; } = null!;
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