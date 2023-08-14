using Dapper;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.UserAccounts;

public class InsertUserAccountIfNotExists : IQuery<IUserAccount>
{
    private readonly long userId;

    private readonly string key;

    private readonly string name;

    public InsertUserAccountIfNotExists(
        IUser user,
        string key,
        string name)
    {
        userId = user.Id;

        this.key = key;
        this.name = name;
    }

    public async Task<IUserAccount> Execute(QueryContext context)
    {
        var rowsAffected = await context.Connection.ExecuteAsync($@"
            insert into user_accounts (
                resource_key,
                user_id,
                name,
                created_at,
                created_by
            )
            values (
                @{nameof(key)},
                @{nameof(userId)},
                @{nameof(name)},
                @{nameof(context.Ticket.CreatedAt)},
                @{nameof(context.Ticket.CreatedBy)}
            )
            on conflict do nothing;",
            new
            {
                key,
                userId,
                name,
                context.Ticket.CreatedAt,
                context.Ticket.CreatedBy
            });

        return rowsAffected == 0
            ? await context.Execute(GetUserAccount.ByUserIdAndName(userId, name))
            : await context.Execute(GetUserAccount.ByKey(key));
    }
}
