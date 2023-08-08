using FluentMigrator;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;

namespace TextGame.Data.Migrations
{
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

    [Migration(1)]
    public class AddUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithPrimaryIdAndResourceKey()
                .WithAuditColumns()
                .WithColumn("email").AsString(256).NotNullable()
                .WithColumn("password_initialization_vector").AsBinary().NotNullable()
                .WithColumn("password_salt").AsBinary().NotNullable()
                .WithColumn("password_iterations").AsInt32().NotNullable()
                .WithColumn("password_data").AsString(256).NotNullable()
                .WithColumn("password_cipher_text").AsBinary().NotNullable()
                .WithColumn("refresh_token").AsString(int.MaxValue).Nullable()
                .WithColumn("refresh_token_expires_at").AsInt32().Nullable();

            this.CreateUniqueIndexWithNullable(
                table: "users",
                indexColumns: "email",
                whereNullColumns: "deleted_at");

            this.CreateUniqueIndexWithNullable(
                table: "users",
                indexColumns: "resource_key",
                whereNullColumns: "deleted_at");
        }

        public override void Down()
        {
            Delete.Table("users");
        }
    }

    [Migration(2)]
    public class AddAccountsTable : Migration
    {
        public override void Up()
        {
            Create.Table("accounts")
                .WithPrimaryIdAndResourceKey()
                .WithCreatedAtAndDeletedAt()
                .WithColumn("user_id").AsInt64().NotNullable().ForeignKey("users", "id")
                .WithColumn("name").AsString(16).NotNullable();

            this.CreateUniqueIndexWithNullable(
                table: "accounts",
                indexColumns: "name, user_id",
                whereNullColumns: "deleted_at");

            this.CreateUniqueIndexWithNullable(
                table: "accounts",
                indexColumns: "resource_key",
                whereNullColumns: "deleted_at");
        }

        public override void Down()
        {
            Delete.Table("accounts");
        }
    }

    [Migration(3)]
    public class AddGamesTable : Migration
    {
        public override void Up()
        {
            Create.Table("games")
                .WithPrimaryIdAndResourceKey()
                .WithCreatedAtAndDeletedAt();

            this.CreateUniqueIndexWithNullable(
                table: "games",
                indexColumns: "resource_key",
                whereNullColumns: "deleted_at");

            Create.Table("game_accounts")
                .WithPrimaryIdAndResourceKey()
                .WithCreatedAtAndDeletedAt()
                .WithColumn("game_id").AsInt64().NotNullable().ForeignKey("games", "id")
                .WithColumn("account_id").AsInt64().NotNullable().ForeignKey("accounts", "id");

            this.CreateUniqueIndexWithNullable(
                table: "game_accounts",
                indexColumns: "account_id, game_id",
                whereNullColumns: "deleted_at");
        }

        public override void Down()
        {
            Delete.Table("game_accounts");
            Delete.Table("games");
        }
    }
}