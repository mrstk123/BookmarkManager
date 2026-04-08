using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces.Queries;
using BookmarkManager.Infrastructure.Connection;
using Dapper;

namespace BookmarkManager.Infrastructure.Queries;

public class FolderQueries : IFolderQueries
{
    private readonly IConnectionFactory _connectionFactory;

    public FolderQueries(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<FolderDto?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Folders WHERE Id = @id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<FolderDto>(sql, new { Id = id });
    }

    public async Task<IEnumerable<FolderDto>> GetByUserIdAsync(int userId)
    {
        var sql = "SELECT * FROM Folders WHERE UserId = @userId";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryAsync<FolderDto>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<FolderDto>> GetAllAsync()
    {
        var sql = "SELECT * FROM Folders";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryAsync<FolderDto>(sql);
    }

    public async Task<FolderDto?> GetByNameAsync(int userId, string name)
    {
        var sql = "SELECT * FROM Folders WHERE UserId = @UserId AND Name = @Name";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<FolderDto>(sql, new { UserId = userId, Name = name });
    }
}
