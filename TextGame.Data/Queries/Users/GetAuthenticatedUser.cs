namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Threading.Tasks;
using TextGame.Data.Contracts;

public class GetAuthenticatedUser : IQuery<AuthenticatedUser>
{
    public static GetAuthenticatedUser ById(long id) => new(id: id);

    public static GetAuthenticatedUser ByKey(string key) => new(key: key);

    private readonly long? id;

    private readonly string? key;

    private GetAuthenticatedUser(long? id = null, string? key = null)
    {
        this.id = id;
        this.key = key;
    }

    public async Task<AuthenticatedUser> Execute(IDbConnection connection)
    {
        return await connection.QuerySingleAsync<AuthenticatedUser>($@"
            select
                {UsersSql.SelectColumns},
                refresh_token as {nameof(AuthenticatedUser.RefreshToken)},
                refresh_token_expires_at as {nameof(AuthenticatedUser.RefreshTokenExpiresAt)}
            from
                users
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