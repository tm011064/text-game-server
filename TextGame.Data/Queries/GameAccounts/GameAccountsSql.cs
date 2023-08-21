using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.GameAccounts;

internal static class GameAccountsSql
{
    public static readonly string SelectColumns = $@"
        game_accounts.id as {nameof(GameAccountResource.Id)},
        game_accounts.resource_key as {nameof(GameAccountResource.Key)},
        game_accounts.version as {nameof(GameAccountResource.Version)},
        game_accounts.game_states_json as {nameof(GameAccountResource.GameStateJson)},
        user_accounts.id as {nameof(GameAccountResource.UserAccountId)},
        user_accounts.resource_key as {nameof(GameAccountResource.UserAccountKey)},
        users.id as {nameof(GameAccountResource.UserId)},
        users.resource_key as {nameof(GameAccountResource.UserKey)},
        games.id as {nameof(GameAccountResource.GameId)},
        games.resource_key as {nameof(GameAccountResource.GameKey)}";
}