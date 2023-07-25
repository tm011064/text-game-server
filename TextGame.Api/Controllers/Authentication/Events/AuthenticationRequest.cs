using FluentResults;
using MediatR;
using TextGame.Data.Contracts;
using static TextGame.Api.Controllers.Authentication.Events.AuthenticationRequest;

namespace TextGame.Api.Controllers.Authentication.Events;

public record AuthenticationRequest(string Email, string Password) : IRequest<Result<Response>>
{
    public record Response(IUser User, string Token);
}


