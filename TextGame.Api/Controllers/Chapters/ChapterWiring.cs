namespace TextGame.Api.Controllers.Chapters;
using TextGame.Core.Chapters;
using TextGame.Data.Contracts.Chapters;

public static class ChapterWiring
{
    public static object ToWire(this IChapter record) => new
    {
        Id = record.GetCompositeKey(),

        Paragraphs = record.Paragraphs.Select(ToWire).ToArray(),
        Challenge = record.Challenge?.ToWire(),
        AllowedCommands = record.NavigationCommands.Select(x => x.Type).ToArray()
    };

    private static object? ToWire(this Challenge record) => new
    {
        record.Type,
        record.Configuration
    };

    private static object ToWire(this Paragraph record, int index) => new
    {
        Index = index,
        record.Text
    };
}