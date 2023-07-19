using System.Text.Json;
using TextGame.Data.Resources.Games;

namespace TextGame.Data.Sources
{
    public abstract class AbstractGameContextItemJsonSource<TRecord>
    {
        private static readonly Type marker = typeof(ResourceMarker);

        private static readonly ILookup<string, string> resourceNames = marker.Assembly
            .GetManifestResourceNames()
            .ToLookup(GetGameKey);

        private static string GetGameKey(string recourceName)
        {
            return recourceName.Substring(
                marker.Namespace!.Length + 1,
                recourceName.IndexOf('.', marker.Namespace.Length + 1) - marker.Namespace.Length - 1);
        }

        public abstract string FileName { get; }

        public TRecord Get(string gameKey, string locale)
        {
            string resourceName = resourceNames[gameKey].SingleOrDefault(x => x.EndsWith(FileName))
                ?? throw new NullReferenceException(); ;

            using var stream = marker.Assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            var json = reader.ReadToEnd();

            return JsonSerializer.Deserialize<TRecord>(json, JsonOptions.Default)
                ?? throw new NullReferenceException();
        }
    }

}

