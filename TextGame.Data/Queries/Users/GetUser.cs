namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Threading.Tasks;
using TextGame.Data.Contracts;

public class GetUserByKey : IQuery<User?>
{
    private readonly Guid key;

    public GetUserByKey(Guid key)
    {
        this.key = key;
    }

    public Task<User?> Execute(IDbConnection connection)
    {
        return connection.QuerySingleAsync<User?>($@"
            select
                id as {nameof(User.Id)},
                user_key as {nameof(User.Key)},
                email as {nameof(User.Email)}
            from
                users
            where
                user_key = @{nameof(key)}
                and deleted_at is null",
            new
            {
                key
            });
    }
}
public class GetUserById : IQuery<User?>
{
    private readonly int id;

    public GetUserById(int id)
    {
        this.id = id;
    }

    public Task<User?> Execute(IDbConnection connection)
    {
        return connection.QuerySingleAsync<User?>($@"
            select
                id as {nameof(User.Id)},
                user_key as {nameof(User.Key)},
                email as {nameof(User.Email)}
            from
                users
            where
                id = @{nameof(id)}
                and deleted_at is null",
            new
            {
                id
            });
    }
}

public class GetUserPassword : IQuery<User?>
{
    private readonly int userId;

    public GetUserPassword(int userId)
    {
        this.userId = userId;
    }

    public Task<User?> Execute(IDbConnection connection)
    {
        return connection.QuerySingleAsync<User?>($@"
            select
                password_initialization_vector as {nameof(UserPassword.InitializationVector)},
                password_salt as {nameof(UserPassword.Salt)},
                password_iterations as {nameof(UserPassword.Iterations)},
                password_data as {nameof(UserPassword.Data)},
                password_cipher_bytes as {nameof(UserPassword.CipherBytes)},
            from
                users
            where
                id = @{nameof(userId)}
                and deleted_at is null",
            new { userId });
    }
}

public class InsertUser : IQuery<int>
{
    private readonly Guid key;

    private readonly string email;

    private readonly UserPassword password;

    private readonly AuthTicket ticket;

    public InsertUser(
        Guid key,
        string email,
        UserPassword password,
        AuthTicket ticket)
    {
        this.key = key;
        this.email = email;
        this.password = password;
        this.ticket = ticket;
    }

    public Task<int> Execute(IDbConnection connection)
    {
        return connection.QuerySingleAsync<int>($@"
            insert into users (
                user_key,
                email,
                password_initialization_vector,
                password_salt,
                password_iterations,
                password_data,
                password_cipher_bytes,
                created_at,
                created_by
            )
            values (
                @{nameof(key)},
                @{nameof(email)},
                @{nameof(password.InitializationVector)},
                @{nameof(password.Salt)},
                @{nameof(password.Iterations)},
                @{nameof(password.Data)},
                @{nameof(password.CipherBytes)},
                @{nameof(ticket.CreatedAt)},
                @{nameof(ticket.Identity)}
            );

            select last_insert_rowid()",
            new
            {
                key,
                email,
                password.InitializationVector,
                password.Salt,
                password.Iterations,
                password.Data,
                password.CipherBytes,
                ticket.CreatedAt,
                ticket.Identity
            });
    }
}