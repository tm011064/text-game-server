namespace TextGame.Data.Contracts.Chapters;

public record Paragraph(string Text);

public record LocalizedParagraph(string Locale, IReadOnlyCollection<Paragraph> Paragraphs);

