using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextGame.Core.Emotions;

namespace TextGame.Api.Controllers.Emoticons;

[ApiController]
[ApiVersion("20220718")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class EmoticonsController : ControllerBase
{
    private readonly IEmotionProvider provider;

    public EmoticonsController(IEmotionProvider provider)
    {
        this.provider = provider;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll()
    {
        var records = provider.Get(this.GetLocale());

        return Ok(records.GetAll().Select(ToWire).ToArray());
    }

    private object ToWire(IGrouping<string, string> record) => new
    {
        Id = record.Key,
        Terms = record
    };
}