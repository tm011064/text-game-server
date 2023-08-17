using Dapper;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.UserAccounts;

public class SearchUserAccounts : IQuery<IReadOnlyCollection<IUserAccount>>
{
    private readonly long? userId;

    public SearchUserAccounts(long? userId = null)
    {
        this.userId = userId;
    }

    public async Task<IReadOnlyCollection<IUserAccount>> Execute(QueryContext context)
    {
        var records = await context.Connection.QueryAsync<UserAccountResource>($@"
            select
                {UserAccountsSql.SelectColumns}
            from
                user_accounts
            where
                deleted_at is null
                {SqlWhere.AndOptional("user_id", nameof(userId), userId)}",
            new
            {
                userId
            });

        return records.ToArray();
    }
}