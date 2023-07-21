namespace TextGame.Api.Controllers.Emoticons;

using Microsoft.AspNetCore.Mvc;
using TextGame.Core.Emotions;
using TextGame.Data.Contracts;

[ApiController]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class EmoticonsController : ControllerBase
{
    private readonly ILogger<EmoticonsController> logger;

    private readonly IEmotionProvider provider;

    public EmoticonsController(ILogger<EmoticonsController> logger, IEmotionProvider provider)
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

    private object ToWire(Emotion record) => new
    {
        Id = record.Key,
        Terms = record.Emoticons
    };
}