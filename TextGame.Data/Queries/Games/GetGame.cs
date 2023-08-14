using Dapper;
using System.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Games;

namespace TextGame.Data.Queries.Games;

public class GetGame : IQuery<IGame>
{
    public static GetGame ById(long id) => new(id: id);

    public static GetGame ByKey(string key) => new(key: key);

    private readonly long? id;

    private readonly string? key;

    private GetGame(long? id = null, string? key = null)
    {
        this.id = id;
        this.key = key;
    }

    public async Task<IGame> Execute(QueryContext context)
    {
        return await context.Connection.QuerySingleAsync<GameResource>($@"
            select
                id as {nameof(GameResource.Id)},
                resource_key as {nameof(GameResource.Key)}
            from
                games
            where
                deleted_at is null
                {SqlWhere.AndOptional("id", nameof(id), id)}
                {SqlWhere.AndOptional("resource_key", nameof(key), key)}",
            new
            {
                id,
                key
            });
    }
}
