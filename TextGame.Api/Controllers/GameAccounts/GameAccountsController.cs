using MediatR;
using Microsoft.AspNetCore.Mvc;
using TextGame.Api.Auth;
using TextGame.Api.Controllers.Chapters;
using TextGame.Core.GameAccounts;
using TextGame.Core.GameAccounts.Events;
using TextGame.Data.Contracts.Chapters;

namespace TextGame.Api.Controllers.GameAccounts;

[ApiController]
[Authorize]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class GameAccountsController : ControllerBase
{
    private readonly IMediator mediator;

    public GameAccountsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] PostGameAccountsSearchRequest request)
    {
        var ticket = this.GetTicket();

        // TODO (Roman): authorization
        // TODO (Roman): validation

        var records = await mediator.Send(new SearchGameAccountsRequest(request.UserId, request.GameId, request.Locale, ticket));

        return Ok(records.Select(Wiring.ToWire).ToArray());
    }
}

public record PostGameAccountsSearchRequest(string? UserId, string? GameId, string Locale);
