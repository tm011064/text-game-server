using Dapper;
using System.Data;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.UserAccounts;

public class GetUserAccount : IQuery<IUserAccount>
{
    public static GetUserAccount ById(long id) => new(id: id);

    public static GetUserAccount ByKey(string key) => new(key: key);

    private readonly long? id;

    private readonly string? key;

    private GetUserAccount(long? id = null, string? key = null)
    {
        this.id = id;
        this.key = key;
    }

    public async Task<IUserAccount> Execute(IDbConnection connection)
    {
        return await connection.QuerySingleAsync<UserAccountResource>($@"
            select
                id as {nameof(UserAccountResource.Id)},
                resource_key as {nameof(UserAccountResource.Key)},
                user_id as {nameof(UserAccountResource.UserId)},
                name as {nameof(UserAccountResource.Name)}
            from
                user_accounts
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