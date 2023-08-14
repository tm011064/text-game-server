using Dapper;
using TextGame.Data.Contracts.Games;
using TextGame.Data.Queries.Games;

namespace TextGame.Data.Queries.Users;

public class InsertGameIfNotExists : IQuery<IGame>
{
    private readonly string key;

    public InsertGameIfNotExists(string key)
    {
        this.key = key;
    }

    public async Task<IGame> Execute(QueryContext context)
    {
        await context.Connection.ExecuteAsync($@"
            insert into games (
                resource_key,
                created_at,
                created_by
            )
            values (
                @{nameof(key)},
                @{nameof(context.Ticket.CreatedAt)},
                @{nameof(context.Ticket.CreatedBy)}
            )
            on conflict do nothing;",
            new
            {
                key,
                context.Ticket.CreatedAt,
                context.Ticket.CreatedBy
            });

        return await context.Execute(GetGame.ByKey(key));
    }
}
