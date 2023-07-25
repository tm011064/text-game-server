using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TextGame.Data.Contracts;

namespace TextGame.Api.Auth;

public class JwtTokenFactory : IJwtTokenFactory
{
    private readonly string secretKey;

    public JwtTokenFactory(IConfiguration configuration)
    {
        secretKey = configuration.GetValue<string>("TokenAuthentication:SecretKey")
            ?? throw new Exception("JWT secret not configured");
    }

    public string Create(IUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[] {
                    new Claim("id", user.Id.ToString())
                }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
