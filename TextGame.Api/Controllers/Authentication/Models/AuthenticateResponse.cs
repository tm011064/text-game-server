using TextGame.Data.Contracts;

namespace TextGame.Api.Controllers.Authentication.Models;

public record AuthenticateResponse(string Id, string Token)
{
    public static AuthenticateResponse FromUser(
        IUser user,
        string token) => new(user.Key, token);
};
