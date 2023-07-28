using Dapper;
using System.Data;

namespace TextGame.Data;

public class DateTimeOffsetTypeHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.Value = value.ToUnixTimeSeconds();
    }

    public override DateTimeOffset Parse(object value)
    {
        return DateTimeOffset.FromUnixTimeSeconds((long)value);
    }
}