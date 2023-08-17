using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.UserAccounts;

internal static class UserAccountsSql
{
    public static readonly string SelectColumns = $@"
        user_accounts.id as {nameof(UserAccountResource.Id)},
        user_accounts.resource_key as {nameof(UserAccountResource.Key)},
        user_accounts.user_id as {nameof(UserAccountResource.UserId)},
        user_accounts.name as {nameof(UserAccountResource.Name)}";
}
