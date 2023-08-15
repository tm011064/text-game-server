using FluentMigrator;

namespace TextGame.Data.Migrations;

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
            .WithRowVersion()
            .WithColumn("game_id").AsInt64().NotNullable().ForeignKey("games", "id")
            .WithColumn("user_account_id").AsInt64().NotNullable().ForeignKey("user_accounts", "id")
            .WithColumn("game_states_json").AsString(int.MaxValue).NotNullable();

        this.CreateUniqueIndexWithNullable(
            table: "game_accounts",
            indexColumns: "user_account_id, game_id",
            whereNullColumns: "deleted_at");
    }

    public override void Down()
    {
        Delete.Table("game_accounts");
        Delete.Table("games");
    }
}