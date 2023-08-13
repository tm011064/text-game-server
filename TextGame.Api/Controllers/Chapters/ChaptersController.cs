using Microsoft.AspNetCore.Mvc;
using TextGame.Api.Auth;
using TextGame.Core.Chapters;

namespace TextGame.Api.Controllers.Chapters;

[ApiController]
[Authorize]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class ChaptersController : ControllerBase
{
    private readonly IChapterProvider provider;

    public ChaptersController(IChapterProvider provider)
    {
        this.provider = provider;
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetChapter(
        [FromRoute] string id)
    {
        var chapter = await provider.GetChapter(chapterKey: id);

        return Ok(chapter.ToWire());
    }
}
