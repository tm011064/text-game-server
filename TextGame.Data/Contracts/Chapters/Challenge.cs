namespace TextGame.Data.Contracts.Chapters;

public record Challenge(
    ChallengeType Type,
    object Configuration,
    string ChapterKey,
    string SuccessMessage);
