namespace TextGame.Data;

public static class StringExtensions
{
    public static bool IsNullOrWhitespace(this string? self)
    {
        return string.IsNullOrWhiteSpace(self);
    }
}
