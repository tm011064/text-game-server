using Microsoft.AspNetCore.Mvc;
using TextGame.Core.TerminalCommands;
using TextGame.Data.Contracts;

namespace TextGame.Api.Controllers.Commands
{
    [ApiController]
    [ApiVersion("20220718")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ILogger<CommandsController> logger;

        private readonly ITerminalCommandProvider provider;

        public CommandsController(ILogger<CommandsController> logger, ITerminalCommandProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var records = await provider.Get();

            return Ok(records.Select(ToWire).ToArray());
        }

        private object ToWire(TerminalCommand record) => new
        {
            Id = record.Key,
            record.Terms
        };
    }
}