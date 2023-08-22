using TextGame.Data.Sources.ResourceFiles;

namespace TextGame.Data.Contracts.Chapters;

public interface IChapter
{
    string Key { get; }

    string GameKey { get; }

    LocalizedContentProvider<IReadOnlyCollection<Paragraph>> ParagraphsByLocale { get; }

    IReadOnlyCollection<NavigationCommand> NavigationCommands { get; }

    Challenge? Challenge { get; }

    string ForwardChapterKey { get; }
}
