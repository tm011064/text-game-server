namespace TextGame.Data;

internal static class SqlStatements
{
    public static readonly string CreateRandomNumber = "abs(random() % 2147483640)";
}