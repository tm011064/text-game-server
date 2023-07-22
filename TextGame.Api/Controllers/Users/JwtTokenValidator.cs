using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TextGame.Api.Controllers.Users;

public interface IJwtTokenValidator
{
    public int? Validate(string? token);
}

public class JwtTokenValidator : IJwtTokenValidator
{
    private readonly string secretKey;

    public JwtTokenValidator(IConfiguration configuration)
    {
        secretKey = configuration.GetValue<string>("TokenAuthentication:SecretKey")
            ?? throw new Exception("JWT secret not configured");
    }

    public int? Validate(string? token) // TODO (Roman): non null
    {
        if (token == null)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(secretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // TODO (Roman): return token rather than id
            return userId;
        }
        catch (Exception exception)
        {
            return null;
        }
    }
}