using TextGame.Core.GameAccounts;
using TextGame.Data.Contracts.Games;

namespace TextGame.Core;

public static class GameSettings
{
    public const string DefaultLocale = "en-US";
}

public record GameContext(IGame Game, GameAccount GameAccount, string Locale);

public static class UserRole
{
    public const string User = nameof(User);

    public const string GameAdmin = nameof(GameAdmin);
}