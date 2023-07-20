using Microsoft.AspNetCore.Mvc;
using TextGame.Core.Chapters;
using TextGame.Data.Contracts;

namespace TextGame.Api.Controllers.Chapters
{
    [ApiController]
    [ApiVersion("20220718")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("[controller]")]
    public class ChaptersController : ControllerBase
    {
        private readonly ILogger<ChaptersController> logger;

        private readonly IChapterProvider provider;

        public ChaptersController(ILogger<ChaptersController> logger, IChapterProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChapter(
            [FromRoute] string id)
        {
            var chapter = await provider.GetChapter(chapterKey: id);

            return Ok(ToWire(chapter));
        }

        private object ToWire(Chapter record) => new
        {
            Id = record.GetCompositeKey(),

            EmoticonId = record.ParagraphGroups.First().EmotionKey,
            Paragraphs = record.ParagraphGroups.First().Paragraphs.Select(ToWire).ToArray()
        };

        private object ToWire(Paragraph record, int index) => new
        {
            Index = index,
            record.Text
        };

        //private object ToWire(ParagraphGroup record) => new
        //{
        //    EmotionId = record.EmotionKey,

        //    Paragraphs = record.Paragraphs.Select(ToWire).ToArray()
        //};

        //private object ToWire(ChapterCommand record) => new
        //{
        //    Type = record.Type.ToString(),
        //    Action = ToWire(record.Action)
        //};

        //private object ToWire(ChapterCommandAction record) => new
        //{
        //    Type = record.Type.ToString(),
        //    ChapterId = record.ChapterKey
        //};
    }
}