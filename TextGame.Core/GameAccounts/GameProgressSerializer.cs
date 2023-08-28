using System.Text.Json;
using TextGame.Core.Chapters;
using TextGame.Data;

namespace TextGame.Core.GameAccounts;

public class GameStateSerializer
{
    private readonly IChapterProvider chapterProvider;

    public GameStateSerializer(IChapterProvider chapterProvider)
    {
        this.chapterProvider = chapterProvider;
    }

    public async Task<IReadOnlyCollection<GameState>> Deserialize(string gameStateJson)
    {
        var deserialized = JsonSerializer.Deserialize<GameStateJson[]>(gameStateJson, JsonOptions.Default)!;

        var chaptersMap = await chapterProvider.GetChaptersMap(
            deserialized
                .SelectMany(x => new[] { x.CurrentChapterKey }
                    .Concat(x.VisitedChapterKeys)
                    .Concat(x.CompletedChallengeKeys))
                .ToHashSet());

        return deserialized
            .Select(json => new GameState(
                Key: json.Key,
                CurrentChapter: chaptersMap[json.CurrentChapterKey],
                UpdatedAt: json.UpdatedAt,
                SlotName: json.SlotName,
                CompletedChallenges: json.CompletedChallengeKeys.Select(x => chaptersMap[x]).ToHashSet(),
                VisitedChapters: json.VisitedChapterKeys.Select(x => chaptersMap[x]).ToHashSet()))
            .ToReadOnlyCollection();
    }

    public string Serialize(params GameState[] gameStates)
    {
        var records = gameStates.Select(x => x.ToJson()).ToArray();

        return JsonSerializer.Serialize(records, JsonOptions.Default);
    }
}