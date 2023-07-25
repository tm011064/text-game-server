namespace TextGame.Api.Controllers.Users;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using TextGame.Api.Auth;
using TextGame.Api.Controllers.Users.Models;
using TextGame.Core.Events.Users;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.Users;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IQueryService queryService;
    private readonly IMediator mediator;

    public UsersController(IQueryService queryService, IMediator mediator)
    {
        this.queryService = queryService;
        this.mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost()]
    public async Task<IActionResult> Post(PostUserRequest request)
    {
        var ticket = new AuthTicket(DateTimeOffset.UtcNow, string.Empty);

        var record = await mediator.Send(new CreateUserRequest(request.Email!, request.Password!, ticket));

        return Ok(ToWire(record));
    }

    [HttpGet("/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await queryService.Run(GetUser.ByKey(id));

        return Ok(ToWire(user));
    }

    private static object ToWire(IUser record) => new
    {
        Id = record.Key,
        record.Email
    };
}
