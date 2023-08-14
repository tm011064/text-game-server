using Dapper;
using TextGame.Data.Contracts.Games;

namespace TextGame.Data.Queries.Games;

public class SearchGames : IQuery<IReadOnlyCollection<IGame>>
{
    public async Task<IReadOnlyCollection<IGame>> Execute(QueryContext context)
    {
        var records = await context.Connection
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