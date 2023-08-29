using TextGame.Data.Contracts.TerminalCommands;

namespace TextGame.Data.Contracts.Navigation;

public record NavigationCommand(
    TerminalCommandType Type,
    string? ObjectKey,
    string ChapterKey)
{
    public string[] Tokens { get; init; } = Array.Empty<string>();
}
