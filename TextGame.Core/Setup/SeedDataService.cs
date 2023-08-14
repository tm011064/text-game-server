using MediatR;
using TextGame.Core.Cryptography;
using TextGame.Core.Users;
using TextGame.Core.Users.Events;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.GameAccounts;
using TextGame.Data.Queries.Games;
using TextGame.Data.Queries.UserAccounts;
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
        var ticket = AuthTicket.System;

        foreach (var gameKey in ResourceService.GameKeys)
        {
            await queryService.Run(new InsertGameIfNotExists(gameKey), ticket);
        }
    }

    public async Task CreateTestUserIfNotExists(string email, string password)
    {
        var ticket = AuthTicket.System;
        var existing = await queryService.Run(GetUser.ByEmail(email), ticket);

        if (existing == null)
        {
            await mediator.Send(new CreateUserRequest(email, password, ticket));
            return;
        }

        var userPassword = await queryService.Run(new GetUserPassword(existing.Id), ticket);

        if (!passwordValidator.IsValid(password, userPassword))
        {
            await queryService.Run(new UpdateUserPassword(existing.Id, passwordEncryptor.Encrypt(password)), ticket);
        }
    }

    public async Task CreateTestUserGameAccountsIfNotExists(string email)
    {
        var ticket = AuthTicket.System;
        var existing = await queryService.Run(GetUser.ByEmail(email), ticket) ?? throw new ResourceNotFoundException();

        var userAccountId = await queryService.Run(
            new InsertUserAccountIfNotExists(existing, Guid.NewGuid().ToString(), email),
            ticket);

        var userAccount = await queryService.Run(GetUserAccount.ById(userAccountId), ticket);

        var games = await queryService.Run(new SearchGames(), ticket);

        foreach (var game in games)
        {
            await queryService.Run(new InsertGameAccountIfNotExists(userAccount, game, Guid.NewGuid().ToString(), "{}"), ticket);
        }
    }
}

