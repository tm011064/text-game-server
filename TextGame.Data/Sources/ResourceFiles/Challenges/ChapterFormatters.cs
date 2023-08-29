using TextGame.Data.Contracts.Chapters;

namespace TextGame.Data.Sources.ResourceFiles.Challenges;

public class ChallengeFormatter
{
    private static readonly IReadOnlyDictionary<ChallengeType, IChallengeConfigurationFormatter> challengeTypeParsers = new Dictionary<ChallengeType, IChallengeConfigurationFormatter>
    {
        { ChallengeType.TypeKeys, new TypeKeysConfigurationChallengeFormatter() },
        { ChallengeType.LearnTabKey, new LearnTabKeyChallengeFormatter() }
    };

    public IEnumerable<(string Locale, Challenge Challenge)> Format(
        Challenge challenge,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> localeMap)
    {
        var formatter = challengeTypeParsers.GetOrNotFound(challenge.Type);

        foreach (var (locale, configuration) in formatter.Format(challenge.Configuration, localeMap))
        {
            yield return (
                locale,
                challenge with
                {
                    SuccessMessage = localeMap.GetValueOrDefault("success-message")?.ToString() ?? challenge.SuccessMessage,
                    Configuration = configuration
                });
        }
    }
}
