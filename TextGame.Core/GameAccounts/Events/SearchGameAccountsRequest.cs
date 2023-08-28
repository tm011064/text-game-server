using MediatR;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.GameAccounts;

namespace TextGame.Core.GameAccounts.Events;

public record SearchGameAccountsRequest(
    string? UserKey,
    string? GameKey,
    AuthTicket Ticket) : IRequest<IReadOnlyCollection<GameAccount>>;

public class SearchGameAccountsRequestHandler : IRequestHandler<SearchGameAccountsRequest, IReadOnlyCollection<GameAccount>>
{
    private readonly GameAccountConverter converter;

    private readonly IQueryService queryService;

    public SearchGameAccountsRequestHandler(GameAccountConverter converter, IQueryService queryService)
    {
        this.converter = converter;
        this.queryService = queryService;
    }

    public async Task<IReadOnlyCollection<GameAccount>> Handle(SearchGameAccountsRequest request, CancellationToken cancellationToken)
    {
        var records = await queryService.Run(
            new SearchGameAccounts(
                userKey: request.UserKey,
                gameKey: request.GameKey),
            request.Ticket);

        return await Task.WhenAll(records.Select(converter.Convert));
    }
}
