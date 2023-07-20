using TextGame.Data.Resources;

namespace TextGame.Data.Sources
{
    public interface IGameSource
    {
        ISet<string> GetKeys();
    }

    public class GameSource : IGameSource
    {
        public ISet<string> GetKeys() => ResourceService.GameKeys;
    }
}

