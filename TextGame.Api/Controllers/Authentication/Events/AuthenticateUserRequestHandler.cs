﻿using FluentResults;
using MediatR;
using Microsoft.Extensions.Configuration;
using TextGame.Api.Auth;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Queries.Users;
using static TextGame.Api.Controllers.Authentication.Events.AuthenticateUserRequest;

namespace TextGame.Api.Controllers.Authentication.Events;

public class AuthenticateUserRequestHandler : IRequestHandler<AuthenticateUserRequest, Result<UserTokenResponse>>
{
    private readonly Rfc2898PasswordValidator validator = new();

    private readonly IQueryService queryService;

    private readonly IJwtTokenFactory tokenFactory;

    private readonly IRefreshTokenFactory refreshTokenFactory;

    public AuthenticateUserRequestHandler(
        IQueryService queryService,
        IJwtTokenFactory tokenFactory,
        IRefreshTokenFactory refreshTokenFactory)
    {
        this.queryService = queryService;
        this.tokenFactory = tokenFactory;
        this.refreshTokenFactory = refreshTokenFactory;
    }

    public async Task<Result<UserTokenResponse>> Handle(AuthenticateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await queryService.Run(GetUser.ByEmail(request.Email!));

        if (user == null)
        {
            return Result.Fail<UserTokenResponse>("User not found");
        }

        var password = await queryService.Run(new GetUserPassword(user.Id));

        if (!validator.IsValid(request.Password!, password))
        {
            return Result.Fail<UserTokenResponse>("Password not valid");
        }

        var token = tokenFactory.Create(user);
        var refreshToken = await refreshTokenFactory.Create(user);

        return Result.Ok(new UserTokenResponse(user, token, refreshToken));
    }
}
