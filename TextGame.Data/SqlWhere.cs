namespace TextGame.Data;

internal static class SqlWhere
{
    public static string AndOptional<TValue>(string column, string parameter, TValue? value)
    {
        return value == null
            ? ""
            : $"and {column} = @{parameter}";
    }
}
