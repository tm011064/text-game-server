using TextGame.Data.Contracts.Chapters;

namespace TextGame.Data.Sources.ResourceFiles.Challenges;

public class LearnTabKeyChallengeFormatter :
    AbstractChallengeConfigurationFormatter<LearnTabKeyConfiguration>,
    IChallengeConfigurationFormatter
{
    protected override LearnTabKeyConfiguration DoFormat(LearnTabKeyConfiguration source, string locale, IReadOnlyDictionary<string, object> map)
    {
        return source with
        {
            Items = source.Items
                .Select((item, index) => item with
                {
                    Description = map.GetValueOrDefault($"description-{index + 1}")?.ToString() ?? item.Description
                })
                .ToArray()
        };
    }
}
