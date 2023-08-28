using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TextGame.Api.Auth;
using TextGame.Api.Controllers.Chapters;
using TextGame.Api.Controllers.GameAccounts;
using TextGame.Core;
using TextGame.Core.Challenges.Events;
using TextGame.Core.GameAccounts.Events;
using TextGame.Core.Games;
using TextGame.Data;

namespace TextGame.Api.Controllers.TerminalCommands;

[ApiController]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class ChallengesController : ControllerBase
{
    private readonly IMediator mediator;

    public ChallengesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [Authorize(Policy.HasGameAccount)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostChallengeRequest request)
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
        if (request.GameAccountVersion == null)
        {
            return BadRequest($"{nameof(request.GameAccountVersion)} must not be empty");
        }

        var locale = this.GetLocale();

        try
        {
            var gameAccount = await mediator.Send(new GetGameAccountRequest(
                request.GameAccountId!,
                ticket,
                request.GameAccountVersion!));

            var gameContext = new GameContext(gameAccount, locale);

            var result = await mediator.Send(new ChallengeSucceededRequest(
                gameContext,
                request.ChapterId!,
                ticket));

            return result switch
            {
                { IsSuccess: true } => Ok(ToWire(result.Value, locale)),

                _ => BadRequest(new { message = string.Join(", ", result.Errors.Select(x => x.Message)) })
            };
        }
        catch (ConcurrencyException exception)
        {
            throw; // TODO (Roman): handle this, return latest autosave instead
        }
    }

    private static object? ToWire(ChallengeSucceededResult record, string locale) => new
    {
        NextChapter = record.NextChapter?.ToWire(locale, record.ForwardParagraphs),
        record.LocalizedChallenges.Get(locale)!.SuccessMessage,
        GameAccount = record.GameAccount.ToWire(locale)
    };
}

public record PostChallengeRequest(
    [Required] string? GameAccountId,
    [Required] long? GameAccountVersion,
    [Required] string? ChapterId);