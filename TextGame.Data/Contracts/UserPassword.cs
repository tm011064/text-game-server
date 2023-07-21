namespace TextGame.Data.Contracts;

public readonly record struct UserPassword(
    byte[] InitializationVector,
    byte[] Salt,
    int Iterations,
    string Data,
    byte[] CipherBytes);



//Create.Table("users")
//                .WithColumn("id").AsInt64().PrimaryKey().Identity()
//                .WithColumn("key").AsGuid()
//                .WithColumn("created_by").AsString(256).NotNullable()
//                .WithColumn("deleted_at").AsDateTimeOffset().Nullable()
//                .WithColumn("deleted_by").AsString(256).Nullable()
//                .WithColumn("email").AsString(256).NotNullable().Unique()
//                .WithColumn("password_initialization_vector").AsBinary().NotNullable()
//                .WithColumn("password_salt").AsBinary().NotNullable()
//                .WithColumn("password_iterations").AsInt32().NotNullable()
//                .WithColumn("password_data").AsString(256)
//                .WithColumn("password_cipher_bytes").AsBinary();
