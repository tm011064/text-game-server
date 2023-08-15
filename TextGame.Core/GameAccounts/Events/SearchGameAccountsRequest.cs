using MediatR;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.GameAccounts;

namespace TextGame.Core.GameAccounts.Events;

public record SearchGameAccountsRequest(
    string? UserKey,
    string? GameKey,
    string Locale,
    AuthTicket Ticket) : IRequest<IReadOnlyCollection<GameAccount>>;

public class SearchGameAccountsRequestHandler : IRequestHandler<SearchGameAccountsRequest, IReadOnlyCollection<GameAccount>>
{
    private readonly GameStateSerializer serializer;

    private readonly IQueryService queryService;

    public SearchGameAccountsRequestHandler(GameStateSerializer serializer, IQueryService queryService)
    {
        this.serializer = serializer;
        this.queryService = queryService;
    }

    public async Task<IReadOnlyCollection<GameAccount>> Handle(SearchGameAccountsRequest request, CancellationToken cancellationToken)
    {
        var records = await queryService.Run(
            new SearchGameAccounts(
                userKey: request.UserKey,
                gameKey: request.GameKey),
            request.Ticket);

        return records
            .Select(x => new GameAccount(
                x.Id,
                x.Key,
                serializer.Deserialize(x.GameStateJson, request.Locale).ToArray(),
                x.UserAccountId,
                x.GameId,
                x.Version))
            .ToArray();
    }
}
