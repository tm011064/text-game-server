using Dapper;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.UserAccounts;

public class GetUserAccount : IQuery<IUserAccount>
{
    public static GetUserAccount ById(long id) => new(id: id);

    public static GetUserAccount ByKey(string key) => new(key: key);

    internal static GetUserAccount ByUserIdAndName(long userId, string name) => new(userId: userId, name: name);

    private readonly long? id;

    private readonly string? key;

    private readonly long? userId;

    private readonly string? name;

    private GetUserAccount(long? id = null, string? key = null, long? userId = null, string? name = null)
    {
        this.id = id;
        this.key = key;
        this.userId = userId;
        this.name = name;
    }

    public async Task<IUserAccount> Execute(QueryContext context)
    {
        return await context.Connection.QuerySingleAsync<UserAccountResource>($@"
            select
                {UserAccountsSql.SelectColumns}
            from
                user_accounts
            where
                deleted_at is null
                {SqlWhere.AndOptional("id", nameof(id), id)}
                {SqlWhere.AndOptional("user_id", nameof(userId), userId)}
                {SqlWhere.AndOptional("name", nameof(name), name)}
                {SqlWhere.AndOptional("resource_key", nameof(key), key)}",
            new
            {
                id,
                key,
                userId,
                name
            });
    }
}
