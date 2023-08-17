using FluentResults;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using TextGame.Api.Auth;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.GameAccounts;
using TextGame.Data.Queries.Users;

namespace TextGame.Api.Controllers.Authentication.Events;

public class CreateRefreshTokenRequestHandler : IRequestHandler<CreateRefreshTokenRequest, Result<UserTokenResponse>>
{
    private readonly IQueryService queryService;

    private readonly IJwtTokenFactory tokenFactory;

    private readonly IRefreshTokenFactory refreshTokenFactory;

    private readonly string secretKey;

    public CreateRefreshTokenRequestHandler(
        IQueryService queryService,
        IJwtTokenFactory tokenFactory,
        IRefreshTokenFactory refreshTokenFactory,
        IConfiguration configuration)
    {
        this.queryService = queryService;
        this.tokenFactory = tokenFactory;
        this.refreshTokenFactory = refreshTokenFactory;

        secretKey = configuration.GetValue<string>("TokenAuthentication:SecretKey")
            ?? throw new Exception("JWT secret not configured");
    }

    public async Task<Result<UserTokenResponse>> Handle(CreateRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = GetPrincipal(request.Token);
        if (principal == null)
        {
            return Result.Fail<UserTokenResponse>("Principal empty");
        }

        var userKey = principal?.Claims.MaybeGetClaim(JwtRegisteredClaimNames.Sub);
        if (userKey == null)
        {
            return Result.Fail<UserTokenResponse>($"Claim {JwtRegisteredClaimNames.Sub} not found");
        }

        var (user, gameAccounts) = await queryService.WithContext(async context =>
        {
            var user = await context.Execute(GetAuthenticatedUser.ByKey(userKey));
            var gameAccounts = await context.Execute(new SearchGameAccounts(userId: user.Id));

            return (user, gameAccounts);
        }, AuthTicket.System);

        if (user == null)
        {
            return Result.Fail<UserTokenResponse>($"User with key {userKey} not found");
        }

        if (user.RefreshToken != request.RefreshToken)
        {
            return Result.Fail<UserTokenResponse>($"User {userKey} refresh token invalid");
        }

        if (user.RefreshTokenExpiresAt <= DateTimeOffset.UtcNow)
        {
            return Result.Fail<UserTokenResponse>($"Refresh token for {userKey} expired");
        }

        var token = await tokenFactory.Create(user, gameAccounts);
        var refreshToken = await refreshTokenFactory.Create(user, AuthTicket.System);

        return Result.Ok(new UserTokenResponse(user, token, refreshToken));
    }

    private ClaimsPrincipal? GetPrincipal(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(
            token,
            new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = false
            },
            out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}