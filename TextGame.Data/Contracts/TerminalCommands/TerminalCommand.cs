using TextGame.Data.Contracts.Chapters;

namespace TextGame.Data.Contracts.TerminalCommands;

public record TerminalCommand(TerminalCommandType Key, string[] Terms);
