using System.Data;

namespace TextGame.Data;

public interface IQuery<TRecord>
{
    Task<TRecord> Execute(IDbConnection connection);
}
