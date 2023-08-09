using System.Text.Json;
using TextGame.Data.Resources;

namespace TextGame.Data.Sources
{
    public abstract class AbstractGlobalResourceJsonSource<TRecord>
    {
        public abstract string FileName { get; }

        public TRecord Get(string locale)
        {
            string resourceName = ResourceService.GlobalResources.SingleOrDefault(x => x.EndsWith(FileName))
                ?? throw new NullReferenceException();

            using var stream = ResourceService.ResourceAssembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            var json = reader.ReadToEnd();

            return JsonSerializer.Deserialize<TRecord>(json, JsonOptions.Default)
                ?? throw new NullReferenceException();
        }
    }
}

