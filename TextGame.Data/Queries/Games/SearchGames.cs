using Dapper;
using System.Data;
using TextGame.Data.Contracts.Games;

namespace TextGame.Data.Queries.Games;

public class SearchGames : IQuery<IReadOnlyCollection<IGame>>
{
    public async Task<IReadOnlyCollection<IGame>> Execute(IDbConnection connection)
    {
        var records = await connection
            .QueryAsync<GameResource>($@"
            select
                id as {nameof(GameResource.Id)},
                resource_key as {nameof(GameResource.Key)}
            from
                games
            where
                deleted_at is null");

        return records.ToArray();
    }
}