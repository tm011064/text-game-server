namespace TextGame.Data.Contracts.Chapters;

public interface IChapter
{
    string Key { get; }

    string GameKey { get; } // TODO (Roman): proper domain model with objects

    IReadOnlyCollection<Paragraph> Paragraphs { get; }

    IReadOnlyCollection<NavigationCommand> NavigationCommands { get; }

    Challenge? Challenge { get; }

    string ForwardChapterKey { get; }
}
