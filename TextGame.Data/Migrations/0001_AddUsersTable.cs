using FluentMigrator;

namespace TextGame.Data.Migrations;

[Migration(1)]
public class AddUsersTable : Migration
{
    public override void Up()
    {
        Create.Table("users")
            .WithPrimaryIdAndResourceKey()
            .WithAuditColumns()
            .WithColumn("email").AsString(256).NotNullable()
            .WithColumn("roles_json").AsString(512).NotNullable()
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
