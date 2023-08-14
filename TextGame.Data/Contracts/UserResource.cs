namespace TextGame.Data.Contracts;

internal class UserResource : IUser
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public string Email { get; set; } = null!;
}
