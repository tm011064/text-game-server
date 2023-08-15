using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.Users;

internal static class UsersSql
{
    public static readonly string SelectColumns = $@"
        id as {nameof(UserResource.Id)},
        resource_key as {nameof(UserResource.Key)},
        email as {nameof(UserResource.Email)}";
}
