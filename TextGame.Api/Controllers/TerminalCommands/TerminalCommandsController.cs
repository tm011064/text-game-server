using Microsoft.AspNetCore.Mvc;
using TextGame.Core;
using TextGame.Core.TerminalCommands;
using TextGame.Data.Contracts.TerminalCommands;

namespace TextGame.Api.Controllers.TerminalCommands;

[ApiController]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class TerminalCommandsController : ControllerBase
{
    private readonly ITerminalCommandProvider provider;

    public TerminalCommandsController(ITerminalCommandProvider provider)
    {
        this.provider = provider;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] PostSearchRequest request)
    {
        var records = await provider.Get(request.Locale ?? GameSettings.DefaultLocale);

        return Ok(records.Values.Select(ToWire).ToArray());
    }

    private object ToWire(TerminalCommand record) => new
    {
        Id = record.Key,
        record.Terms
    };
}

public record PostSearchRequest(string? Locale);