using FluentValidation;
using MediatR;
using TextGame.Core.Chapters;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.GameAccounts;

namespace TextGame.Core.GameAccounts.Events;

public record UpdateGameAccountStateRequest(
    GameAccount GameAccount,
    string GameStateKey,
    string? CurrentChapterKey,
    HashSet<string>? VisitedChapterKeys,
    HashSet<string>? CompletedChallengeKeys,
    string Locale, // TODO (Roman): move locale inside of chapter, it is not needed here
    AuthTicket Ticket) : IRequest<GameAccount>;

public class UpdateGameAccountStateRequestValidator : AbstractValidator<UpdateGameAccountStateRequest>
{
    public UpdateGameAccountStateRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => !x.CurrentChapterKey.IsNullOrWhitespace()
                || x.CompletedChallengeKeys != null
                || x.VisitedChapterKeys != null)
            .WithMessage("At least one updateable property must be set");

        RuleForEach(x => x.CompletedChallengeKeys)
            .NotEmpty()
            .When(x => x.CompletedChallengeKeys != null);

        RuleForEach(x => x.VisitedChapterKeys)
            .NotEmpty()
            .When(x => x.VisitedChapterKeys != null);

        RuleFor(x => x.GameStateKey)
            .Must((record, value) => record.GameAccount.GameStates.Any(x => x.Key == value))
            .WithMessage(x => $"{nameof(x.GameStateKey)} not found");
    }
}

public class UpdateGameAccountStateRequestHandler : IRequestHandler<UpdateGameAccountStateRequest, GameAccount>
{
    private readonly GameStateCollectionBuilderFactory builderFactory;

    private readonly GameAccountConverter converter;

    private readonly IQueryService queryService;

    private readonly IChapterProvider chapterProvider;

    private readonly IValidator<UpdateGameAccountStateRequest> validator;

    public UpdateGameAccountStateRequestHandler(
        GameStateCollectionBuilderFactory builderFactory,
        IQueryService queryService,
        IValidator<UpdateGameAccountStateRequest> validator,
        IChapterProvider chapterProvider,
        GameAccountConverter converter)
    {
        this.builderFactory = builderFactory;
        this.queryService = queryService;
        this.validator = validator;
        this.chapterProvider = chapterProvider;
        this.converter = converter;
    }

    public async Task<GameAccount> Handle(UpdateGameAccountStateRequest request, CancellationToken cancellationToken)
    {
        var chapterKeys = (request.CompletedChallengeKeys ?? Enumerable.Empty<string>())
            .Concat(request.VisitedChapterKeys ?? Enumerable.Empty<string>())
            .Concat(new[] { request.CurrentChapterKey })
            .Where(x => !x.IsNullOrWhitespace())
            .Select(x => x!);

        var chapters = chapterProvider.GetChaptersMap(chapterKeys.ToHashSet(), request.Locale);

        await validator.ValidateAndThrowAsync(request);

        var json = builderFactory.Create(request.GameAccount)
            .Replace(
                x => x.Key == request.GameStateKey,
                gameState => gameState with
                {
                    CurrentChapter = request.CurrentChapterKey
                        ?.Let(chapters.GetOrNotFound)
                        ?? gameState.CurrentChapter,

                    CompletedChallenges = request.CompletedChallengeKeys
                        ?.Let(x => x.Select(chapters.GetOrNotFound).ToHashSet())
                        ?? gameState.CompletedChallenges,

                    VisitedChapters = request.VisitedChapterKeys
                        ?.Let(x => x.Select(chapters.GetOrNotFound).ToHashSet())
                        ?? gameState.VisitedChapters
                })
            .Build();

        var gameAccount = await queryService.Run(
            new UpdateGameAccountGameState(request.GameAccount.Id, request.GameAccount.Version, json),
            request.Ticket);

        return await converter.Convert(gameAccount, request.Locale);
    }
}
