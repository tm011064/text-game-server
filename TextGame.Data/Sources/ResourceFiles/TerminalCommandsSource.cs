using TextGame.Data.Contracts.TerminalCommands;

namespace TextGame.Data.Sources;

public interface ITerminalCommandsSource : IGlobalResourceJsonSource<TerminalCommandType, string>
{
}

public class TerminalCommandsSource :
    AbstractTwoWayGlobalLocalizedResourceJsonSource<TerminalCommand, TerminalCommandType, string>,
    ITerminalCommandsSource
{
    protected override string FilePrefix => "commands";

    public LocalizedContentProvider<TwoWayLookup<TerminalCommandType, string>> Get() => LoadTwoWayLookup();

    protected override TerminalCommandType GetKey(TerminalCommand value) => value.Key;

    protected override IEnumerable<string> GetValues(TerminalCommand key) => key.Terms;
}
