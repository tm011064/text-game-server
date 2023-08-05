namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Net.Sockets;
using System.Threading.Tasks;
using TextGame.Data.Contracts;

public class InsertUser : IQuery<int>
{
    private readonly string key;

    private readonly string email;

    private readonly UserPassword password;

    private readonly AuthTicket ticket;

    public InsertUser(
        string key,
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
                password_cipher_text,
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
                @{nameof(ticket.CreatedBy)}
            );

            select last_insert_rowid();",
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
                ticket.CreatedBy
            });
    }
}
