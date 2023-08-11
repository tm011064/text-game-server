using System.Text;

namespace TextGame.Data;

public static class StringExtensions
{
    public static bool IsNullOrWhitespace(this string? self)
    {
        return string.IsNullOrWhiteSpace(self);
    }

    public static IEnumerable<string> SplitAndTrim(this string source, string splitOn = ",")
    {
        return source.Split(splitOn).Select(x => x.Trim()).WhereNot(string.IsNullOrWhiteSpace);
    }

    public static string? FromPascalToKebabCase(this string? value)
    {
        if (value.IsNullOrWhitespace())
        {
            return null;
        }

        var builder = new StringBuilder();

        builder.Append(char.ToLower(value!.First()));

        foreach (var c in value!.Skip(1))
        {
            if (char.IsUpper(c))
            {
                builder.Append('-');
                builder.Append(char.ToLower(c));
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}
