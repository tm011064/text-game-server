using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace TextGame.Data.Migrations;

public static class MigrationExtensions
{
    public static ICreateTableWithColumnSyntax WithPrimaryIdAndResourceKey(this ICreateTableWithColumnSyntax self)
    {
        return self
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("resource_key").AsString(36).NotNullable();
    }

    public static ICreateTableWithColumnSyntax WithCreatedAtAndDeletedAt(this ICreateTableWithColumnSyntax self)
    {
        return self
            .WithColumn("created_at").AsInt32().NotNullable()
            .WithColumn("created_by").AsString(256).NotNullable()
            .WithColumn("deleted_at").AsInt32().Nullable()
            .WithColumn("deleted_by").AsString(256).Nullable();
    }

    public static ICreateTableWithColumnSyntax WithAuditColumns(this ICreateTableWithColumnSyntax self)
    {
        return self
            .WithColumn("created_at").AsInt32().NotNullable()
            .WithColumn("created_by").AsString(256).NotNullable()
            .WithColumn("updated_at").AsInt32().NotNullable()
            .WithColumn("updated_by").AsString(256).NotNullable()
            .WithColumn("deleted_at").AsInt32().Nullable()
            .WithColumn("deleted_by").AsString(256).Nullable();
    }

    public static void CreateUniqueIndexWithNullable(
        this Migration self,
        string table,
        string indexColumns,
        string whereNullColumns)
    {
        var columnArray = indexColumns.SplitAndTrim().ToArray();
        var whereNullColumnArray = whereNullColumns.SplitAndTrim().ToArray();

        var sql =
            $"create unique index if not exists IX_{table}_{string.Join("_", columnArray)} " +
            $"on {table}({string.Join(", ", columnArray)}) " +
            $"where {string.Join(" and ", whereNullColumns.SplitAndTrim().Select(x => $"{x} is null"))};";

        self.Execute.Sql(sql);
    }
}
