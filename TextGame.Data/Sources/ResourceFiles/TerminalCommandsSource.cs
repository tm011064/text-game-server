using TextGame.Data.Contracts.TerminalCommands;

namespace TextGame.Data.Sources;

public class TerminalCommandsSource :
    AbstractGlobalResourceJsonSource<TerminalCommand[]>,
    IGlobalResourceJsonSource<TerminalCommand[]>
{
    public override string FileName { get; } = "commands.json";
}
