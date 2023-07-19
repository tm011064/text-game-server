using Microsoft.AspNetCore.Mvc;
using TextGame.Core.Rooms;
using TextGame.Data.Contracts;
using System.Linq;

namespace TextGame.Api.Controllers.Chapters
{
    [ApiController]
    [ApiVersion("20220718")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly ILogger<GamesController> logger;

        private readonly IChapterProvider chapterProvider;

        public GamesController(ILogger<GamesController> logger, IChapterProvider chapterProvider)
        {
            this.logger = logger;
            this.chapterProvider = chapterProvider;
        }

        [HttpGet("{gameId}/chapters/{chapterId}")]
        public async Task<IActionResult> GetChapter(
            [FromRoute] string gameId,
            [FromRoute] string chapterId)
        {
            var chapter = await chapterProvider.GetChapter(gameId, chapterId);

            return Ok(ToWire(chapter));
        }

        private object ToWire(Chapter record) => new
        {
            Id = record.Key,

            Commands = record.Commands.Select(ToWire).ToArray(),
            MessageGroups = record.MessageGroups.Select(ToWire).ToArray()
        };

        private object ToWire(MessageGroupMessage record) => new
        {
            record.Text
        };

        private object ToWire(MessageGroup record) => new
        {
            EmotionId = record.EmotionKey,

            Messages = record.Messages.Select(ToWire).ToArray()
        };

        private object ToWire(ChapterCommand record) => new
        {
            Type = record.Type.ToString(),
            Action = ToWire(record.Action)
        };

        private object ToWire(ChapterCommandAction record) => new
        {
            Type = record.Type.ToString(),
            ChapterId = record.ChapterKey
        };
    }
}