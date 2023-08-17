using TextGame.Data.Contracts;

namespace TextGame.Data;

public interface IQueryService
{
    Task<TRecord> Run<TRecord>(IQuery<TRecord> query, AuthTicket ticket);

    Task<TRecord> WithContext<TRecord>(Func<QueryContext, Task<TRecord>> query, AuthTicket ticket);
}
