using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TextGame.Api.Auth;
using TextGame.Core.Emotions;
using TextGame.Data.Contracts.Emotions;

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