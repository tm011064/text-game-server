using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TextGame.Data.Contracts;

namespace TextGame.Api.Auth;

public class JwtTokenFactory : IJwtTokenFactory
{
    private readonly string secretKey;

    private readonly TimeSpan tokenExpiry;

    private readonly JwtSecurityTokenHandler tokenHandler = new();

    public JwtTokenFactory(IConfiguration configuration)
    {
        secretKey = configuration.GetValue<string>("TokenAuthentication:SecretKey")
            ?? throw new Exception("JWT secret not configured");

        tokenExpiry = configuration.GetValue<TimeSpan?>("TokenAuthentication:TokenExpiry")
            ?? TimeSpan.FromMinutes(1);
    }

    public string Create(IUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Key),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(tokenExpiry),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
