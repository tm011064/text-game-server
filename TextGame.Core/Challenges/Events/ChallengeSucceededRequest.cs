using FluentResults;
using MediatR;
using TextGame.Core.Chapters;
using TextGame.Core.GameAccounts;
using TextGame.Core.Games;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Queries.GameAccounts;
using TextGame.Data.Sources.ResourceFiles;

namespace TextGame.Core.Challenges.Events;

public record ChallengeSucceededRequest(
    GameContext GameContext,
    string ChapterKey,
    AuthTicket Ticket) : IRequest<Result<ChallengeSucceededResult>>;

public record ChallengeSucceededResult(IChapter NextChapter, GameAccount GameAccount, string SuccessMessage)
{
    public LocalizedContentProvider<IReadOnlyCollection<Paragraph>> ForwardParagraphs { get; init; } = LocalizedContentProvider<IReadOnlyCollection<Paragraph>>.Empty;
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
        var chapter = await chapterProvider.GetChapter(request.ChapterKey);

        var nextChapter = await chapterProvider.GetChapter(
            request.GameContext.Game.GetCompositeChapterKey(chapter.Challenge!.ChapterKey));

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
            request.GameContext.Game.GetCompositeChapterKey(nextChapter.ForwardChapterKey));

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
            ForwardParagraphs = nextChapter.ParagraphsByLocale
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

        return await gameAccountConverter.Convert(record, request.GameContext.Locale);
    }
}