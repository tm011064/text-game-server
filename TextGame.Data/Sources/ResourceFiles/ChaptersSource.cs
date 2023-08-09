using System.Text.Json;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Resources;

namespace TextGame.Data.Sources.ResourceFiles;

public class ChaptersSource : IGameResourceJsonSource<IChapter[]>
{
    private const string FOLDER_NAME = ".chapters.";

    public IChapter[] Get(string gameKey, string locale)
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

        return builder with { Key = chapterKey, GameKey = gameKey };
    }

    private record ChapterBuilder(
        string Key,
        string GameKey,
        IReadOnlyCollection<Paragraph> Paragraphs,
        string ForwardChapterKey,
        Challenge? Challenge) : IChapter
    {
        public IReadOnlyCollection<NavigationCommand> NavigationCommands { get; init; } = Array.Empty<NavigationCommand>();
    }
}