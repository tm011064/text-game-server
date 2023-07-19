namespace TextGame.Data.Contracts
{
    public readonly record struct GameContext(
        IReadOnlyCollection<Chapter> Chapters,
        IReadOnlyCollection<Emotion> Emotions,
        IReadOnlyCollection<TerminalCommand> Commands);

    public readonly record struct Chapter(
        string Key,
        IReadOnlyCollection<ChapterCommand> Commands,
        IReadOnlyCollection<MessageGroup> MessageGroups,
        MessageGroupConfig MessageGroupConfig);

    public readonly record struct ChapterCommand(
        ChapterCommandType Type,
        ChapterCommandAction Action);

    public readonly record struct ChapterCommandAction(
        ChapterCommandActionType Type,
        string? ChapterKey);

    public readonly record struct MessageGroupConfig(
        MessageGroupType Type);

    public readonly record struct MessageGroup(
        string EmotionKey,
        IReadOnlyCollection<MessageGroupMessage> Messages);

    public readonly record struct MessageGroupMessage(
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
    public enum MessageGroupType
    {
        Sequential
    }

    public readonly record struct Emotion(
        string Key,
        IReadOnlyCollection<string> Emoticons);
}

