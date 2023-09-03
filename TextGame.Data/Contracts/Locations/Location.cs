using System.Text.Json.Serialization;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Contracts.Games;
using TextGame.Data.Contracts.Navigation;
using TextGame.Data.Sources;

namespace TextGame.Data.Contracts.Locations;

public interface ILocation
{
    string Key { get; }

    IGame Game { get; }

    LocalizedContentProvider<IReadOnlyCollection<NavigationCommand>> LocalizedNavigationCommands { get; }
}

public record LocationBuilder(
    string Key,
    IReadOnlyCollection<NavigationCommand>? NavigationCommands)
{
    [JsonIgnore]
    public LocalizedContentProvider<IReadOnlyCollection<NavigationCommand>> LocalizedNavigationCommands { get; init; } = null!;

    public IReadOnlyCollection<LocationGameObject> Objects { get; init; } = Array.Empty<LocationGameObject>();

    public IReadOnlyCollection<LocationHint> Hints { get; init; } = Array.Empty<LocationHint>();
}

public record LocationGameObject(
    string Key,
    string[] RequiredChapterKeys);

public enum LocationHintType
{
    FindObject,
    General
}

public record LocationHint(
    LocationHintType Type,
    string? ObjectKey,
    Paragraph[] Paragraphs);



