using MediatR;
using TextGame.Core.Chapters;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Contracts.Chapters;

namespace TextGame.Core.GameAccounts;
// TODO (Roman): cleanup

public record GameStateJson(
    string Key,
    string CurrentChapterKey,
    DateTimeOffset UpdatedAt,
    string? SlotName,
    string[] CompletedChallengeKeys,
    string[] VisitedChapterKeys)
{
    public bool IsAutoSave() => SlotName.IsNullOrWhitespace();
}

public record GameState(
    string Key,
    IChapter CurrentChapter,
    DateTimeOffset UpdatedAt,
    string? SlotName,
    IReadOnlySet<IChapter> CompletedChallenges,
    IReadOnlySet<IChapter> VisitedChapters)
{
    public static GameState New(IChapter chapter, string? slotName, AuthTicket ticket) => new(
        Key: Guid.NewGuid().ToString(),
        CurrentChapter: chapter,
        UpdatedAt: ticket.CreatedAt,
        SlotName: slotName,
        CompletedChallenges: Enumerable.Empty<IChapter>().ToHashSet(),
        VisitedChapters: Enumerable.Empty<IChapter>().ToHashSet());

    public bool IsAutoSave() => SlotName.IsNullOrWhitespace();

    public GameStateJson ToJson() => new(
        Key,
        CurrentChapter.GetCompositeKey(),
        UpdatedAt,
        SlotName,
        CompletedChallenges.Select(x => x.GetCompositeKey()).Distinct().ToArray(),
        VisitedChapters.Select(x => x.GetCompositeKey()).Distinct().ToArray());

    public GameState WithCompletedChallenge(IChapter chapter) => CompletedChallenges.Contains(chapter)
        ? this
        : this with { CompletedChallenges = CompletedChallenges.Concat(new[] { chapter }).ToHashSet() };

    public GameState WithVisitedChapter(IChapter chapter) => VisitedChapters.Contains(chapter)
        ? this
        : this with { VisitedChapters = VisitedChapters.Concat(new[] { chapter }).ToHashSet() };
}

public class GameStateCollectionBuilderFactory
{
    private readonly GameStateSerializer gameStateSerializer;

    public GameStateCollectionBuilderFactory(GameStateSerializer gameStateSerializer)
    {
        this.gameStateSerializer = gameStateSerializer;
    }

    public GameStateCollectionBuilder Create(GameAccount gameAccount)
    {
        return new GameStateCollectionBuilder(gameAccount.GameStates, gameStateSerializer);
    }
}

public class GameStateCollectionBuilder
{
    private readonly GameStateSerializer gameStateSerializer;

    private readonly IDictionary<string, GameState> gameStatesByKey;

    public GameStateCollectionBuilder(IReadOnlyCollection<GameState> gameStates, GameStateSerializer gameStateSerializer)
    {
        gameStatesByKey = gameStates.ToDictionary(x => x.Key);

        this.gameStateSerializer = gameStateSerializer;
    }

    public GameStateCollectionBuilder Replace(Func<GameState, bool> selector, Func<GameState, GameState> map)
    {
        var record = gameStatesByKey.Values.Single(selector);

        gameStatesByKey[record.Key] = map(record);

        return this;
    }

    public string Build()
    {
        var records = gameStatesByKey.Values
            .OrderByDescending(x => x.IsAutoSave())
            .ThenBy(x => x.SlotName);

        return gameStateSerializer.Serialize(records.ToArray());
    }
}

public record GameStateCollectionItem(string SlotName, GameState GameState);

public record GameAccount(
    long Id,
    string Key,
    IReadOnlyCollection<GameState> GameStates,
    long UserAccountId,
    long GameId,
    long Version);


public class GameAccountConverter
{
    private readonly GameStateSerializer serializer;

    public GameAccountConverter(GameStateSerializer serializer)
    {
        this.serializer = serializer;
    }

    public GameAccount Convert(IGameAccount record, string locale)
    {
        return new GameAccount(
            record.Id,
            record.Key,
            serializer.Deserialize(record.GameStateJson, locale).ToArray(),
            record.UserAccountId,
            record.GameId,
            record.Version);
    }
}