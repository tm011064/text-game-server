using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Contracts.Navigation;

namespace TextGame.Data.Contracts.Locations;

public record Location(string Key)
{
    public IReadOnlyCollection<NavigationCommand> NavigationCommands { get; init; } = Array.Empty<NavigationCommand>();

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



