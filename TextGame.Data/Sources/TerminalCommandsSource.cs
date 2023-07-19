using TextGame.Data.Contracts;

namespace TextGame.Data.Sources
{
    public class TerminalCommandsSource :
        AbstractJsonSource<TerminalCommand[]>,
        IGameContextItemJsonSource<TerminalCommand[]>
    {
        public override string FileName { get; } = "commands.json";
    }

}

