namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Threading.Tasks;
using TextGame.Data.Contracts;

public class GetUserByEmail : IQuery<IUser>
{
    private readonly string email;

    public GetUserByEmail(string email)
    {
        this.email = email;
    }

    public async Task<IUser> Execute(IDbConnection connection)
    {
        return await connection.QuerySingleAsync<UserModel>($@"
            select
                id as {nameof(UserModel.Id)},
                resource_key as {nameof(UserModel.Key)},
                email as {nameof(UserModel.Email)}
            from
                users
            where
                email = @{nameof(email)}
                and deleted_at is null",
            new
            {
                email
            });
    }
}

public class GetUserByKey : IQuery<IUser>
{
    private readonly string key;

    public GetUserByKey(Guid key)
    {
        this.key = key.ToString().ToLowerInvariant();
    }

    public async Task<IUser> Execute(IDbConnection connection)
    {
        return await connection.QuerySingleAsync<UserModel>($@"
            select
                id as {nameof(UserModel.Id)},
                resource_key as {nameof(UserModel.Key)},
                email as {nameof(UserModel.Email)}
            from
                users
            where
                resource_key = @{nameof(key)}
                and deleted_at is null",
            new
            {
                key
            });
    }
}

public class GetUserById : IQuery<IUser>
{
    private readonly int id;

    public GetUserById(int id)
    {
        this.id = id;
    }

    public async Task<IUser> Execute(IDbConnection connection)
    {
        return await connection.QuerySingleAsync<UserModel>($@"
            select
                id as {nameof(UserModel.Id)},
                resource_key as {nameof(UserModel.Key)},
                email as {nameof(UserModel.Email)}
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

public class GetUserPassword : IQuery<UserPassword>
{
    private readonly int userId;

    public GetUserPassword(int userId)
    {
        this.userId = userId;
    }

    public Task<UserPassword> Execute(IDbConnection connection)
    {
        return connection.QuerySingleAsync<UserPassword>($@"
            select
                password_initialization_vector as {nameof(UserPassword.InitializationVector)},
                password_salt as {nameof(UserPassword.Salt)},
                password_iterations as {nameof(UserPassword.Iterations)},
                password_data as {nameof(UserPassword.Data)},
                password_cipher_bytes as {nameof(UserPassword.CipherBytes)}
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
                resource_key,
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
                key = key.ToString().ToLowerInvariant(),
                email,
                password.InitializationVector,
                password.Salt,
                password.Iterations,
                password.Data,
                password.CipherBytes,
                CreatedAt = ticket.CreatedAt.ToUnixTimeSeconds(),
                ticket.Identity
            });
    }
}