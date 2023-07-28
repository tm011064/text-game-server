using TextGame.Data.Contracts;

namespace TextGame.Api.Controllers.Authentication.Events;

public record UserTokenResponse(IUser User, string Token, string RefreshToken);
