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
            where
                deleted_at is null
                {SqlWhere.AndOptional("id", nameof(id), id)}
                {SqlWhere.AndOptional("game_id", nameof(gameId), gameId)}
                {SqlWhere.AndOptional("user_account_id", nameof(userAccountId), userAccountId)}
                {SqlWhere.AndOptional("resource_key", nameof(key), key)}",
            new
            {
                id,
                key,
                gameId,
                userAccountId
            });
    }
}
