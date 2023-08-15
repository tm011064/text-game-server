using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.GameAccounts;

internal static class GameAccountsSql
{
    public static readonly string SelectColumns = $@"
        game_accounts.id as {nameof(GameAccountResource.Id)},
        game_accounts.resource_key as {nameof(GameAccountResource.Key)},
        game_accounts.version as {nameof(GameAccountResource.Version)},
        game_accounts.user_account_id as {nameof(GameAccountResource.UserAccountId)},
        game_accounts.game_id as {nameof(GameAccountResource.GameId)},
        game_accounts.game_states_json as {nameof(GameAccountResource.GameStateJson)}";
}
