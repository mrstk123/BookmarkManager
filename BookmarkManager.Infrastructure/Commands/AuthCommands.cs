using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces.Commands;
using BookmarkManager.Domain.Entities;
using Dapper;
using Npgsql;

namespace BookmarkManager.Infrastructure.Commands;

/// <summary>
/// Infrastructure implementation of IAuthCommands — pure DB writes using Dapper.
/// All business logic (uniqueness checks, password hashing, JWT generation) lives
/// in the Application layer (AuthService).
/// </summary>
public class AuthCommands : IAuthCommands
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly TimeProvider _timeProvider;

    public AuthCommands(IConnectionFactory connectionFactory, TimeProvider timeProvider)
    {
        _connectionFactory = connectionFactory;
        _timeProvider = timeProvider;
    }

    public async Task<User> CreateUserAsync(string userName, string fullName, string email, string passwordHash)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        var isNpgsql = conn is NpgsqlConnection;
        var sql = isNpgsql
            ? """
                INSERT INTO Users (username, fullname, email, passwordhash, createdat, updatedat)
                VALUES (@UserName, @FullName, @Email, @PasswordHash, @CreatedAt, @UpdatedAt)
                RETURNING id;
              """
            : """
                INSERT INTO Users (UserName, FullName, Email, PasswordHash, CreatedAt, UpdatedAt)
                VALUES (@UserName, @FullName, @Email, @PasswordHash, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);
              """;

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var userId = await conn.ExecuteScalarAsync<int>(sql, new
        {
            UserName = userName,
            FullName = fullName,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = now,
            UpdatedAt = now
        });

        return new User
        {
            Id = userId,
            UserName = userName,
            FullName = fullName,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var getUserSql = "SELECT Id, UserName, FullName, Email, CreatedAt, UpdatedAt FROM Users WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        var user = await conn.QueryFirstOrDefaultAsync<User>(getUserSql, new { Id = userId });

        if (user == null)
            throw new KeyNotFoundException($"User {userId} not found.");

        var updateSql = "UPDATE Users SET FullName = @FullName, UpdatedAt = @UpdatedAt WHERE Id = @Id";
        var updatedAt = _timeProvider.GetUtcNow().UtcDateTime;
        await conn.ExecuteAsync(updateSql, new { Id = userId, FullName = request.FullName, UpdatedAt = updatedAt });

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = request.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task ChangePasswordAsync(int userId, string newPasswordHash)
    {
        var sql = "UPDATE Users SET PasswordHash = @PasswordHash, UpdatedAt = @UpdatedAt WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        await conn.ExecuteAsync(sql, new
        {
            Id = userId,
            PasswordHash = newPasswordHash,
            UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime
        });
    }
}