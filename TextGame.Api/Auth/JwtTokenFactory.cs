using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TextGame.Core;
using TextGame.Core.Games;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Games;

namespace TextGame.Api.Auth;

public class JwtTokenFactory : IJwtTokenFactory
{
    private readonly string secretKey;

    private readonly TimeSpan tokenExpiry;

    private readonly JwtSecurityTokenHandler tokenHandler = new();

    private readonly IGameProvider gameProvider;

    public JwtTokenFactory(IConfiguration configuration, IGameProvider gameProvider)
    {
        secretKey = configuration.GetValue<string>("TokenAuthentication:SecretKey")
            ?? throw new Exception("JWT secret not configured");

        tokenExpiry = configuration.GetValue<TimeSpan?>("TokenAuthentication:TokenExpiry")
            ?? TimeSpan.FromMinutes(1);
        this.gameProvider = gameProvider;
    }

    public async Task<string> Create(IUser user, IReadOnlyCollection<IGameAccount> gameAccounts)
    {
        var gamesById = await gameProvider.GetMap();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Key),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(
                CustomClaimNames.IsGameAdmin,
                user.Roles.Contains(UserRole.GameAdmin).ToString()),
            new Claim(
                CustomClaimNames.GameIds,
                string.Join(",", gameAccounts.Select(x => gamesById.GetOrNotFound(x.GameId).Key).Distinct())),
            new Claim(
                CustomClaimNames.GameAccountIds,
                string.Join(",", gameAccounts.Select(x => x.Key)))
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

public static class CustomClaimNames
{
    public const string IsGameAdmin = nameof(IsGameAdmin);

    public const string GameIds = nameof(GameIds);

    public const string GameAccountIds = nameof(GameAccountIds);
}