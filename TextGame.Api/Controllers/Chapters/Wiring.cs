using TextGame.Core.Chapters;
using TextGame.Data.Contracts.Chapters;

namespace TextGame.Api.Controllers.Chapters;

public static class ChapterWiring
{
    public static object ToWire(this IChapter record, IEnumerable<Paragraph>? forwardParagraphs = null) => new
    {
        Id = record.GetCompositeKey(),

        Paragraphs = (forwardParagraphs ?? Array.Empty<Paragraph>()).Concat(record.Paragraphs)
            .Select(ParagraphWiring.ToWire)
            .ToArray(),
        Challenge = record.Challenge?.ToWire(),
        AllowedCommands = record.NavigationCommands.Select(x => x.Type).ToArray()
    };

    private static object? ToWire(this Challenge record) => new
    {
        record.Type,
        record.Configuration
    };
}

public static class ParagraphWiring
{
    public static object ToWire(this Paragraph record, int index) => new
    {
        Index = index,
        record.Text
    };
}