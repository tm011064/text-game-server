namespace TextGame.Data.Contracts.Chapters;

public record NavigationCommand(
    NavigationCommandType Type,
    string ChapterKey);
