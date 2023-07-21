namespace TextGame.Core.TerminalCommands;

using TextGame.Data.Contracts;

public interface ITerminalCommandProvider
{
    Task<IReadOnlyCollection<TerminalCommand>> Get(string locale = "en-US");
}