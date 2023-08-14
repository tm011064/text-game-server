namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Threading.Tasks;

public class UpdateUserRefreshToken : IQuery<int>
{
    private readonly long id;

    private readonly string token;

    private readonly DateTimeOffset expiresAt;

    public UpdateUserRefreshToken(long id, string token, DateTimeOffset expiresAt)
    {
        this.id = id;
        this.token = token;
        this.expiresAt = expiresAt;
    }

    public Task<int> Execute(QueryContext context)
    {
        return context.Connection.QuerySingleAsync<int>($@"
            update users
            set
                refresh_token = @{nameof(token)},
                refresh_token_expires_at = @{nameof(expiresAt)}
            where
                id = @{nameof(id)}
                and deleted_at is null;

            select changes();",
            new
            {
                id,
                token,
                expiresAt = expiresAt.ToUnixTimeSeconds()
            });
    }
}
