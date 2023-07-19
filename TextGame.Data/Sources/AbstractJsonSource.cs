using System.Text.Json;

namespace TextGame.Data.Sources
{
    public abstract class AbstractJsonSource<TRecord>
    {
        public abstract string FileName { get; }

        public TRecord Get(string gameKey, string locale)
        {
            var file = new FileInfo($"Resources/Games/{gameKey}/{FileName}");

            if (!file.Exists)
            {
                throw new FileNotFoundException(file.FullName);
            }

            using var reader = new StreamReader(file.FullName);

            var json = reader.ReadToEnd();

            return JsonSerializer.Deserialize<TRecord>(json, JsonOptions.Default)
                ?? throw new NullReferenceException();
        }
    }

}

