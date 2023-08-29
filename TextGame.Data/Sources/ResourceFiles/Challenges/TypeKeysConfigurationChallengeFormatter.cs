using System.Text.Json;
using TextGame.Data.Contracts.Chapters;

namespace TextGame.Data.Sources.ResourceFiles.Challenges;

public class TypeKeysConfigurationChallengeFormatter :
    AbstractChallengeConfigurationFormatter<TypeKeysConfiguration>,
    IChallengeConfigurationFormatter
{
    protected override TypeKeysConfiguration DoFormat(TypeKeysConfiguration source, string locale, IReadOnlyDictionary<string, object> map)
    {
        return source with
        {
            Items = source.Items
                .Select((item, index) => item with
                {
                    Description = map.GetValueOrDefault($"description-{index + 1}")?.ToString() ?? item.Description,
                    Keys = map.GetValueOrDefault($"description-{index + 1}-keys")
                        ?.Let(x => JsonSerializer.Deserialize<string[]>(x.ToString()!, JsonOptions.Default))
                        ?? item.Keys
                })
                .ToArray()
        };
    }
}
