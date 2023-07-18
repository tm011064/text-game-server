using Microsoft.AspNetCore.Mvc;

namespace TextGame.Api.Controllers.Rooms
{
    [ApiController]
    [Route("rooms")]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> logger;

        public RoomController(ILogger<RoomController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "WOHOO";
        }
    }
}