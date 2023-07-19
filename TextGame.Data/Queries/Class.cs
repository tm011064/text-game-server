using System;
using System.Resources;
using System.Text.Json;
using System.Text.Json.Serialization;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries
{
    public class GameContextSource
    {
        public GameContext Get(int gameId, string locale)
        {
            var file = new FileInfo("Resources/Games/Tutorials/chapters.json");

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false
            };
            options.Converters.Add(new JsonStringEnumConverter());

            using (var r = new StreamReader(file.FullName))
            {
                string json = r.ReadToEnd();
                var context = JsonSerializer.Deserialize<Chapter[]>(
                    json,
                    options);
            }

            return new GameContext();
        }
    }
}

