namespace TextGame.Api.Controllers.Authentications;

using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TextGame.Api.Auth;
using TextGame.Api.Controllers.Authentication.Events;
using TextGame.Api.Controllers.Authentication.Models;

[ApiController]
[Authorize]
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
    public async Task<IActionResult> Authenticate(AuthenticateRequest model)
    {
        var response = await mediator.Send(
            new AuthenticationRequest(model.Email!, model.Password!));

        return response switch
        {
            { IsSuccess: true } => Ok(ToWire(response)),

            _ => BadRequest(new { message = "Email or password is incorrect" })
        };
    }

    private static object ToWire(Result<AuthenticationRequest.Response> response) => new
    {
        response.Value.Token
    };
}

