using TextGame.Data.Contracts;

namespace TextGame.Core.TerminalCommands
{
    public interface ITerminalCommandProvider
    {
        Task<IReadOnlyCollection<TerminalCommand>> Get(string locale = "en-US");
    }
}