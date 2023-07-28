using FluentResults;
using MediatR;

namespace TextGame.Api.Controllers.Authentication.Events;

public record AuthenticateUserRequest(string Email, string Password) : IRequest<Result<UserTokenResponse>>;
