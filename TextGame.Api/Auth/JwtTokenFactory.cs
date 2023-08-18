using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TextGame.Core;
using TextGame.Core.Games;
using TextGame.Data;
using TextGame.Data.Contracts;

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

        var subject = new ClaimsIdentity();

        subject.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Key));
        subject.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        subject.AddClaim(new Claim(
            CustomClaimNames.IsGameAdmin,
            user.Roles.Contains(UserRole.GameAdmin).ToString(),
            ClaimValueTypes.Boolean));

        foreach (var gameId in gameAccounts.Select(x => x.GameId).Distinct())
        {
            subject.AddClaim(new Claim(CustomClaimNames.GameIds, gamesById.GetOrNotFound(gameId).Key));
        }
        foreach (var gameAccount in gameAccounts)
        {
            subject.AddClaim(new Claim(CustomClaimNames.GameAccountIds, gameAccount.Key));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
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
    public const string IsGameAdmin = "isGameAdmin";

    public const string GameIds = "gameIds";

    public const string GameAccountIds = "gameAccountIds";
}