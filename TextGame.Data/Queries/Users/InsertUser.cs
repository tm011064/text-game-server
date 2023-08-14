namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Net.Sockets;
using System.Threading.Tasks;
using TextGame.Data.Contracts;

public class InsertUser : IQuery<long>
{
    private readonly string key;

    private readonly string email;

    private readonly UserPassword password;

    public InsertUser(
        string key,
        string email,
        UserPassword password)
    {
        this.key = key;
        this.email = email;
        this.password = password;
    }

    public Task<long> Execute(QueryContext context)
    {
        return context.Connection.QuerySingleAsync<long>($@"
            insert into users (
                resource_key,
                email,
                password_initialization_vector,
                password_salt,
                password_iterations,
                password_data,
                password_cipher_text,
                created_at,
                created_by,
                updated_at,
                updated_by
            )
            values (
                @{nameof(key)},
                @{nameof(email)},
                @{nameof(password.InitializationVector)},
                @{nameof(password.Salt)},
                @{nameof(password.Iterations)},
                @{nameof(password.Data)},
                @{nameof(password.CipherBytes)},
                @{nameof(context.Ticket.CreatedAt)},
                @{nameof(context.Ticket.CreatedBy)},
                @{nameof(context.Ticket.CreatedAt)},
                @{nameof(context.Ticket.CreatedBy)}
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
                context.Ticket.CreatedAt,
                context.Ticket.CreatedBy
            });
    }
}
