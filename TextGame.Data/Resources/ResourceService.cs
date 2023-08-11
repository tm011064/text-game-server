using System.Reflection;
using TextGame.Data.Resources.Games;
using TextGame.Data.Resources.Global;

namespace TextGame.Data.Resources;

public static class ResourceService
{
    private static readonly Type globalMarker = typeof(GlobalResourceMarker);

    private static readonly Type gameMarker = typeof(GamesResourceMarker);

    private static readonly Assembly assembly = typeof(ResourceService).Assembly;

    private static string GetGameKey(string recourceName)
    {
        var prefixLength = gameMarker.Namespace!.Length;
        var separator = ".";
        var separatorLength = separator.Length;

        return recourceName.Substring(
            prefixLength + separatorLength,
            recourceName.IndexOf(separator, prefixLength + separatorLength) - prefixLength - separatorLength);
    }

    public static readonly ILookup<string, string> ResourceNames = assembly
        .GetManifestResourceNames()
        .Where(resourceName => resourceName.StartsWith(gameMarker.Namespace!))
        .ToLookup(GetGameKey);

    public static readonly ISet<string> GlobalResources = assembly
        .GetManifestResourceNames()
        .Where(resourceName => resourceName.StartsWith(globalMarker.Namespace!))
        .ToHashSet();

    public static readonly Assembly ResourceAssembly = assembly;

    public static readonly ISet<string> GameKeys = ResourceNames.Select(x => x.Key).ToHashSet();
}

