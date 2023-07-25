using FluentResults;
using MediatR;
using TextGame.Api.Controllers.Users;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Queries.Users;
using static TextGame.Api.Controllers.Authentication.Events.AuthenticationRequest;

namespace TextGame.Api.Controllers.Authentication.Events;

public class AuthenticationRequestHandler : IRequestHandler<AuthenticationRequest, Result<Response>>
{
    private readonly Rfc2898PasswordValidator validator = new();

    private readonly IQueryService queryService;

    private readonly IJwtTokenFactory tokenFactory;

    public AuthenticationRequestHandler(
        IQueryService queryService,
        IJwtTokenFactory tokenFactory)
    {
        this.queryService = queryService;
        this.tokenFactory = tokenFactory;
    }

    public async Task<Result<Response>> Handle(AuthenticationRequest request, CancellationToken cancellationToken)
    {
        var user = await queryService.Run(GetUser.ByEmail(request.Email!));

        if (user == null)
        {
            return Result.Fail<Response>("User not found");
        }

        var password = await queryService.Run(new GetUserPassword(user.Id));

        if (!validator.IsValid(request.Password!, password))
        {
            return Result.Fail<Response>("Password not valid");
        }

        var token = tokenFactory.Create(user);

        return Result.Ok(new Response(user, token));
    }
}


