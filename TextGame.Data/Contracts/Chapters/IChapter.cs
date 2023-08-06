namespace TextGame.Data.Contracts.Chapters;

public interface IChapter
{
    string Key { get; }

    string GameKey { get; }

    IReadOnlyCollection<Paragraph> Paragraphs { get; }

    IReadOnlyCollection<NavigationCommand> NavigationCommands { get; }

    Challenge? Challenge { get; }
}
