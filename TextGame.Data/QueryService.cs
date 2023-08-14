using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;
using TextGame.Data.Contracts;

namespace TextGame.Data;

public class QueryService : IQueryService
{
    protected readonly string connectionString;

    public QueryService(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("SqlLiteDatabase")!;
    }

    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(connectionString);
    }

    public Task<TRecord> Run<TRecord>(IQuery<TRecord> query, AuthTicket ticket)
    {
        using var connection = CreateConnection();

        return query.Execute(connection, ticket);
    }
}
