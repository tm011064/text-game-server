using FluentResults;
using MediatR;
using TextGame.Core.Chapters;
using TextGame.Core.GameAccounts;
using TextGame.Core.Games;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Contracts.TerminalCommands;
using TextGame.Data.Queries.GameAccounts;

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
        IReadOnlyCollection<Paragraph> forwardParagraphs,
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

    public IReadOnlyCollection<Paragraph> ForwardParagraphs { get; init; } = Array.Empty<Paragraph>();
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
        var chapter = await chapterProvider.GetChapter(request.ChapterKey, request.GameContext.Locale);

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

    private bool DoTokensMatch(IEnumerable<string> tokens, TerminalCommand terminalCommand, string commandText)
    {
        return tokens.Any(
            token => terminalCommand.Terms.Any(
                term => string.Equals(string.Concat(term, " ", token), commandText, StringComparison.OrdinalIgnoreCase)));
    }

    private async Task<Result<PerformCommandResult>> HandleChangeChapterRequest(PerformCommandRequest request, IChapter chapter)
    {
        var terminalCommands = await terminalCommandProvider.Get(request.GameContext.Locale);
        var terminalCommand = terminalCommands.GetOrNotFound(request.CommandType);
        var commandText = string.Join(" ", request.Tokens);

        var navigationCommand = chapter.NavigationCommands
            .Where(x => x.Type == request.CommandType)
            .Where(x => !x.Tokens.Any() || DoTokensMatch(x.Tokens, terminalCommand, commandText))
            .FirstOrDefault();

        if (navigationCommand == null)
        {
            return Result.Fail("Command not understood"); // TODO (Roman): needs list of responses from json locale
        }

        var nextChapter = await chapterProvider.GetChapter(
            request.GameContext.Game.GetCompositeChapterKey(navigationCommand.ChapterKey),
            request.GameContext.Locale);

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
            request.GameContext.Game.GetCompositeChapterKey(nextChapter.ForwardChapterKey),
            request.GameContext.Locale);

        return Result.Ok(PerformCommandResult.ForwardChapter(
            nextChapter.Paragraphs,
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

        return gameAccountConverter.Convert(record, request.GameContext.Locale);
    }
}