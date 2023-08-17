using System.Security.Claims;
using TextGame.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace TextGame.Api.Auth;

public class TokenAuthenticationOptions : AuthenticationSchemeOptions
{
}

public class TokenAuthenticationHandler : AuthenticationHandler<TokenAuthenticationOptions>
{
    private readonly IJwtTokenValidator validator;

    public TokenAuthenticationHandler(
        IOptionsMonitor<TokenAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IJwtTokenValidator validator) : base(options, logger, encoder, clock)
    {
        this.validator = validator;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!token.IsNullOrWhitespace())
        {
            var result = validator.Validate(token!);

            if (result.IsSuccess)
            {
                var userKey = result.Value.Claims.MaybeGetClaim(JwtRegisteredClaimNames.Sub);

                if (!userKey.IsNullOrWhitespace())
                {
                    var principal = new UserPrincipal(
                        new ClaimsIdentity(result.Value.Claims, "JWT"),
                        new UserIdentity(
                            userKey!,
                            IsGameAdmin: result.Value.Claims.MaybeGetClaim(CustomClaimNames.IsGameAdmin)?.Let(bool.Parse) ?? false,
                            GameKeys: result.Value.Claims.MaybeGetClaim(CustomClaimNames.GameIds)
                                ?.Let(x => x.Split(",").ToHashSet())
                                ?? Array.Empty<string>().ToHashSet(),
                            GameAccountKeys: result.Value.Claims.MaybeGetClaim(CustomClaimNames.GameAccountIds)
                                ?.Let(x => x.Split(",").ToHashSet())
                                ?? Array.Empty<string>().ToHashSet()
                            ));

                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }
            else
            {
                // TODO (Roman): remove
                Console.WriteLine(string.Join(",", result.Errors.Select(x => x.Message)) + " -> " + token);
            }
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
