using Microsoft.AspNetCore.Mvc;
using TextGame.Core.Rooms;

namespace TextGame.Api.Controllers.Rooms
{
    [ApiController]
    [ApiVersion("20220718")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly ILogger<RoomsController> logger;


        public RoomsController(ILogger<RoomsController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public object Get()
        {
            return new { id = 1 };
        }

        //private static object ToWire(Room room)
        //{
        //    return new
        //    {
        //        id = room.id.ToString(),
        //        gameId = room.gameId.ToString(),
        //        emotionType = room.emotionType.ToString(),
        //        messages = room.messages.Select(ToWire).ToArray()
        //    };
        //}

        //private static object ToWire(RoomMessage x)
        //{
        //    return new
        //    {
        //        x.index,
        //        x.text
        //    };
        //}
    }
}