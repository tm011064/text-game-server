using MediatR;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Chapters;

namespace TextGame.Core.TerminalCommands.Events;

public record PerformCommandRequest(string gameKey, string chapterKey, string[] Tokens, AuthTicket Ticket) : IRequest<PerformCommandResult>;

public record PerformCommandResult();

public class PerformCommandRequestHandler : IRequestHandler<PerformCommandRequest, PerformCommandResult>
{
    private readonly IQueryService queryService;

    private readonly Rfc2898PasswordEncryptor encryptor = new();

    public PerformCommandRequestHandler(IQueryService queryService)
    {
        this.queryService = queryService;
    }

    public async Task<PerformCommandResult> Handle(PerformCommandRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<NavigationCommandType>(request.Tokens.First(), out var chapterCommand))
        {
            throw new InvalidOperationException("Command not found");
        }

        throw new Exception();
    }
}