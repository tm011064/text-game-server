using Dapper;
using System.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Games;

namespace TextGame.Data.Queries.GameAccounts;

public class InsertGameAccountIfNotExists : IQuery<long>
{
    private readonly long userAccountId;

    private readonly long gameId;

    private readonly string key;

    private readonly string progressJson;

    private readonly AuthTicket ticket;

    public InsertGameAccountIfNotExists(
        IUserAccount userAccount,
        IGame game,
        string key,
        string progressJson,
        AuthTicket ticket)
    {
        this.key = key;
        this.progressJson = progressJson;
        this.ticket = ticket;

        gameId = game.Id;
        userAccountId = userAccount.Id;
    }

    public Task<long> Execute(IDbConnection connection)
    {
        return connection.QuerySingleAsync<long>($@"
            insert into game_accounts (
                resource_key,
                user_account_id,
                game_id,
                progress_json,
                created_at,
                created_by
            )
            values (
                @{nameof(key)},
                @{nameof(userAccountId)},
                @{nameof(gameId)},
                @{nameof(progressJson)},
                @{nameof(ticket.CreatedAt)},
                @{nameof(ticket.CreatedBy)}
            )
            on conflict do nothing;

            select
                id
            from
                game_accounts
            where
                resource_key = @{nameof(key)}
                and deleted_at is null;",
            new
            {
                key,
                userAccountId,
                gameId,
                progressJson,
                ticket.CreatedAt,
                ticket.CreatedBy
            });
    }
}
