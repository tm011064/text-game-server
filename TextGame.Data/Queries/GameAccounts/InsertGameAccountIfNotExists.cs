using Dapper;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Games;

namespace TextGame.Data.Queries.GameAccounts;

public class InsertGameAccountIfNotExists : IQuery<IGameAccount>
{
    private readonly long userAccountId;

    private readonly long gameId;

    private readonly string key;

    private readonly string gameStateJson;

    public InsertGameAccountIfNotExists(
        IUserAccount userAccount,
        IGame game,
        string key,
        string gameStateJson)
    {
        this.key = key;
        this.gameStateJson = gameStateJson;

        gameId = game.Id;
        userAccountId = userAccount.Id;
    }

    public async Task<IGameAccount> Execute(QueryContext context)
    {
        var rowsAffected = await context.Connection.ExecuteAsync($@"
            insert into game_accounts (
                resource_key,
                version,
                user_account_id,
                game_id,
                game_states_json,
                created_at,
                created_by
            )
            values (
                @{nameof(key)},
                {SqlStatements.CreateRandomNumber},
                @{nameof(userAccountId)},
                @{nameof(gameId)},
                @{nameof(gameStateJson)},
                @{nameof(context.Ticket.CreatedAt)},
                @{nameof(context.Ticket.CreatedBy)}
            )
            on conflict do nothing;",
            new
            {
                key,
                userAccountId,
                gameId,
                gameStateJson,
                context.Ticket.CreatedAt,
                context.Ticket.CreatedBy
            });

        return rowsAffected == 0
            ? await context.Execute(GetGameAccount.ByGameIdAndUserAccountId(gameId, userAccountId))
            : await context.Execute(GetGameAccount.ByKey(key));
    }
}
