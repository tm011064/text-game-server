namespace TextGame.Data.Contracts;

internal class UserModel : IUser
{
    public int Id { get; set; }

    public string Key { get; set; }

    public string Email { get; set; }
}

