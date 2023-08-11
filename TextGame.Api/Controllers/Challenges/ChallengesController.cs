using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TextGame.Api.Auth;
using TextGame.Api.Controllers.Chapters;
using TextGame.Core;
using TextGame.Core.Challenges.Events;
using TextGame.Core.Games;
using TextGame.Data;

namespace TextGame.Api.Controllers.TerminalCommands;

[ApiController]
[Authorize]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class ChallengesController : ControllerBase
{
    private readonly IGameProvider gameProvider;

    private readonly IMediator mediator;

    public ChallengesController(IGameProvider gameProvider, IMediator mediator)
    {
        this.gameProvider = gameProvider;
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostChallengeRequest request)
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

        var game = await gameProvider.Get(request.GameId!);

        var result = await mediator.Send(new ChallengeSucceededRequest(
            new GameContext(game, GameSettings.DefaultLocale),
            request.ChapterId!,
            ticket));

        return result switch
        {
            { IsSuccess: true } => Ok(ToWire(result.Value)),

            _ => BadRequest(new { message = string.Join(", ", result.Errors.Select(x => x.Message)) })
        };
    }

    private static object? ToWire(ChallengeSucceededResult record) => new
    {
        NextChapter = record.NextChapter?.ToWire(record.ForwardParagraphs)
    };
}

public record PostChallengeRequest(
    [Required] string? GameId,
    [Required] string? ChapterId,
    [Required] string? Locale);