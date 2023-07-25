﻿using FluentMigrator;

namespace TextGame.Data.Migrations
{
    [Migration(1)]
    public class AddUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("resource_key").AsString(36).NotNullable()
                .WithColumn("created_at").AsInt32().NotNullable()
                .WithColumn("created_by").AsString(256).NotNullable()
                .WithColumn("deleted_at").AsInt32().Nullable()
                .WithColumn("deleted_by").AsString(256).Nullable()
                .WithColumn("email").AsString(256).NotNullable().Unique()
                .WithColumn("password_initialization_vector").AsBinary().NotNullable()
                .WithColumn("password_salt").AsBinary().NotNullable()
                .WithColumn("password_iterations").AsInt32().NotNullable()
                .WithColumn("password_data").AsString(256).NotNullable()
                .WithColumn("password_cipher_text").AsBinary().NotNullable();

            Create.UniqueConstraint()
                .OnTable("users")
                .Columns("email", "deleted_at");

            Create.Index().OnTable("users")
                .OnColumn("resource_key").Ascending()
                .OnColumn("deleted_at").Ascending();
        }

        public override void Down()
        {
            Delete.Table("users");
        }
    }
}