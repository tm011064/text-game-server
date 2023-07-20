namespace TextGame.Data.Contracts
{
    public readonly record struct GameContext(
        IReadOnlyCollection<Chapter> Chapters,
        IReadOnlyCollection<Emotion> Emotions,
        IReadOnlyCollection<TerminalCommand> Commands);

    public readonly record struct Chapter(
        string Key,
        string GameKey,
        IReadOnlyCollection<ChapterCommand> Commands,
        IReadOnlyCollection<ParagraphGroup> ParagraphGroups,
        ParagraphGroupConfig ParagraphGroupConfig);

    public readonly record struct ParagraphGroup(
        string EmotionKey,
        IReadOnlyCollection<Paragraph> Paragraphs);

    public readonly record struct ChapterCommand(
        ChapterCommandType Type,
        ChapterCommandAction Action);

    public readonly record struct ChapterCommandAction(
        ChapterCommandActionType Type,
        string? ChapterKey);

    public readonly record struct ParagraphGroupConfig(
        ParagraphGroupType Type);

    public readonly record struct Paragraph(
        string Text);

    public readonly record struct TerminalCommand(
        string Key,
        IReadOnlyCollection<string> Terms);

    public enum ChapterCommandActionType
    {
        ChangeChapter,
        ShowNextMessage
    }

    public enum ChapterCommandType
    {
        Confirm,
        Decline
    }
    public enum ParagraphGroupType
    {
        Sequential
    }

    public readonly record struct Emotion(
        string Key,
        IReadOnlyCollection<string> Emoticons);
}

