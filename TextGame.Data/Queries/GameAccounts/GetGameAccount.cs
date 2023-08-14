using Dapper;
using System.Data;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.GameAccounts;

public class GetGameAccount : IQuery<IGameAccount>
{
    public static GetGameAccount ById(long id) => new(id: id);

    public static GetGameAccount ByKey(string key) => new(key: key);

    private readonly long? id;

    private readonly string? key;

    private GetGameAccount(long? id = null, string? key = null)
    {
        this.id = id;
        this.key = key;
    }

    public async Task<IGameAccount> Execute(IDbConnection connection, AuthTicket ticket)
    {
        return await connection.QuerySingleAsync<GameAccountResource>($@"
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
                {SqlWhere.AndOptional("resource_key", nameof(key), key)}",
            new
            {
                id,
                key
            });
    }
}