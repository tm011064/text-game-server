using Dapper;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.GameAccounts;

public class SearchGameAccounts : IQuery<IReadOnlyCollection<IGameAccount>>
{
    private readonly long? id;

    private readonly string? key;

    private readonly long? gameId;

    private readonly long? userAccountId;

    private readonly long? userId;

    private readonly string? userKey;

    private readonly string? gameKey;

    public SearchGameAccounts(
        long? id = null,
        string? key = null,
        long? gameId = null,
        long? userAccountId = null,
        long? userId = null,
        string? userKey = null,
        string? gameKey = null)
    {
        this.id = id;
        this.key = key;
        this.gameId = gameId;
        this.userAccountId = userAccountId;
        this.userId = userId;
        this.userKey = userKey;
        this.gameKey = gameKey;
    }

    public async Task<IReadOnlyCollection<IGameAccount>> Execute(QueryContext context)
    {
        var records = await context.Connection.QueryAsync<GameAccountResource>($@"
            select
                {GameAccountsSql.SelectColumns}
            from
                game_accounts
                join user_accounts on game_accounts.user_account_id = user_accounts.id
                join users on user_accounts.user_id = users.id
                join games on game_accounts.game_id = games.id
            where
                game_accounts.deleted_at is null
                {SqlWhere.AndOptional("game_accounts.id", nameof(id), id)}
                {SqlWhere.AndOptional("game_accounts.game_id", nameof(gameId), gameId)}
                {SqlWhere.AndOptional("game_accounts.user_account_id", nameof(userAccountId), userAccountId)}
                {SqlWhere.AndOptional("game_accounts.resource_key", nameof(key), key)}
                {(userId.HasValue ? SqlWhere.AndOptional("user_accounts.user_id", nameof(userId), userId) : "")}
                {(userKey.NotEmpty() ? SqlWhere.AndOptional("users.resource_key", nameof(userKey), userKey) : "")}
                {(gameKey.NotEmpty() ? SqlWhere.AndOptional("games.resource_key", nameof(gameKey), gameKey) : "")}
            ",
            new
            {
                id,
                key,
                gameId,
                userAccountId,
                userId,
                gameKey,
                userKey
            });

        return records.ToArray();
    }
}
