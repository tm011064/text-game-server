using Dapper;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.GameAccounts;

public class UpdateGameAccountGameState : IQuery<IGameAccount>
{
    private readonly long gameAccountId;

    private readonly long version;

    private readonly string gameStateJson;

    public UpdateGameAccountGameState(long gameAccountId, long version, string gameStateJson)
    {
        this.gameAccountId = gameAccountId;
        this.version = version;
        this.gameStateJson = gameStateJson;
    }

    public async Task<IGameAccount> Execute(QueryContext context)
    {
        var rowsAffected = await context.Connection.ExecuteAsync($@"
            update
                game_accounts
            set
                game_states_json = @{nameof(gameStateJson)},
                version = {SqlStatements.CreateRandomNumber}
            where
                deleted_at is null
                and id = @{nameof(gameAccountId)}
                and version = @{nameof(version)}",
            new
            {
                gameAccountId,
                version,
                gameStateJson
            });

        return rowsAffected == 1
            ? await context.Execute(GetGameAccount.ById(gameAccountId))
            : throw new ConcurrencyException();
    }
}
