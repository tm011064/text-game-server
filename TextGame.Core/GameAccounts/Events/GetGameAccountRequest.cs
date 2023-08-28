using MediatR;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.GameAccounts;

namespace TextGame.Core.GameAccounts.Events;

public record GetGameAccountRequest(
    string Key,
    AuthTicket Ticket,
    long? Version = null) : IRequest<GameAccount>;

public class GetGameAccountRequestHandler : IRequestHandler<GetGameAccountRequest, GameAccount>
{
    private readonly GameAccountConverter converter;

    private readonly IQueryService queryService;

    public GetGameAccountRequestHandler(GameAccountConverter converter, IQueryService queryService)
    {
        this.converter = converter;
        this.queryService = queryService;
    }

    public async Task<GameAccount> Handle(GetGameAccountRequest request, CancellationToken cancellationToken)
    {
        var record = await queryService.Run(GetGameAccount.ByKey(request.Key), request.Ticket)
            ?? throw new ResourceNotFoundException();

        if (request.Version.HasValue && record.Version != request.Version)
        {
            throw new ConcurrencyException();
        }

        return await converter.Convert(record);
    }
}