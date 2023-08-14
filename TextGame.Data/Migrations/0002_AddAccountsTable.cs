using FluentMigrator;

namespace TextGame.Data.Migrations;

[Migration(2)]
public class AddAccountsTable : Migration
{
    public override void Up()
    {
        Create.Table("user_accounts")
            .WithPrimaryIdAndResourceKey()
            .WithCreatedAtAndDeletedAt()
            .WithColumn("user_id").AsInt64().NotNullable().ForeignKey("users", "id")
            .WithColumn("name").AsString(16).NotNullable();

        this.CreateUniqueIndexWithNullable(
            table: "user_accounts",
            indexColumns: "name, user_id",
            whereNullColumns: "deleted_at");

        this.CreateUniqueIndexWithNullable(
            table: "user_accounts",
            indexColumns: "resource_key",
            whereNullColumns: "deleted_at");
    }

    public override void Down()
    {
        Delete.Table("user_accounts");
    }
}
