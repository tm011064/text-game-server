namespace TextGame.Data.Queries.Users;

using Dapper;
using System.Data;
using System.Threading.Tasks;
using TextGame.Data.Contracts;

public class UpdateUserPassword : IQuery<long>
{
    private readonly long id;

    private readonly UserPassword password;

    public UpdateUserPassword(
        long id,
        UserPassword password)
    {
        this.id = id;
        this.password = password;
    }

    public Task<long> Execute(QueryContext context)
    {
        return context.Connection.QuerySingleAsync<long>($@"
            update
                users
            set
                password_initialization_vector = @{nameof(password.InitializationVector)},
                password_salt = @{nameof(password.Salt)},
                password_iterations = @{nameof(password.Iterations)},
                password_data = @{nameof(password.Data)},
                password_cipher_text = @{nameof(password.CipherBytes)},
                updated_at = @{nameof(context.Ticket.CreatedAt)},
                updated_by = @{nameof(context.Ticket.CreatedBy)}
            where
                id = @{nameof(id)}            ",
            new
            {
                id,
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