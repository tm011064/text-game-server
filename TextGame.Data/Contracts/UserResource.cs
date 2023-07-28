namespace TextGame.Data.Contracts;

internal class UserResource : IUser
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public string Email { get; set; } = null!;
}
