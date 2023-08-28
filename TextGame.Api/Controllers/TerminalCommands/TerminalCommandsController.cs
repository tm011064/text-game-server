using Microsoft.AspNetCore.Mvc;
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
    public IActionResult Search()
    {
        var records = provider.Get(this.GetLocale());

        return Ok(records.GetAll().Select(ToWire).ToArray());
    }

    private object ToWire(IGrouping<TerminalCommandType, string> record) => new
    {
        Id = record.Key,
        Terms = record
    };
}
