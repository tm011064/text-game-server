namespace TextGame.Data.Contracts;

public class AuthenticatedUser : IUser
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
}