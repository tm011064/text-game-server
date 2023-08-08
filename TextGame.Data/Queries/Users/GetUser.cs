namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Threading.Tasks;
using TextGame.Data.Contracts;

public class GetUser : IQuery<IUser?>
{
    public static GetUser ByEmail(string email) => new(email: email);

    public static GetUser ById(long id) => new(id: id);

    public static GetUser ByKey(string key) => new(key: key);

    private readonly string? email;

    private readonly long? id;

    private readonly string? key;

    private GetUser(string? email = null, long? id = null, string? key = null)
    {
        this.email = email;
        this.id = id;
        this.key = key;
    }

    public async Task<IUser?> Execute(IDbConnection connection)
    {
        return await connection.QuerySingleOrDefaultAsync<UserResource>($@"
            select
                {UsersSql.SelectColumns}
            from
                users
            where
                deleted_at is null
                {SqlWhere.AndOptional("email", nameof(email), email)}
                {SqlWhere.AndOptional("id", nameof(id), id)}
                {SqlWhere.AndOptional("resource_key", nameof(key), key)}",
            new
            {
                email,
                id,
                key
            });
    }
}
