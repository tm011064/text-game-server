using FluentResults;
using System.IdentityModel.Tokens.Jwt;

namespace TextGame.Api.Controllers.Users;

public interface IJwtTokenValidator
{
    public Result<JwtSecurityToken> Validate(string token);
}
