using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextGame.Api.Auth;
using TextGame.Api.Controllers.Authentication.Events;
using TextGame.Api.Controllers.Authentication.Models;

namespace TextGame.Api.Controllers.Authentications;

[ApiController]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class AuthenticationsController : ControllerBase
{
    private readonly IMediator mediator;

    public AuthenticationsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Authenticate(AuthenticateRequest request)
    {
        var response = await mediator.Send(
            new AuthenticateUserRequest(request.Email!, request.Password!));

        return response switch
        {
            { IsSuccess: true } => Ok(ToWire(response)),

            _ => BadRequest(new { message = "Email or password is incorrect" })
        };
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var response = await mediator.Send(
            new CreateRefreshTokenRequest(request.Token!, request.RefreshToken!));

        return response switch
        {
            { IsSuccess: true } => Ok(ToWire(response)),

            _ => BadRequest(new { message = "Token invalid" })
        };
    }

    private static object ToWire(Result<UserTokenResponse> response) => new
    {
        response.Value.Token,
        response.Value.RefreshToken
    };
}

