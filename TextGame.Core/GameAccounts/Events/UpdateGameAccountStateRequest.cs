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
    public UpdateGameAccountStateRequestValidator(IChapterProvider provider)
    {
        RuleFor(x => x)
            .Must(x => !x.CurrentChapterKey.IsNullOrWhitespace()
                || x.CompletedChallengeKeys != null
                || x.VisitedChapterKeys != null)
            .WithMessage("At least one updateable property must be set");

        RuleForEach(x => x.CurrentChapterKey)
            .NotEmpty()
            .When(x => x.CurrentChapterKey.NotEmpty());


        RuleForEach(x => x.CompletedChallengeKeys)
            .Must((x, value) => provider.Exists(value, x.Locale))
            .When(x => x.CurrentChapterKey.NotEmpty());

        //RuleFor(x => x.CompletedChallengeKeys)
        //    .
        //    .When(x => x.CompletedChallengeKeys != null)
        //    .WithMessage(x => $"ChapterKey '{x.CurrentChapterKey}' not found");
    }
}

public class UpdateGameAccountStateRequestHandler : IRequestHandler<UpdateGameAccountStateRequest, GameAccount>
{
    private readonly GameAccountConverter converter;

    private readonly IQueryService queryService;

    private readonly IChapterProvider chapterProvider;

    private readonly IValidator<UpdateGameAccountStateRequest> validator;

    public UpdateGameAccountStateRequestHandler(GameAccountConverter converter, IQueryService queryService, IValidator<UpdateGameAccountStateRequest> validator, IChapterProvider chapterProvider)
    {
        this.converter = converter;
        this.queryService = queryService;
        this.validator = validator;
        this.chapterProvider = chapterProvider;
    }

    public async Task<GameAccount> Handle(UpdateGameAccountStateRequest request, CancellationToken cancellationToken)
    {
        var chapterKeys = (request.CompletedChallengeKeys ?? Enumerable.Empty<string>())
            .Concat(request.VisitedChapterKeys ?? Enumerable.Empty<string>())
            .Concat(new[] { request.CurrentChapterKey })
            .Where(x => !x.IsNullOrWhitespace());

        var chapters = chapterProvider.GetChaptersMap(chapterKeys.ToHashSet(), GameSettings)


        await validator.ValidateAndThrowAsync(request);



        var records = await queryService.Run(
            new SearchGameAccounts(
                userKey: request.UserKey,
                gameKey: request.GameKey),
            request.Ticket);

        return await Task.WhenAll(records.Select(x => converter.Convert(x, request.Locale)));
    }
}
