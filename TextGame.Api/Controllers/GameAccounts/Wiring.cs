using TextGame.Api.Controllers.Chapters;
using TextGame.Core.GameAccounts;

namespace TextGame.Api.Controllers.GameAccounts;

public static class Wiring
{
    public static object ToWire(this GameAccount record) => new
    {
        Id = record.Key,
        record.Version,

        GameStates = record.GameStates.Select(ToWire).ToArray()
    };

    private static object ToWire(GameState record) => new
    {
        CurrentChapter = record.CurrentChapter.ToWire(),
        record.SlotName,
        IsAutoSave = record.IsAutoSave(),
        record.UpdatedAt
    };
}