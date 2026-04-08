using BookmarkManager.Domain.Entities;
using BookmarkManager.Application.Interfaces.Commands;
using BookmarkManager.Infrastructure.Connection;
using Dapper;
using Npgsql;

namespace BookmarkManager.Infrastructure.Commands;

public class BookmarkCommands : IBookmarkCommands
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly TimeProvider _timeProvider;

    public BookmarkCommands(IConnectionFactory connectionFactory, TimeProvider timeProvider)
    {
        _connectionFactory = connectionFactory;
        _timeProvider = timeProvider;
    }

    public async Task<int> AddAsync(Bookmark bookmark)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        var sql = conn is NpgsqlConnection
            ? """
                INSERT INTO bookmarks (userid, folderid, title, url, isfavorite, iconurl, importhash, lastvisitedat, createdat, updatedat)
                VALUES (@UserId, @FolderId, @Title, @Url, @IsFavorite, @IconUrl, @ImportHash, @LastVisitedAt, @CreatedAt, @UpdatedAt)
                RETURNING id;
              """
            : """
                INSERT INTO Bookmarks (UserId, FolderId, Title, Url, IsFavorite, IconUrl, ImportHash, LastVisitedAt, CreatedAt, UpdatedAt)
                VALUES (@UserId, @FolderId, @Title, @Url, @IsFavorite, @IconUrl, @ImportHash, @LastVisitedAt, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);
              """;

        return await conn.ExecuteScalarAsync<int>(sql, bookmark);
    }

    public async Task<int> UpdateAsync(Bookmark bookmark)
    {
        var sql = """
            UPDATE Bookmarks 
            SET FolderId = @FolderId, 
                Title = @Title, 
                Url = @Url, 
                IsFavorite = @IsFavorite, 
                IconUrl = @IconUrl, 
                ImportHash = @ImportHash,
                LastVisitedAt = @LastVisitedAt, 
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id
        """;
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.ExecuteAsync(sql, bookmark);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM Bookmarks WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<int> ToggleFavoriteAsync(int id, int userId)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        var sql = conn is NpgsqlConnection
            ? "UPDATE Bookmarks SET IsFavorite = NOT IsFavorite, UpdatedAt = @UpdatedAt WHERE Id = @Id AND UserId = @UserId"
            : "UPDATE Bookmarks SET IsFavorite = IsFavorite ^ 1, UpdatedAt = @UpdatedAt WHERE Id = @Id AND UserId = @UserId";

        return await conn.ExecuteAsync(sql, new { Id = id, UserId = userId, UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime });
    }

    public async Task UpdateTagsAsync(int bookmarkId, IEnumerable<int> tagIds)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            var deleteSql = "DELETE FROM BookmarkTags WHERE BookmarkId = @BookmarkId";
            await conn.ExecuteAsync(deleteSql, new { BookmarkId = bookmarkId }, transaction);

            if (tagIds != null && tagIds.Any())
            {
                var insertSql = "INSERT INTO BookmarkTags (BookmarkId, TagId) VALUES (@BookmarkId, @TagId)";
                var parameters = tagIds.Select(tagId => new { BookmarkId = bookmarkId, TagId = tagId });
                await conn.ExecuteAsync(insertSql, parameters, transaction);
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<int> RecordVisitAsync(int id)
    {
        var sql = "UPDATE Bookmarks SET LastVisitedAt = @LastVisitedAt WHERE Id = @Id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.ExecuteAsync(sql, new { Id = id, LastVisitedAt = _timeProvider.GetUtcNow().UtcDateTime });
    }
}
