using FluentResults;
using MediatR;
using TextGame.Core.Chapters;
using TextGame.Core.Games;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Chapters;

namespace TextGame.Core.Challenges.Events;

public record ChallengeSucceededRequest(
    GameContext GameContext,
    string ChapterKey,
    AuthTicket Ticket) : IRequest<Result<ChallengeSucceededResult>>;

public record ChallengeSucceededResult(IChapter NextChapter)
{
    public IReadOnlyCollection<Paragraph> ForwardParagraphs { get; init; } = Array.Empty<Paragraph>();
}

public class ChallengeSucceededRequestHandler : IRequestHandler<ChallengeSucceededRequest, Result<ChallengeSucceededResult>>
{
    private readonly IChapterProvider chapterProvider;

    public ChallengeSucceededRequestHandler(IChapterProvider chapterProvider)
    {
        this.chapterProvider = chapterProvider;
    }

    public async Task<Result<ChallengeSucceededResult>> Handle(ChallengeSucceededRequest request, CancellationToken cancellationToken)
    {
        var chapter = await chapterProvider.GetChapter(request.ChapterKey, request.GameContext.Locale);

        var nextChapter = await chapterProvider.GetChapter(
            request.GameContext.Game.GetCompositeChapterKey(chapter.Challenge!.ChapterKey),
            request.GameContext.Locale);

        if (nextChapter.ForwardChapterKey != null)
        {
            var forwardChapter = await chapterProvider.GetChapter(
                request.GameContext.Game.GetCompositeChapterKey(nextChapter.ForwardChapterKey),
                request.GameContext.Locale);

            return Result.Ok(new ChallengeSucceededResult(forwardChapter)
            {
                ForwardParagraphs = nextChapter.Paragraphs
            });
        }

        return Result.Ok(new ChallengeSucceededResult(nextChapter));
    }
}