using MediatR;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.Users;

namespace TextGame.Core.Users.Events;

public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, IUser>
{
    private readonly IQueryService queryService;

    private readonly Rfc2898PasswordEncryptor encryptor = new();

    public CreateUserRequestHandler(IQueryService queryService)
    {
        this.queryService = queryService;
    }

    public async Task<IUser> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var password = encryptor.Encrypt(request.Password);

        var id = await queryService.Run(new InsertUser(
            key: Guid.NewGuid().ToString(),
            email: request.Email,
            password: password,
            ticket: request.Ticket));

        return await queryService.Run(GetUser.ById(id));
    }
}