using TextGame.Data.Contracts.GameObjects;

namespace TextGame.Data.Sources;

public interface IGameObjectsSource : IGlobalResourceJsonSource<string, string>
{
}

public class GameObjectsSource :
    AbstractTwoWayGlobalLocalizedResourceJsonSource<GameObject, string, string>,
    IGameObjectsSource
{
    protected override string FilePrefix => "objects";

    public LocalizedContentProvider<TwoWayLookup<string, string>> Get() => LoadTwoWayLookup();

    protected override string GetKey(GameObject value) => value.Key;

    protected override IEnumerable<string> GetValues(GameObject key) => key.Terms;
}