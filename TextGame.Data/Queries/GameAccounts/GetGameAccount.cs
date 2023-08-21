using Dapper;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.GameAccounts;

public class GetGameAccount : IQuery<IGameAccount>
{
    public static GetGameAccount ById(long id) => new(id: id);

    public static GetGameAccount ByKey(string key) => new(key: key);

    public static GetGameAccount ByGameIdAndUserAccountId(long gameId, long userAccountId) => new(gameId: gameId, userAccountId: userAccountId);

    private readonly long? id;

    private readonly string? key;

    private readonly long? gameId;

    private readonly long? userAccountId;

    private GetGameAccount(long? id = null, string? key = null, long? gameId = null, long? userAccountId = null)
    {
        this.id = id;
        this.key = key;
        this.gameId = gameId;
        this.userAccountId = userAccountId;
    }

    public async Task<IGameAccount> Execute(QueryContext context)
    {
        return await context.Connection.QuerySingleAsync<GameAccountResource>($@"
            select
                {GameAccountsSql.SelectColumns}
            from
                game_accounts
                join user_accounts on game_accounts.user_account_id = user_accounts.id
                join users on user_accounts.user_id = users.id
                join games on game_accounts.game_id = games.id
            where
                game_accounts.deleted_at is null
                {SqlWhere.AndOptional("game_accounts.id", nameof(id), id)}
                {SqlWhere.AndOptional("game_accounts.game_id", nameof(gameId), gameId)}
                {SqlWhere.AndOptional("game_accounts.user_account_id", nameof(userAccountId), userAccountId)}
                {SqlWhere.AndOptional("game_accounts.resource_key", nameof(key), key)}",
            new
            {
                id,
                key,
                gameId,
                userAccountId
            });
    }
}