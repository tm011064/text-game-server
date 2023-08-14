namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Threading.Tasks;
using TextGame.Data.Contracts;

public class GetUserPassword : IQuery<UserPassword>
{
    private readonly long userId;

    public GetUserPassword(long userId)
    {
        this.userId = userId;
    }

    public Task<UserPassword> Execute(IDbConnection connection, AuthTicket ticket)
    {
        return connection.QuerySingleAsync<UserPassword>($@"
            select
                password_initialization_vector as {nameof(UserPassword.InitializationVector)},
                password_salt as {nameof(UserPassword.Salt)},
                password_iterations as {nameof(UserPassword.Iterations)},
                password_data as {nameof(UserPassword.Data)},
                password_cipher_text as {nameof(UserPassword.CipherBytes)}
            from
                users
            where
                id = @{nameof(userId)}
                and deleted_at is null",
            new { userId });
    }
}
