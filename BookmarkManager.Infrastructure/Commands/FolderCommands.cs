using BookmarkManager.Domain.Entities;
using BookmarkManager.Application.Interfaces.Commands;
using Dapper;
using Npgsql;

namespace BookmarkManager.Infrastructure.Commands;

public class FolderCommands : IFolderCommands
{
    private readonly IConnectionFactory _connectionFactory;

    public FolderCommands(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> AddAsync(Folder folder)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        var sql = conn is NpgsqlConnection
            ? """
                INSERT INTO folders (userid, name, createdat, updatedat)
                VALUES (@UserId, @Name, @CreatedAt, @UpdatedAt)
                RETURNING id;
              """
            : @"
                INSERT INTO Folders (UserId, Name, CreatedAt, UpdatedAt)
                VALUES (@UserId, @Name, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

        return await conn.ExecuteScalarAsync<int>(sql, folder);
    }

    public async Task<int> UpdateAsync(Folder folder)
    {
        var sql = @"
            UPDATE Folders 
            SET Name = @Name,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.ExecuteAsync(sql, folder);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM Folders WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.ExecuteAsync(sql, new { Id = id });
    }
}
