using TextGame.Data.Contracts.Emotions;
using TextGame.Data.Sources.ResourceFiles;

namespace TextGame.Data.Sources;

public interface IEmotionsSource : IGlobalResourceJsonSource<string, string>
{
}

public class EmotionsSource :
    AbstractTwoWayGlobalLocalizedResourceJsonSource<Emotion, string, string>,
    IEmotionsSource
{
    protected override string FilePrefix => "emotions";

    public LocalizedContentProvider<TwoWayLookup<string, string>> Get() => LoadTwoWayLookup();

    protected override string GetKey(Emotion value) => value.Key;

    protected override IEnumerable<string> GetValues(Emotion key) => key.Emoticons;
}