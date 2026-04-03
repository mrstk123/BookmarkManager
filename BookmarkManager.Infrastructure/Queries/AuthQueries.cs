using Dapper;
using BookmarkManager.Application.Interfaces.Queries;
using BookmarkManager.Application.DTOs;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Infrastructure.Queries;

/// <summary>
/// Infrastructure implementation of IAuthQueries using Dapper.
/// </summary>
public class AuthQueries : IAuthQueries
{
    private readonly IConnectionFactory _connectionFactory;

    public AuthQueries(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        using var conn = _connectionFactory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { Email = email }) > 0;
    }

    public async Task<UserDto?> GetProfileAsync(int userId)
    {
        var sql = "SELECT Id, UserName, FullName, Email, CreatedAt FROM Users WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<UserDto>(sql, new { Id = userId });
    }

    public async Task<User?> GetUserByEmailOrUsernameAsync(string emailOrUsername)
    {
        // Allow login with either email or username
        var sql = "SELECT Id, UserName, FullName, Email, PasswordHash, CreatedAt, UpdatedAt FROM Users WHERE Email = @EmailOrUsername OR UserName = @EmailOrUsername";
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { EmailOrUsername = emailOrUsername });
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        // Retrieves user by ID with PasswordHash for credential verification.
        var sql = "SELECT Id, UserName, FullName, Email, PasswordHash, CreatedAt, UpdatedAt FROM Users WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { Id = userId });
    }
}
