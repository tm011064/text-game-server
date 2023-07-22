namespace TextGame.Api.Controllers.Users;

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TextGame.Api.Auth;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.Users;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthenticator authenticator;

    private readonly IQueryService queryService;

    public UsersController(IAuthenticator authenticator, IQueryService queryService)
    {
        this.authenticator = authenticator;
        this.queryService = queryService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = authenticator.Authenticate(model);

        if (response == null)
            return BadRequest(new { message = "Username or password is incorrect" });

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost()]
    public async Task<IActionResult> Post(CreateUserRequest request)
    {
        var encryptor = new Rfc2898PasswordEncryptor();

        var password = encryptor.Encrypt(request.Password!);

        var key = Guid.NewGuid().ToString();

        var ticket = new AuthTicket(DateTimeOffset.UtcNow, key);

        var id = await queryService.Run(new InsertUser(key, request.Email!, password, ticket));

        var record = await queryService.Run(new GetUserById(id));

        return Ok(record); // towire
    }

    [AllowAnonymous]
    [HttpGet("/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await queryService.Run(new GetUserByKey(id));

        return Ok(user);
    }
}

public class CreateUserRequest
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}

public class AuthenticateResponse
{
    public int Id { get; set; }

    public string Token { get; set; }

    public AuthenticateResponse(IUser user, string token)
    {
        Id = user.Id;
        Token = token;
    }
}

public class AuthenticateRequest
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}