using BookmarkManager.Application.Interfaces;
using BookmarkManager.Domain.Entities;
using Dapper;
using Npgsql;

namespace BookmarkManager.Infrastructure.Commands;

public class TagCommands : ITagCommands
{
    private readonly IConnectionFactory _connectionFactory;

    public TagCommands(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> AddAsync(Tag tag)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        var sql = conn is NpgsqlConnection
            ? """
                INSERT INTO tags (userid, name, createdat, updatedat)
                VALUES (@UserId, @Name, @CreatedAt, @UpdatedAt)
                RETURNING id;
              """
            : @"
                INSERT INTO Tags (UserId, Name, CreatedAt, UpdatedAt)
                VALUES (@UserId, @Name, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

        return await conn.ExecuteScalarAsync<int>(sql, tag);
    }

    public async Task<int> UpdateAsync(Tag tag)
    {
        var sql = @"
            UPDATE Tags 
            SET Name = @Name,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.ExecuteAsync(sql, tag);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM Tags WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.ExecuteAsync(sql, new { Id = id });
    }
}
