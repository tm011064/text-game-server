using MediatR;
using TextGame.Core.Cryptography;
using TextGame.Core.Users.Events;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.Users;
using TextGame.Data.Resources;

namespace TextGame.Core.Setup;

public class SeedDataService
{
    private readonly IQueryService queryService;

    private readonly IMediator mediator;

    private readonly Rfc2898PasswordValidator passwordValidator = new();

    private readonly Rfc2898PasswordEncryptor passwordEncryptor = new();

    public SeedDataService(IQueryService queryService, IMediator mediator)
    {
        this.queryService = queryService;
        this.mediator = mediator;
    }

    public async Task InsertResourceFileGamesIfNotExist()
    {
        var ticket = CreateTicket();

        foreach (var gameKey in ResourceService.GameKeys)
        {
            await queryService.Run(new InsertGameIfNotExists(gameKey, ticket));
        }
    }

    public async Task CreateTestUserIfNotExists(string email, string password)
    {
        var ticket = CreateTicket();
        var existing = await queryService.Run(GetUser.ByEmail(email));

        if (existing == null)
        {
            await mediator.Send(new CreateUserRequest(email, password, ticket));
            return;
        }

        var userPassword = await queryService.Run(new GetUserPassword(existing.Id));

        if (!passwordValidator.IsValid(password, userPassword))
        {
            await queryService.Run(new UpdateUserPassword(existing.Id, passwordEncryptor.Encrypt(password), ticket));
        }
    }

    private static AuthTicket CreateTicket()
    {
        return new AuthTicket(DateTimeOffset.UtcNow, SystemUserIdentity.Instance);
    }

    private record SystemUserIdentity : IUserIdentity
    {
        public static readonly SystemUserIdentity Instance = new();

        public string Key { get; init; } = "system";
    }
}

