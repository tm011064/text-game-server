namespace TextGame.Data.Contracts.Chapters;

public record Challenge(
    ChallengeType Type,
    object Configuration,
    string ChapterKey,
    string SuccessMessage,
    string SuccessMessageContentKey);

public record LearnTabKeyConfiguration(
    LearnTabKeyConfigurationItem[] Items);

public record LearnTabKeyConfigurationItem(
    string Description,
    int? TabCount,
    int? TargetIndex,
    int? StartIndex,
    int? RandomTargetCount);

public record TypeKeysConfiguration(
    TypeKeysConfigurationItem[] Items);

public record TypeKeysConfigurationItem(
    string Description,
    string[] Keys);