namespace TextGame.Data;

public interface IQuery<TRecord>
{
    Task<TRecord> Execute(QueryContext context);
}
