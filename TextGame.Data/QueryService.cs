using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace TextGame.Data;

public interface IQueryService
{
    Task<TRecord> Run<TRecord>(IQuery<TRecord> query);
}

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

    public Task<TRecord> Run<TRecord>(IQuery<TRecord> query)
    {
        using var connection = CreateConnection();

        return query.Execute(connection);
    }
}

public interface IQuery<TRecord>
{
    Task<TRecord> Execute(IDbConnection connection);
}