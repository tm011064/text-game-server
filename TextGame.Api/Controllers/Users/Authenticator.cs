using TextGame.Api.Auth;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Queries.Users;

namespace TextGame.Api.Controllers.Users;

public interface IAuthenticator
{
    Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
}

public class Authenticator : IAuthenticator
{
    private readonly IQueryService queryService;

    private readonly Rfc2898PasswordValidator validator = new();

    private readonly IJwtTokenFactory tokenFactory;

    public Authenticator(IQueryService queryService, IJwtTokenFactory tokenFactory)
    {
        this.queryService = queryService;
        this.tokenFactory = tokenFactory;
    }

    public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest request)
    {
        var user = await queryService.Run(new GetUserByEmail(request.Email!));

        if (user == null)
        {
            return null;
        }

        var password = await queryService.Run(new GetUserPassword(user.Id));

        if (!validator.IsValid(request.Password!, password))
        {
            return null;
        }

        var token = tokenFactory.Create(user);

        return new AuthenticateResponse(user, token);
    }
}

