using Dapper;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.Users;

public class SearchUsers : IQuery<IReadOnlyCollection<IUser>>
{
    private readonly string text;

    private readonly int limit;

    public SearchUsers(string text, int limit)
    {
        this.text = $"%{text}%";
        this.limit = limit;
    }

    public async Task<IReadOnlyCollection<IUser>> Execute(QueryContext context)
    {
        var records = await context.Connection.QueryAsync<UserResource>($@"
            select
                {UsersSql.SelectColumns}
            from
                users
            where
                deleted_at is null
                and email like @{nameof(text)}                
            order by email asc
            limit @{nameof(limit)}",
            new
            {
                text,
                limit
            });

        return records.ToArray();
    }
}