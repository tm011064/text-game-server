﻿using FluentResults;
using MediatR;
using TextGame.Api.Auth;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.GameAccounts;
using TextGame.Data.Queries.UserAccounts;
using TextGame.Data.Queries.Users;

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
        var ticket = AuthTicket.System;
        var user = await queryService.Run(GetUser.ByEmail(request.Email!), ticket);

        if (user == null)
        {
            return Result.Fail<UserTokenResponse>("User not found");
        }

        var (password, gameAccounts) = await queryService.WithContext(async context =>
        {
            var password = await queryService.Run(new GetUserPassword(user.Id), ticket);
            var gameAccounts = await context.Execute(new SearchGameAccounts(userId: user.Id));

            return (password, gameAccounts);
        }, ticket);

        if (!validator.IsValid(request.Password!, password))
        {
            return Result.Fail<UserTokenResponse>("Password not valid");
        }

        var token = await tokenFactory.Create(user, gameAccounts);
        var refreshToken = await refreshTokenFactory.Create(user, ticket);

        return Result.Ok(new UserTokenResponse(user, token, refreshToken));
    }
}
