using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using Dapper;

namespace BookmarkManager.Infrastructure.Queries;

public class TagQueries : ITagQueries
{
    private readonly IConnectionFactory _connectionFactory;

    public TagQueries(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<TagDto?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Tags WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<TagDto>(sql, new { Id = id });
    }

    public async Task<TagDto?> GetByNameAsync(int userId, string name)
    {
        var sql = "SELECT * FROM Tags WHERE UserId = @UserId AND Name = @Name";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<TagDto>(sql, new { UserId = userId, Name = name });
    }

    public async Task<IEnumerable<TagDto>> GetByUserIdAsync(int userId)
    {
        var sql = "SELECT * FROM Tags WHERE UserId = @UserId";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryAsync<TagDto>(sql, new { UserId = userId });
    }
}
