using FluentResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TextGame.Api.Controllers.Users;

public class JwtTokenValidator : IJwtTokenValidator
{
    private readonly string secretKey;

    private readonly JwtSecurityTokenHandler tokenHandler = new();

    public JwtTokenValidator(IConfiguration configuration)
    {
        secretKey = configuration.GetValue<string>("TokenAuthentication:SecretKey")
            ?? throw new Exception("JWT secret not configured");
    }

    public Result<JwtSecurityToken> Validate(string token)
    {
        var key = Encoding.ASCII.GetBytes(secretKey);

        try
        {
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                },
                out var validatedToken);

            return Result.Ok((JwtSecurityToken)validatedToken);
        }
        catch (Exception exception)
        {
            return Result.Fail<JwtSecurityToken>(new ExceptionalError(exception));
        }
    }
}