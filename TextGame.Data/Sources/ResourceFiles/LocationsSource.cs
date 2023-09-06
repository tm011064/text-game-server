using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Contracts.Games;
using TextGame.Data.Contracts.Locations;
using TextGame.Data.Contracts.Navigation;
using TextGame.Data.Resources;
using TextGame.Data.Sources.ResourceFiles.NavigationCommands;

namespace TextGame.Data.Sources.ResourceFiles;

public interface ILocationsSource
{
    ILocation[] Get(IGame game);
}

public partial class LocationsSource : ILocationsSource
{
    private const string FOLDER_NAME = ".locations.";

    private static readonly NavigationCommandFormatter navigationCommandFormatter = new();

    private static readonly Regex regex = Regex();

    [GeneratedRegex("^([a-zA-Z0-9\\-\\.]*).([a-z]{2})-([a-zA-Z]{2}).json$", RegexOptions.Compiled)]
    private static partial Regex Regex();

    public ILocation[] Get(IGame game)
    {
        var (localeMaps, files) = ResourceService.ResourceNames[game.Key]
            .Where(x => x.Contains(FOLDER_NAME))
            .Select(x =>
            {
                var match = regex.Match(x);

                return match.Success
                    ? new
                    {
                        ResourceName = x,
                        FileName = match.Groups[1].Value,
                        IsKeyValueMap = true,
                        Locale = $"{match.Groups[2].Value}-{match.Groups[3].Value}"
                    }
                    : new
                    {
                        ResourceName = x,
                        FileName = x[..x.LastIndexOf('.')],
                        IsKeyValueMap = false,
                        Locale = ""
                    };
            })
            .Split(x => x.IsKeyValueMap);

        var localeMapsByFileName = localeMaps.ToLookup(x => x.FileName);

        return files
            .Select(file =>
            {
                var map = localeMapsByFileName[file.FileName];

                return Load(
                    file.ResourceName,
                    game,
                    map.ToDictionary(x => x.Locale, x => LoadLocaleMap(x.ResourceName)));
            })
            .ToArray();
    }

    private static IReadOnlyDictionary<string, object> LoadLocaleMap(string resourceName)
    {
        using var stream = ResourceService.ResourceAssembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);

        var json = reader.ReadToEnd();

        var items = JsonSerializer.Deserialize<LocaleMapItem[]>(json, JsonOptions.Default)
            ?? throw new Exception();

        return items.ToDictionary(x => x.Key, x => x.Value);
    }

    private static LocationBuilder Load(
        string resourceName,
        IGame game,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> localeMap)
    {
        using var stream = ResourceService.ResourceAssembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);

        var json = reader.ReadToEnd();

        var builder = JsonSerializer.Deserialize<LocationBuilder>(json, JsonOptions.Default)
            ?? throw new Exception();

        var fileName = resourceName[(resourceName.IndexOf(FOLDER_NAME) + FOLDER_NAME.Length)..];
        var locationKey = fileName[..fileName.LastIndexOf('.')];

        return builder with
        {
            Key = locationKey,
            Game = game,

            LocalizedNavigationCommands = builder.NavigationCommands
                ?.Let(navigationCommands => new LocalizedContentProvider<IReadOnlyCollection<NavigationCommand>>(
                    navigationCommands
                        .SelectMany(x => navigationCommandFormatter.Format(x, localeMap))
                        .GroupBy(x => x.Locale)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.NavigationCommand).ToReadOnlyCollection())))
                ?? LocalizedContentProvider<IReadOnlyCollection<NavigationCommand>>.Empty
        };
    }

    private record LocaleMapItem(string Key, object Value);

    private record LocationBuilder(
        string Key,
        string LocationKey,
        IGame Game,
        string ForwardLocationKey,
        IReadOnlyCollection<NavigationCommand>? NavigationCommands,
        IReadOnlyCollection<LocationGameObject>? Objects,
        Challenge? Challenge) : ILocation
    {
        [JsonIgnore]
        public LocalizedContentProvider<IReadOnlyCollection<NavigationCommand>> LocalizedNavigationCommands { get; init; } = null!;
    }
}

