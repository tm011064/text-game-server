using FluentMigrator;
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
    }

    [Migration(1)]
    public class AddUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithPrimaryIdAndResourceKey()
                .WithCreatedAtAndDeletedAt()
                .WithColumn("email").AsString(256).NotNullable()
                .WithColumn("password_initialization_vector").AsBinary().NotNullable()
                .WithColumn("password_salt").AsBinary().NotNullable()
                .WithColumn("password_iterations").AsInt32().NotNullable()
                .WithColumn("password_data").AsString(256).NotNullable()
                .WithColumn("password_cipher_text").AsBinary().NotNullable()
                .WithColumn("refresh_token").AsString(int.MaxValue).Nullable()
                .WithColumn("refresh_token_expires_at").AsInt32().Nullable();

            Create.UniqueConstraint()
                .OnTable("users")
                .Columns("email", "deleted_at");

            Create.UniqueConstraint()
                .OnTable("users")
                .Columns("resource_key", "deleted_at");
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

            Create.UniqueConstraint()
                .OnTable("accounts")
                .Columns("name", "user_id", "deleted_at");

            Create.UniqueConstraint()
                .OnTable("accounts")
                .Columns("resource_key", "deleted_at");
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

            Create.UniqueConstraint()
                .OnTable("games")
                .Columns("resource_key", "deleted_at");

            Create.Table("commands")
                .WithPrimaryIdAndResourceKey()
                .WithCreatedAtAndDeletedAt()
                .WithColumn("command_type").AsString(64).NotNullable()
                .WithColumn("action_type").AsString(64).NotNullable()
                .WithColumn("chapter_id").AsInt64().Nullable();

            Create.Table("game_accounts")
                .WithPrimaryIdAndResourceKey()
                .WithCreatedAtAndDeletedAt()
                .WithColumn("game_id").AsInt64().NotNullable().ForeignKey("games", "id")
                .WithColumn("account_id").AsInt64().NotNullable().ForeignKey("accounts", "id");

            Create.UniqueConstraint()
                .OnTable("game_accounts")
                .Columns("account_id", "game_id", "deleted_at");

            //Create.Table("chapters")
            //    .WithPrimaryIdAndResourceKey()
            //    .WithCreatedAtAndDeletedAt()
            //    .WithColumn("game_id").AsInt64().NotNullable().ForeignKey("games", "id")
            //    .WithColumn("json").AsString(int.MaxValue).Nullable();
        }

        public override void Down()
        {
            Delete.Table("games");
        }
    }
}