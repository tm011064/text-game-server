using System.Security.Cryptography;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.Users;

namespace TextGame.Api.Auth;

public class RefreshTokenFactory : IRefreshTokenFactory
{
    private readonly IQueryService queryService;

    private readonly TimeSpan refreshTokenExpiry;

    public RefreshTokenFactory(IConfiguration configuration, IQueryService queryService)
    {
        this.queryService = queryService;

        refreshTokenExpiry = configuration.GetValue<TimeSpan?>("TokenAuthentication:RefreshTokenExpiry")
            ?? TimeSpan.FromDays(1);
    }

    public async Task<string> Create(IUser user)
    {
        var token = GenerateRefreshToken();

        await queryService.Run(new UpdateUserRefreshToken(user.Id, token, DateTimeOffset.UtcNow.Add(refreshTokenExpiry)));

        return token;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
}
