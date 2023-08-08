﻿using FluentResults;
using MediatR;
using TextGame.Core.Chapters;
using TextGame.Core.Cryptography;
using TextGame.Core.Games;
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
    CommandResultActionType ActionType,
    IChapter? NextChapter = null,
    string? Message = null,
    CommandResultMessageType? MessageType = null)
{
    public static PerformCommandResult ChangeChapter(IChapter chapter) => new(CommandResultActionType.ChangeChapter, chapter);

    public static PerformCommandResult Error(string message) => new(CommandResultActionType.ShowMessage, Message: message, MessageType: CommandResultMessageType.Error);

    public static PerformCommandResult Info(string message) => new(CommandResultActionType.ShowMessage, Message: message, MessageType: CommandResultMessageType.Info);
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

        return request.CommandType switch
        {
            TerminalCommandType.Forward or
            TerminalCommandType.Move or
            TerminalCommandType.Decline or
            TerminalCommandType.Confirm => await HandleChangeChapterRequest(request, chapter),

            _ => throw new NotImplementedException(request.CommandType.ToString())
        };
    }

    private async Task<Result<PerformCommandResult>> HandleChangeChapterRequest(PerformCommandRequest request, IChapter chapter)
    {
        var navigationCommand = chapter.NavigationCommands.Single(x => x.Type == request.CommandType);

        var nextChapter = await chapterProvider.GetChapter(
            request.GameContext.Game.GetCompositeChapterKey(navigationCommand.ChapterKey),
            request.GameContext.Locale);

        return Result.Ok(PerformCommandResult.ChangeChapter(nextChapter));
    }
}