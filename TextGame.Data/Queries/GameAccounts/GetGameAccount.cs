using Dapper;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.UserAccounts;

namespace TextGame.Data.Queries.GameAccounts;

public class GetGameAccount : IQuery<IGameAccount>
{
    public static GetGameAccount ById(long id) => new(id: id);

    public static GetGameAccount ByKey(string key) => new(key: key);

    internal static GetGameAccount ByGameIdAndUserAccountId(long gameId, long userAccountId) => new(gameId: gameId, userAccountId: userAccountId);

    private readonly long? id;

    private readonly string? key;

    private readonly long? gameId;

    private readonly long? userAccountId;

    private GetGameAccount(long? id = null, string? key = null, long? gameId = 0, long? userAccountId = 0)
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
                id as {nameof(GameAccountResource.Id)},
                resource_key as {nameof(GameAccountResource.Key)},
                user_account_id as {nameof(GameAccountResource.UserAccountId)},
                game_id as {nameof(GameAccountResource.GameId)},
                progress_json as {nameof(GameAccountResource.ProgressJson)}
            from
                game_accounts
            where
                deleted_at is null
                {SqlWhere.AndOptional("id", nameof(id), id)}
                {SqlWhere.AndOptional("game_id", nameof(gameId), id)}
                {SqlWhere.AndOptional("user_account_id", nameof(userAccountId), id)}
                {SqlWhere.AndOptional("resource_key", nameof(key), key)}",
            new
            {
                id,
                key
            });
    }
}