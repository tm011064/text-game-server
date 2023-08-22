using TextGame.Core.Chapters;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Sources.ResourceFiles;

namespace TextGame.Api.Controllers.Chapters;

public static class ChapterWiring
{
    public static object ToWire(this IChapter record, string locale) =>
        record.ToWire(locale, LocalizedContentProvider<IReadOnlyCollection<Paragraph>>.Empty);

    public static object ToWire(this IChapter record, string locale, LocalizedContentProvider<IReadOnlyCollection<Paragraph>> forwardParagraphs) => new
    {
        Id = record.GetCompositeKey(),

        Paragraphs = forwardParagraphs.Get(locale, Array.Empty<Paragraph>())
            .Concat(record.ParagraphsByLocale.Get(locale, Array.Empty<Paragraph>()))
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