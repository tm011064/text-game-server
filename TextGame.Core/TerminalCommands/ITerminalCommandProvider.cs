namespace TextGame.Core.TerminalCommands;

using TextGame.Data.Contracts.TerminalCommands;

public interface ITerminalCommandProvider
{
    Task<IReadOnlyDictionary<TerminalCommandType, TerminalCommand>> Get(string locale = GameSettings.DefaultLocale);
}