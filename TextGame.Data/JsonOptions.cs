using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextGame.Data
{
    public static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = Create();

        private static JsonSerializerOptions Create()
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false
            };

            options.Converters.Add(new JsonStringEnumConverter());

            return options;
        }
    }

}

