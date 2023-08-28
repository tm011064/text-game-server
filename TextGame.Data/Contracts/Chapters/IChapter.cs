using TextGame.Data.Contracts.Navigation;
using TextGame.Data.Sources;

namespace TextGame.Data.Contracts.Chapters;

public interface IChapter
{
    string Key { get; }

    string LocationKey { get; }

    string GameKey { get; }

    LocalizedContentProvider<IReadOnlyCollection<Paragraph>> LocalizedParagraphs { get; }

    IReadOnlyCollection<NavigationCommand> NavigationCommands { get; }

    LocalizedContentProvider<Challenge> LocalizedChallenges { get; }

    string ForwardChapterKey { get; }
}
