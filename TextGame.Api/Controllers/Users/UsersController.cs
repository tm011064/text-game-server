namespace TextGame.Api.Controllers.Users;

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.Users;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;

    private readonly IQueryService queryService;

    public UsersController(IUserService userService, IQueryService queryService)
    {
        _userService = userService;
        this.queryService = queryService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);

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

        var key = Guid.NewGuid();
        var ticket = new AuthTicket(DateTimeOffset.UtcNow, key.ToString());

        var id = await queryService.Run(new InsertUser(key, request.Email!, password, ticket));

        var record = await queryService.Run(new GetUserById(id));

        return Ok(record); // towire
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var identity = User.Identity;

        var users = _userService.GetAll();
        return Ok(users);
    }
}

public class CreateUserRequest
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}
