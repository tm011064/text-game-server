using TextGame.Data.Contracts.TerminalCommands;

namespace TextGame.Data.Contracts.Chapters;

public record NavigationCommand(
    TerminalCommandType Type,
    string ChapterKey);
