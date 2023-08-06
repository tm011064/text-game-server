namespace TextGame.Api.Controllers.Chapters;

using Microsoft.AspNetCore.Mvc;
using TextGame.Api.Auth;
using TextGame.Core.Chapters;
using TextGame.Data.Contracts.Chapters;

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

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetChapter(
        [FromRoute] string id)
    {
        var chapter = await provider.GetChapter(chapterKey: id);

        return Ok(ToWire(chapter));
    }

    private object ToWire(IChapter record) => new
    {
        Id = record.GetCompositeKey(),

        Paragraphs = record.Paragraphs.Select(ToWire).ToArray(),
        Challenge = ToWire(record.Challenge)
    };

    private static object? ToWire(Challenge? challenge) => challenge == null
        ? null
        : new
        {
            challenge!.Type,
            challenge!.Configuration
        };

    private object ToWire(Paragraph record, int index) => new
    {
        Index = index,
        record.Text
    };
}