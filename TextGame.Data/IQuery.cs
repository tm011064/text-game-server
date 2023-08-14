using System.Data;
using TextGame.Data.Contracts;

namespace TextGame.Data;

public interface IQuery<TRecord>
{
    Task<TRecord> Execute(IDbConnection connection, AuthTicket ticket);
}
