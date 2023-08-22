using TextGame.Api.Controllers.Chapters;
using TextGame.Core.GameAccounts;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Sources.ResourceFiles;

namespace TextGame.Api.Controllers.GameAccounts;

public static class Wiring
{
    public static object ToWire(this GameAccount record, string locale) => new
    {
        Id = record.Key,
        record.Version,

        GameStates = record.GameStates.Select(x => x.ToWire(locale)).ToArray()
    };

    private static object ToWire(this GameState record, string locale) => new
    {
        CurrentChapter = record.CurrentChapter.ToWire(locale),
        record.SlotName,
        IsAutoSave = record.IsAutoSave(),
        record.UpdatedAt
    };
}