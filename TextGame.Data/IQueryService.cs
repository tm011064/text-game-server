using TextGame.Data.Contracts;

namespace TextGame.Data;

public interface IQueryService
{
    Task<TRecord> Run<TRecord>(IQuery<TRecord> query, AuthTicket ticket);
}
