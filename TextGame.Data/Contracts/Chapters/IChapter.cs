using TextGame.Data.Contracts.Games;
using TextGame.Data.Contracts.Navigation;
using TextGame.Data.Sources;

namespace TextGame.Data.Contracts.Chapters;

public interface IChapter
{
    string Key { get; }

    string LocationKey { get; }

    IGame Game { get; }

    LocalizedContentProvider<IReadOnlyCollection<Paragraph>> LocalizedParagraphs { get; }

    IReadOnlyCollection<NavigationCommand> NavigationCommands { get; }

    LocalizedContentProvider<Challenge> LocalizedChallenges { get; }

    string ForwardChapterKey { get; }
}
