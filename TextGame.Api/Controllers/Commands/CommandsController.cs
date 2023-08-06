namespace TextGame.Api.Controllers.Commands;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using TextGame.Core.TerminalCommands;
using TextGame.Core.TerminalCommands.Events;
using TextGame.Data;
using TextGame.Data.Contracts.TerminalCommands;

[ApiController]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class CommandsController : ControllerBase
{
    private readonly ILogger<CommandsController> logger;

    private readonly ITerminalCommandProvider provider;

    private readonly IMediator mediator;

    public CommandsController(ILogger<CommandsController> logger, ITerminalCommandProvider provider, IMediator mediator)
    {
        this.logger = logger;
        this.provider = provider;
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostCommandRequest request)
    {
        var ticket = this.GetTicket();

        if (request.GameId.IsNullOrWhitespace())
        {
            return BadRequest("GameId must not be empty");
        }
        if (request.ChapterId.IsNullOrWhitespace())
        {
            return BadRequest("GameId must not be empty");
        }
        if ((request.Tokens?.Length ?? 0) == 0)
        {
            return BadRequest("No tokens provided");
        }

        await mediator.Send(new PerformCommandRequest(request.GameId!, request.ChapterId!, request.Tokens!, ticket));
        throw new NotImplementedException();
    }

    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] PostSearchRequest request)
    {
        throw new NotImplementedException();
    }

    private object ToWire(TerminalCommand record) => new
    {
        Id = record.Key,
        record.Terms
    };
}

public record PostSearchRequest(string? GameId, string? Locale = null);

public record PostCommandRequest(string? GameId, string? ChapterId, string[]? Tokens, string? Locale);