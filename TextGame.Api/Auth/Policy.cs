namespace TextGame.Api.Auth;

public static class Policy
{
    public const string HasGameAccount = nameof(HasGameAccount);

    public const string CanViewGameAccount = nameof(CanViewGameAccount);

    public const string CanManageUsers = nameof(CanManageUsers);
}