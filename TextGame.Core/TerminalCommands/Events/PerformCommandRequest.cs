using FluentResults;
using MediatR;
using TextGame.Core.Chapters;
using TextGame.Core.GameAccounts;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Contracts.Games;
using TextGame.Data.Contracts.TerminalCommands;
using TextGame.Data.Queries.GameAccounts;
using TextGame.Data.Sources;

namespace TextGame.Core.TerminalCommands.Events;

public record PerformCommandRequest(
    GameContext GameContext,
    string ChapterKey,
    string[] Tokens,
    TerminalCommandType CommandType,
    AuthTicket Ticket) : IRequest<Result<PerformCommandResult>>;

public record PerformCommandResult(
    CommandResultActionType ActionType,
    IChapter? NextChapter = null,
    GameAccount? GameAccount = null,
    string? Message = null,
    CommandResultMessageType? MessageType = null)
{
    public static PerformCommandResult ChangeChapter(IChapter chapter, GameAccount gameAccount) => new(
        CommandResultActionType.ChangeChapter,
        chapter,
        gameAccount);

    public static PerformCommandResult ForwardChapter(
        LocalizedContentProvider<IReadOnlyCollection<Paragraph>> forwardParagraphs,
        IChapter chapter,
        GameAccount gameAccount) => new(CommandResultActionType.ChangeChapter, chapter, gameAccount)
        {
            ForwardParagraphs = forwardParagraphs
        };

    public static PerformCommandResult Error(string message) => new(
        CommandResultActionType.ShowMessage,
        Message: message,
        MessageType: CommandResultMessageType.Error);

    public static PerformCommandResult Info(string message) => new(
        CommandResultActionType.ShowMessage,
        Message: message,
        MessageType: CommandResultMessageType.Info);

    public LocalizedContentProvider<IReadOnlyCollection<Paragraph>> ForwardParagraphs { get; init; } = LocalizedContentProvider<IReadOnlyCollection<Paragraph>>.Empty;
}

public enum CommandResultMessageType
{
    Error,
    Info
}

public enum CommandResultActionType
{
    ShowMessage,
    ChangeChapter
}

public class PerformCommandRequestHandler : IRequestHandler<PerformCommandRequest, Result<PerformCommandResult>>
{
    private readonly IChapterProvider chapterProvider;

    private readonly ITerminalCommandProvider terminalCommandProvider;

    private readonly IQueryService queryService;

    private readonly GameStateCollectionBuilderFactory gameStateCollectionBuilderFactory;

    private readonly GameAccountConverter gameAccountConverter;

    public PerformCommandRequestHandler(
        IChapterProvider chapterProvider,
        IQueryService queryService,
        GameStateCollectionBuilderFactory gameStateCollectionBuilderFactory,
        GameAccountConverter gameAccountConverter,
        ITerminalCommandProvider terminalCommandProvider)
    {
        this.chapterProvider = chapterProvider;
        this.queryService = queryService;
        this.gameStateCollectionBuilderFactory = gameStateCollectionBuilderFactory;
        this.gameAccountConverter = gameAccountConverter;
        this.terminalCommandProvider = terminalCommandProvider;
    }

    public async Task<Result<PerformCommandResult>> Handle(PerformCommandRequest request, CancellationToken cancellationToken)
    {
        var chapter = await chapterProvider.GetChapter(request.ChapterKey);

        // TODO (Roman):  validate command type and token match

        return request.CommandType switch
        {
            TerminalCommandType.Enter or
            TerminalCommandType.Next or
            TerminalCommandType.Move or
            TerminalCommandType.Decline or
            TerminalCommandType.Confirm => await HandleChangeChapterRequest(request, chapter),

            _ => throw new NotImplementedException(request.CommandType.ToString())
        };
    }

    private static bool DoTokensMatch(IEnumerable<string> tokens, TwoWayLookup<TerminalCommandType, string> terminalCommandMap, string commandText)
    {
        return tokens.Any(
            token => terminalCommandMap.GetValues().Any(
                term => string.Equals(string.Concat(term, " ", token), commandText, StringComparison.OrdinalIgnoreCase)));
    }

    private async Task<Result<PerformCommandResult>> HandleChangeChapterRequest(PerformCommandRequest request, IChapter chapter)
    {
        var terminalCommands = terminalCommandProvider.Get(request.GameContext.Locale);
        var commandText = string.Join(" ", request.Tokens);

        var navigationCommand = chapter.LocalizedNavigationCommands.Get(request.GameContext.Locale)!
            .Where(x => x.Type == request.CommandType)
            .Where(x => !x.Tokens.Any() || DoTokensMatch(x.Tokens, terminalCommands, commandText))
            .FirstOrDefault();

        if (navigationCommand == null)
        {
            return Result.Ok(PerformCommandResult.Error("Don't understand")); // TODO (Roman): needs list of responses from json locale
        }

        var nextChapter = await chapterProvider.GetChapter(
            request.GameContext.Game.GetCompositeKey(navigationCommand.ChapterKey));

        var gameStateBuilder = gameStateCollectionBuilderFactory.Create(request.GameContext.GameAccount)
            .Replace(
                x => x.IsAutoSave(),
                x => x.WithVisitedChapter(chapter)
                    with
                {
                    CurrentChapter = nextChapter,
                    UpdatedAt = request.Ticket.CreatedAt
                });

        if (nextChapter.ForwardChapterKey == null)
        {
            return Result.Ok(PerformCommandResult.ChangeChapter(
                nextChapter,
                await UpdateGameAccount(request, gameStateBuilder)));
        }

        var forwardChapter = await chapterProvider.GetChapter(
            request.GameContext.Game.GetCompositeKey(nextChapter.ForwardChapterKey));

        gameStateBuilder = gameStateBuilder.Replace(
            x => x.IsAutoSave(),
            x => x.WithVisitedChapter(nextChapter) with
            {
                CurrentChapter = forwardChapter,
                UpdatedAt = request.Ticket.CreatedAt
            });

        return Result.Ok(PerformCommandResult.ForwardChapter(
            nextChapter.LocalizedParagraphs,
            forwardChapter,
            await UpdateGameAccount(request, gameStateBuilder)));
    }

    private async Task<GameAccount> UpdateGameAccount(
        PerformCommandRequest request,
        GameStateCollectionBuilder gameStateBuilder)
    {
        var record = await queryService.Run(
            new UpdateGameAccountGameState(
                request.GameContext.GameAccount.Id,
                request.GameContext.GameAccount.Version,
                gameStateBuilder.Build()),
            request.Ticket);

        return await gameAccountConverter.Convert(record);
    }
}