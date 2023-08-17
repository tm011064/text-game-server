using System.Text.Json;

namespace TextGame.Data.Contracts;

internal class UserResource : IUser
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public string RolesJson { get; set; } = null!;

    public string Email { get; set; } = null!;

    public IReadOnlySet<string> Roles => JsonSerializer.Deserialize<string[]>(RolesJson).ToHashSet();
}
