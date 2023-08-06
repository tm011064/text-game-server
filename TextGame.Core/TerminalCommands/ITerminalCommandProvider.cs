namespace TextGame.Core.TerminalCommands;

using TextGame.Data.Contracts.TerminalCommands;

public interface ITerminalCommandProvider
{
    Task<IReadOnlyCollection<TerminalCommand>> Get(string locale = GameSettings.DefaultLocale);
}