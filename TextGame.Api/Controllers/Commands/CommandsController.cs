﻿namespace TextGame.Api.Controllers.Commands;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using TextGame.Core;
using TextGame.Core.Games;
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
    private readonly IGameProvider gameProvider;

    private readonly IMediator mediator;

    public CommandsController(IMediator mediator, IGameProvider gameProvider)
    {
        this.mediator = mediator;
        this.gameProvider = gameProvider;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostCommandRequest request)
    {
        var ticket = this.GetTicket();

        if (request.GameId.IsNullOrWhitespace())
        {
            return BadRequest($"{nameof(request.GameId)} must not be empty");
        }
        if (request.ChapterId.IsNullOrWhitespace())
        {
            return BadRequest($"{nameof(request.ChapterId)} must not be empty");
        }
        if (request.CommandType == null)
        {
            return BadRequest($"{nameof(request.CommandType)} must not be empty");
        }
        if ((request.Tokens?.Length ?? 0) == 0)
        {
            return BadRequest($"{nameof(request.Tokens)} must not be empty");
        }

        var game = await gameProvider.Get(request.GameId!);

        await mediator.Send(new PerformCommandRequest(
            new GameContext(game, GameSettings.DefaultLocale),
            request.ChapterId!,
            request.Tokens!,
            request.CommandType ?? throw new Exception(),
            ticket));

        throw new NotImplementedException();
    }
}

public record PostSearchRequest(string? GameId, string? Locale = null);

public record PostCommandRequest(
    [Required] string? GameId,
    [Required] string? ChapterId,
    [Required] TerminalCommandType? CommandType,
    [Required] string[]? Tokens,
    [Required] string? Locale);