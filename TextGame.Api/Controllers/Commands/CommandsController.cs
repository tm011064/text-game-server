using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TextGame.Api.Auth;
using TextGame.Api.Controllers.Chapters;
using TextGame.Api.Controllers.GameAccounts;
using TextGame.Core;
using TextGame.Core.GameAccounts.Events;
using TextGame.Core.TerminalCommands.Events;
using TextGame.Data;
using TextGame.Data.Contracts.TerminalCommands;

namespace TextGame.Api.Controllers.Commands;

[ApiController]
[Authorize(Policy = Policy.HasGameAccount)]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class CommandsController : ControllerBase
{
    private readonly IMediator mediator;

    public CommandsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [Authorize(Policy.HasGameAccount)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostCommandRequest request)
    {
        var ticket = this.GetTicket();

        if (request.GameAccountId.IsNullOrWhitespace())
        {
            return BadRequest($"{nameof(request.GameAccountId)} must not be empty");
        }
        if (request.ChapterId.IsNullOrWhitespace())
        {
            return BadRequest($"{nameof(request.ChapterId)} must not be empty");
        }
        if (request.CommandType == null)
        {
            return BadRequest($"{nameof(request.CommandType)} must not be empty");
        }
        if (request.CommandType != TerminalCommandType.Next && (request.Tokens?.Length ?? 0) == 0)
        {
            return BadRequest($"{nameof(request.Tokens)} must not be empty");
        }
        if (request.GameAccountVersion == null)
        {
            return BadRequest($"{nameof(request.GameAccountVersion)} must not be empty");
        }

        var locale = this.GetLocale();

        try
        {
            var gameAccount = await mediator.Send(new GetGameAccountRequest(
                request.GameAccountId!,
                locale,
                ticket,
                request.GameAccountVersion!));

            var gameContext = new GameContext(gameAccount, locale);

            var result = await mediator.Send(new PerformCommandRequest(
                gameContext,
                request.ChapterId!,
                request.Tokens ?? Array.Empty<string>(),
                request.CommandType ?? throw new Exception(),
                ticket));

            return result switch
            {
                { IsSuccess: true } => Ok(ToWire(result.Value)),

                _ => BadRequest(new { message = string.Join(", ", result.Errors.Select(x => x.Message)) })
            };
        }
        catch (ConcurrencyException exception)
        {
            throw; // TODO (Roman): handle this, return latest autosave instead
        }
    }

    private static object? ToWire(PerformCommandResult record) => new
    {
        record.ActionType,
        record.Message,
        record.MessageType,
        GameAccount = record.GameAccount?.ToWire(),
        NextChapter = record.NextChapter?.ToWire(record.ForwardParagraphs)
    };
}

public record PostCommandRequest(
    [Required] string? GameAccountId,
    [Required] long? GameAccountVersion,
    [Required] string? ChapterId,
    [Required] TerminalCommandType? CommandType,
    string[]? Tokens);