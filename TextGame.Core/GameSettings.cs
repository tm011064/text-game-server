using System;
using TextGame.Data.Contracts.Games;

namespace TextGame.Core;

public static class GameSettings
{
    public const string DefaultLocale = "en-US";
}

public record GameContext(IGame Game, string Locale);