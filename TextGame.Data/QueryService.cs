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

        var context = new QueryContext(connection, ticket);

        return query.Execute(context);
    }

    public Task<TRecord> WithContext<TRecord>(Func<QueryContext, Task<TRecord>> query, AuthTicket ticket)
    {
        using var connection = CreateConnection();

        var context = new QueryContext(connection, ticket);

        return query(context);
    }
}

public record QueryContext(IDbConnection Connection, AuthTicket Ticket)
{
    public Task<TRecord> Execute<TRecord>(IQuery<TRecord> query)
    {
        return query.Execute(this);
    }
};
