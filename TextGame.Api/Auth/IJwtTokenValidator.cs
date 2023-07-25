using FluentResults;
using System.IdentityModel.Tokens.Jwt;

namespace TextGame.Api.Auth;

public interface IJwtTokenValidator
{
    public Result<JwtSecurityToken> Validate(string token);
}
