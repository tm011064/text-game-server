using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TextGame.Api.Auth;
using TextGame.Api.Controllers.Users.Models;
using TextGame.Core;
using TextGame.Core.Users.Events;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.Users;

namespace TextGame.Api.Controllers.Users;

[ApiController]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
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
        var ticket = new AuthTicket(DateTimeOffset.UtcNow, AnonymousUserIdentity.Instance.Key);

        var record = await mediator.Send(new CreateUserRequest(request.Email!, request.Password!, new[] { UserRole.User }.ToHashSet(), ticket));

        return Ok(ToWire(record));
    }

    [Authorize(Policy.CanManageUsers)]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var ticket = this.GetTicket();

        var user = await queryService.Run(GetUser.ByKey(id), ticket) ?? throw new ResourceNotFoundException();

        return Ok(ToWire(user));
    }

    [Authorize(Policy.CanManageUsers)]
    [HttpPost("search")]
    public async Task<IActionResult> Search(PostUserSearchRequest request)
    {
        var ticket = this.GetTicket();

        if (Guid.TryParse(request.Text, out var _))
        {
            var record = await queryService.Run(GetUser.ByKey(request.Text), ticket);

            return record == null
                ? Ok()
                : Ok(new[] { ToWire(record) });
        }

        var records = await queryService.Run(new SearchUsers(request.Text, request.Limit), ticket);

        return Ok(records.Select(ToWire).ToArray());
    }

    private static object ToWire(IUser record) => new
    {
        Id = record.Key,
        record.Email
    };
}
