using FluentResults;
using MediatR;
using System.Net.Sockets;
using TextGame.Core.Chapters;
using TextGame.Core.GameAccounts;
using TextGame.Core.Games;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Contracts.Games;
using TextGame.Data.Queries.GameAccounts;

namespace TextGame.Core.Challenges.Events;

public record ChallengeSucceededRequest(
    GameContext GameContext,
    string ChapterKey,
    AuthTicket Ticket) : IRequest<Result<ChallengeSucceededResult>>;

public record ChallengeSucceededResult(IChapter NextChapter, GameAccount GameAccount, string SuccessMessage)
{
    public IReadOnlyCollection<Paragraph> ForwardParagraphs { get; init; } = Array.Empty<Paragraph>();
}

public class ChallengeSucceededRequestHandler : IRequestHandler<ChallengeSucceededRequest, Result<ChallengeSucceededResult>>
{
    private readonly IChapterProvider chapterProvider;

    private readonly IQueryService queryService;

    private readonly GameStateCollectionBuilderFactory gameStateCollectionBuilderFactory;

    private readonly GameAccountConverter gameAccountConverter;

    public ChallengeSucceededRequestHandler(
        IChapterProvider chapterProvider,
        GameStateCollectionBuilderFactory gameStateCollectionBuilderFactory,
        IQueryService queryService,
        GameAccountConverter gameAccountConverter)
    {
        this.chapterProvider = chapterProvider;
        this.gameStateCollectionBuilderFactory = gameStateCollectionBuilderFactory;
        this.queryService = queryService;
        this.gameAccountConverter = gameAccountConverter;
    }

    public async Task<Result<ChallengeSucceededResult>> Handle(ChallengeSucceededRequest request, CancellationToken cancellationToken)
    {
        var chapter = await chapterProvider.GetChapter(request.ChapterKey, request.GameContext.Locale);

        var nextChapter = await chapterProvider.GetChapter(
            request.GameContext.Game.GetCompositeChapterKey(chapter.Challenge!.ChapterKey),
            request.GameContext.Locale);

        var gameStateBuilder = gameStateCollectionBuilderFactory.Create(request.GameContext.GameAccount)
            .Replace(
                x => x.IsAutoSave(),
                x => x
                    .WithCompletedChallenge(chapter)
                    .WithVisitedChapter(chapter) with
                {
                    CurrentChapter = nextChapter,
                    UpdatedAt = request.Ticket.CreatedAt
                });

        if (nextChapter.ForwardChapterKey == null)
        {
            return Result.Ok(new ChallengeSucceededResult(
                nextChapter,
                await UpdateGameAccount(request, gameStateBuilder),
                chapter.Challenge!.SuccessMessage));
        }

        var forwardChapter = await chapterProvider.GetChapter(
            request.GameContext.Game.GetCompositeChapterKey(nextChapter.ForwardChapterKey),
            request.GameContext.Locale);

        gameStateBuilder = gameStateBuilder.Replace(
            x => x.IsAutoSave(),
            x => x.WithVisitedChapter(nextChapter) with
            {
                CurrentChapter = forwardChapter,
                UpdatedAt = request.Ticket.CreatedAt
            });

        var result = new ChallengeSucceededResult(
            forwardChapter,
            await UpdateGameAccount(request, gameStateBuilder),
            chapter.Challenge!.SuccessMessage)
        {
            ForwardParagraphs = nextChapter.Paragraphs
        };

        return Result.Ok(result);
    }

    private async Task<GameAccount> UpdateGameAccount(
        ChallengeSucceededRequest request,
        GameStateCollectionBuilder gameStateBuilder)
    {
        var record = await queryService.Run(
            new UpdateGameAccountGameState(
                request.GameContext.GameAccount.Id,
                request.GameContext.GameAccount.Version,
                gameStateBuilder.Build()),
            request.Ticket);

        return gameAccountConverter.Convert(record, request.GameContext.Locale);
    }
}