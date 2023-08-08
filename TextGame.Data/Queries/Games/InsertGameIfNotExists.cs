namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Net.Sockets;
using System.Threading.Tasks;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Games;

public class InsertGameIfNotExists : IQuery<long>
{
    private readonly string key;

    private readonly AuthTicket ticket;

    public InsertGameIfNotExists(
        string key,
        AuthTicket ticket)
    {
        this.key = key;
        this.ticket = ticket;
    }

    public Task<long> Execute(IDbConnection connection)
    {
        return connection.QuerySingleAsync<long>($@"
            insert into games (
                resource_key,
                created_at,
                created_by
            )
            values (
                @{nameof(key)},
                @{nameof(ticket.CreatedAt)},
                @{nameof(ticket.CreatedBy)}
            )
            on conflict do nothing;

            select
                id
            from
                games
            where
                resource_key = @{nameof(key)}
                and deleted_at is null;",
            new
            {
                key,
                ticket.CreatedAt,
                ticket.CreatedBy
            });
    }
}
