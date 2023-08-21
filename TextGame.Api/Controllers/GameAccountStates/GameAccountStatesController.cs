using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TextGame.Api.Auth;
using TextGame.Core.Chapters;
using TextGame.Core.GameAccounts;
using TextGame.Core.GameAccounts.Events;
using TextGame.Data;
using TextGame.Data.Queries.GameAccounts;

namespace TextGame.Api.Controllers.GameAccountStates;

[ApiController]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class GameAccountStatesController : ControllerBase
{
    private readonly IMediator mediator;

    private readonly IQueryService queryService;

    public GameAccountStatesController(IMediator mediator, IQueryService queryService)
    {
        this.mediator = mediator;
        this.queryService = queryService;
    }

    [Authorize(Policy.CanManageUsers)]
    [HttpPatch()]
    public async Task<IActionResult> Search([FromBody] PatchGameAccountStateRequest request)
    {
        var ticket = this.GetTicket();
        var locale = this.GetLocale();

        var gameAccount = await queryService.Run(
            GetGameAccount.ByKey(request.GameAccountId!),
            ticket) ?? throw new ResourceNotFoundException();

        if (gameAccount.Version != request.Version)
        {
            throw new ConcurrencyException();
        }

        var records = await mediator.Send(new UpdateGameAccountStateRequest(, locale, ticket));

        var wired = records.SelectMany(gameAccount =>
            gameAccount.GameStates.Select(gameState => ToWire(gameState, gameAccount)));

        return Ok(wired.ToArray());
    }

    [Authorize(Policy.CanManageUsers)]
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] PostGameAccountStatesSearchRequest request)
    {
        var ticket = this.GetTicket();
        var locale = this.GetLocale();

        var records = await mediator.Send(new SearchGameAccountsRequest(request.UserId, GameKey: null, locale, ticket));

        var wired = records.SelectMany(gameAccount =>
            gameAccount.GameStates.Select(gameState => ToWire(gameState, gameAccount)));

        return Ok(wired.ToArray());
    }

    private static object ToWire(GameState gameState, GameAccount gameAccount) => new
    {
        Id = gameState.Key,
        CurrentChapterId = gameState.CurrentChapter.GetCompositeKey(),
        gameState.SlotName,
        IsAutoSave = gameState.IsAutoSave(),
        gameState.UpdatedAt,
        VisitedChapterIds = gameState.VisitedChapters.Select(x => x.GetCompositeKey()).ToArray(),
        CompletedChallengeIds = gameState.CompletedChallenges.Select(x => x.GetCompositeKey()).ToArray(),

        gameAccountId = gameAccount.Key,
        gameAccountVersion = gameAccount.Version,
        GameId = gameAccount.Game.Key,
        UserAccountId = gameAccount.UserAccountKey,
        UserId = gameAccount.UserKey,
    };
}

public record PatchGameAccountStateRequest(
    [Required] string? GameAccountId,
    [Required] long? Version,
    [Required] string? GameStateId,
    string? CurrentChapterId,
    string? VisitedChapterIds,
    string? CompletedChallengeIds);

public record PostGameAccountStatesSearchRequest(string? UserId);
