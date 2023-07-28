namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Threading.Tasks;

public class UpdateUserRefreshToken : IQuery<int>
{
    private readonly int id;

    private readonly string token;

    private readonly DateTimeOffset expiresAt;

    public UpdateUserRefreshToken(int id, string token, DateTimeOffset expiresAt)
    {
        this.id = id;
        this.token = token;
        this.expiresAt = expiresAt;
    }

    public Task<int> Execute(IDbConnection connection)
    {
        return connection.QuerySingleAsync<int>($@"
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