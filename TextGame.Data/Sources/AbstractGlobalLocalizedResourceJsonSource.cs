using System.Text.Json;
using System.Text.RegularExpressions;
using TextGame.Data.Resources;

namespace TextGame.Data.Sources;

public abstract class AbstractGlobalLocalizedResourceJsonSource<TKey, TRecord>
{
    protected abstract string FilePrefix { get; }

    protected abstract TKey GetKey(TRecord value);

    private static TRecord[] Load(ResourceMeta meta)
    {
        using var stream = ResourceService.ResourceAssembly.GetManifestResourceStream(meta.ResourceName)!;
        using var reader = new StreamReader(stream);

        var json = reader.ReadToEnd();

        return JsonSerializer.Deserialize<TRecord[]>(json, JsonOptions.Default)!;
    }

    protected IReadOnlyDictionary<string, IReadOnlyCollection<TRecord>> LoadLocalizedRecords()
    {
        var regex = new Regex($"{ResourceService.BuildGlobalResourcePath(FilePrefix)}.([a-z]{{2}})-([a-zA-Z]{{2}}).json$");

        var (defaultMeta, localizedMetas) = ResourceService.GlobalResources
            .Where(x => x.StartsWith(ResourceService.BuildGlobalResourcePath(FilePrefix)))
            .Select(x =>
            {
                var match = regex.Match(x);

                return match.Success
                    ? new ResourceMeta(x, $"{match.Groups[1].Value}-{match.Groups[2].Value}")
                    : new ResourceMeta(x, null);
            })
            .Split(x => x.Locale == null);

        var defaultValues = Load(defaultMeta.Single());

        if (typeof(TKey).IsEnum)
        {
            var expected = Enum.GetValues(typeof(TKey)).Cast<TKey>().ToHashSet();

            if (defaultValues.DistinctBy(GetKey).Count() < expected.Count)
            {
                var missing = expected.Except(defaultValues.Select(GetKey));

                throw new InvalidOperationException($"Default keys must be exhaustive, missing items: {string.Join(", ", missing)}");
            }
        }

        var localizedRecords = localizedMetas
            .Select(meta => new
            {
                Records = defaultValues
                    .LeftMerge(Load(meta), GetKey)
                    .ToReadOnlyCollection(),
                Locale = meta.Locale!
            })
            .ToDictionary(x => x.Locale, x => x.Records);

        localizedRecords[LocaleSettings.DefaultLocale] = defaultValues;

        return localizedRecords;
    }

    private record ResourceMeta(
        string ResourceName,
        string? Locale);
}


public abstract class AbstractTwoWayGlobalLocalizedResourceJsonSource<TRecord, TKey, TValue> :
    AbstractGlobalLocalizedResourceJsonSource<TKey, TRecord>
    where TValue : notnull
    where TKey : notnull
{
    protected abstract IEnumerable<TValue> GetValues(TRecord key);

    protected virtual IEqualityComparer<TKey>? GetKeyComparer() => null;

    protected virtual IEqualityComparer<TValue>? GetValueComparer() => null;

    protected LocalizedContentProvider<TwoWayLookup<TKey, TValue>> LoadTwoWayLookup()
    {
        var localizedRecords = LoadLocalizedRecords();

        var converted = localizedRecords.ToDictionary(
            x => x.Key,
            x => TwoWayLookup.Create(
                x.Value,
                GetKey,
                GetValues,
                GetValueComparer(),
                GetKeyComparer()));

        return new LocalizedContentProvider<TwoWayLookup<TKey, TValue>>(converted);
    }
}
