﻿using Dapper;
using System.Data;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.UserAccounts;

public class InsertUserAccountIfNotExists : IQuery<long>
{
    private readonly long userId;

    private readonly string key;

    private readonly string name;

    public InsertUserAccountIfNotExists(
        IUser user,
        string key,
        string name)
    {
        userId = user.Id;
        this.key = key;
        this.name = name;
    }

    public Task<long> Execute(IDbConnection connection, AuthTicket ticket)
    {
        return connection.QuerySingleAsync<long>($@"
            insert into user_accounts (
                resource_key,
                user_id,
                name,
                created_at,
                created_by
            )
            values (
                @{nameof(key)},
                @{nameof(userId)},
                @{nameof(name)},
                @{nameof(ticket.CreatedAt)},
                @{nameof(ticket.CreatedBy)}
            )
            on conflict do nothing;

            select
                id
            from
                user_accounts
            where
                resource_key = @{nameof(key)}
                and deleted_at is null;",
            new
            {
                key,
                userId,
                name,
                ticket.CreatedAt,
                ticket.CreatedBy
            });
    }
}
