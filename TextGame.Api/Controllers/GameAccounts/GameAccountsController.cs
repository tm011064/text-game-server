﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextGame.Api.Auth;
using TextGame.Core.GameAccounts.Events;

namespace TextGame.Api.Controllers.GameAccounts;

[ApiController]
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

    [Authorize(Policy.CanViewGameAccount)]
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] PostGameAccountsSearchRequest request)
    {
        var ticket = this.GetTicket();
        var locale = this.GetLocale();

        var records = await mediator.Send(new SearchGameAccountsRequest(request.UserId, request.GameId, ticket));

        return Ok(records.Select(x => Wiring.ToWire(x, locale)).ToArray());
    }
}

public record PostGameAccountsSearchRequest(string? UserId, string? GameId);
