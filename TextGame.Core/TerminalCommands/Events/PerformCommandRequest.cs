using FluentResults;
using MediatR;
using TextGame.Core.Chapters;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Contracts.TerminalCommands;

namespace TextGame.Core.TerminalCommands.Events;

public record PerformCommandRequest(
    GameContext GameContext,
    string ChapterKey,
    string[] Tokens,
    TerminalCommandType CommandType,
    AuthTicket Ticket) : IRequest<Result<PerformCommandResult>>;

public record PerformCommandResult(
    CommandResultNavigationType NavigationType,
    IChapter NavigationChapter);

public enum CommandResultNavigationType
{
    Stay,
    Navigate
}


public class PerformCommandRequestHandler : IRequestHandler<PerformCommandRequest, Result<PerformCommandResult>>
{
    private readonly IQueryService queryService;

    private readonly IChapterProvider chapterProvider;

    private readonly ITerminalCommandProvider terminalCommandProvider;

    public PerformCommandRequestHandler(
        IQueryService queryService,
        IChapterProvider chapterProvider,
        ITerminalCommandProvider terminalCommandProvider)
    {
        this.queryService = queryService;
        this.chapterProvider = chapterProvider;
        this.terminalCommandProvider = terminalCommandProvider;
    }

    public async Task<Result<PerformCommandResult>> Handle(PerformCommandRequest request, CancellationToken cancellationToken)
    {
        var chapter = await chapterProvider.GetChapter(request.ChapterKey, request.GameContext.Locale);

        // TODO (Roman):  validate command type and token match

        var firstToken = request.Tokens.First();
        var terminalCommands = terminalCommandProvider.Get(request.GameContext.Locale);

        //foreach (var command in chapter.NavigationCommands)
        //{

        //}

        //if (!Enum.TryParse<TerminalCommandType>(request.Tokens.First(), out var chapterCommand))
        //{
        //    throw new InvalidOperationException("Command not found");
        //}

        throw new Exception();
    }
}