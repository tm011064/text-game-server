using MediatR;
using TextGame.Core.Chapters;
using TextGame.Core.Cryptography;
using TextGame.Core.GameAccounts;
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
    private readonly IChapterProvider chapterProvider;

    private readonly GameStateSerializer gameStateSerializer;

    private readonly IQueryService queryService;

    private readonly IMediator mediator;

    private readonly Rfc2898PasswordValidator passwordValidator = new();

    private readonly Rfc2898PasswordEncryptor passwordEncryptor = new();

    public SeedDataService(IQueryService queryService, IMediator mediator, IChapterProvider chapterProvider, GameStateSerializer gameStateSerializer)
    {
        this.queryService = queryService;
        this.mediator = mediator;
        this.chapterProvider = chapterProvider;
        this.gameStateSerializer = gameStateSerializer;
    }

    public async Task InsertResourceFileGamesIfNotExist()
    {
        var ticket = AuthTicket.System;

        foreach (var gameKey in ResourceService.GameKeys)
        {
            await queryService.Run(new InsertGameIfNotExists(gameKey), ticket);
        }
    }

    public async Task CreateTestUserIfNotExists(string email, string password, params string[] roles)
    {
        var ticket = AuthTicket.System;
        var existing = await queryService.Run(GetUser.ByEmail(email), ticket);

        if (existing == null)
        {
            await mediator.Send(new CreateUserRequest(email, password, roles.ToHashSet(), ticket));
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

        var userAccount = await queryService.Run(
            new InsertUserAccountIfNotExists(existing, Guid.NewGuid().ToString(), email),
            ticket);

        var games = await queryService.Run(new SearchGames(), ticket);

        foreach (var game in games)
        {
            var root = await chapterProvider.GetChapter($"{game.Key}-root");

            var defaultSaveSlot = GameState.New(root, "default", ticket);
            var autoSaveSlot = GameState.New(root, null, ticket);

            var gameStateJson = gameStateSerializer.Serialize(defaultSaveSlot, autoSaveSlot);

            await queryService.Run(new InsertGameAccountIfNotExists(userAccount, game, Guid.NewGuid().ToString(), gameStateJson), ticket);
        }
    }
}

