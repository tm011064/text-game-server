using FluentResults;
using MediatR;

namespace TextGame.Api.Controllers.Authentication.Events;

public record CreateRefreshTokenRequest(string Token, string RefreshToken) : IRequest<Result<UserTokenResponse>>;
