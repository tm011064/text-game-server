using TextGame.Core.GameAccounts;
using TextGame.Data.Contracts.Games;

namespace TextGame.Core;

public record GameContext(GameAccount GameAccount, string Locale)
{
    public IGame Game => GameAccount.Game;
}

public static class UserRole
{
    public const string User = nameof(User);

    public const string GameAdmin = nameof(GameAdmin);
}