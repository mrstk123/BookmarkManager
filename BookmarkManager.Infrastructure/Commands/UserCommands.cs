using BookmarkManager.Domain.Entities;
using BookmarkManager.Application.Interfaces.Commands;
using Dapper;
using Npgsql;

namespace BookmarkManager.Infrastructure.Commands;

public class UserCommands : IUserCommands
{
    private readonly BookmarkManager.Infrastructure.IConnectionFactory _connectionFactory;

    public UserCommands(BookmarkManager.Infrastructure.IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> AddAsync(User user)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        var sql = conn is NpgsqlConnection
            ? """
                INSERT INTO Users (username, fullname, email, passwordhash, createdat, updatedat)
                VALUES (@UserName, @FullName, @Email, @PasswordHash, @CreatedAt, @UpdatedAt)
                RETURNING id;
              """
            : @"
                INSERT INTO Users (UserName, FullName, Email, PasswordHash, CreatedAt, UpdatedAt)
                VALUES (@UserName, @FullName, @Email, @PasswordHash, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

        return await conn.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<int> UpdateAsync(User user)
    {
        var sql = @"
            UPDATE Users 
            SET UserName = @UserName, 
                FullName = @FullName, 
                Email = @Email, 
                PasswordHash = @PasswordHash,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.ExecuteAsync(sql, user);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM Users WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.ExecuteAsync(sql, new { Id = id });
    }
}
