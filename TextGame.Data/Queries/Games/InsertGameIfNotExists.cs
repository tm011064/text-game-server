﻿using Dapper;
using System.Data;
using TextGame.Data.Contracts;

namespace TextGame.Data.Queries.Users;

public class InsertGameIfNotExists : IQuery<long>
{
    private readonly string key;

    public InsertGameIfNotExists(string key)
    {
        this.key = key;
    }

    public Task<long> Execute(IDbConnection connection, AuthTicket ticket)
    {
        return connection.QuerySingleAsync<long>($@"
            insert into games (
                resource_key,
                created_at,
                created_by
            )
            values (
                @{nameof(key)},
                @{nameof(ticket.CreatedAt)},
                @{nameof(ticket.CreatedBy)}
            )
            on conflict do nothing;

            select
                id
            from
                games
            where
                resource_key = @{nameof(key)}
                and deleted_at is null;",
            new
            {
                key,
                ticket.CreatedAt,
                ticket.CreatedBy
            });
    }
}
