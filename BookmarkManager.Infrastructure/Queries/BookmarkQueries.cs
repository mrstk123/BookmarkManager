using System.Data;
using System.Linq;
using BookmarkManager.Infrastructure;
using Dapper;
using BookmarkManager.Application.Interfaces;
using BookmarkManager.Application.DTOs;
using Npgsql;

namespace BookmarkManager.Infrastructure.Queries;

public class BookmarkQueries : IBookmarkQueries
{
    private class BookmarkTagResult
    {
        public int BookmarkId { get; set; }
        public required string TagName { get; set; }
    }
    private readonly IConnectionFactory _connectionFactory;

    public BookmarkQueries(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<BookmarkDto?> GetByIdAsync(int id){
        var sql = "SELECT * FROM Bookmarks WHERE Id = @id";
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<BookmarkDto>(sql, new { Id = id });
    }
    
    public async Task<IEnumerable<BookmarkDto>> GetByUserIdAsync(int userId)
    {
        var sql = @"
            SELECT 
                b.Id, b.UserId, b.FolderId, b.Title, b.Url, b.IsFavorite, b.IconUrl, 
                b.ImportHash, b.LastVisitedAt, b.CreatedAt, b.UpdatedAt,
                f.Name AS FolderName 
            FROM Bookmarks b 
            LEFT JOIN Folders f ON b.FolderId = f.Id 
            WHERE b.UserId = @userId;

            SELECT 
                bt.BookmarkId, t.Name AS TagName 
            FROM Bookmarks b
            INNER JOIN BookmarkTags bt ON b.Id = bt.BookmarkId 
            INNER JOIN Tags t ON bt.TagId = t.Id 
            WHERE b.UserId = @userId;
        ";

        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        using var multi = await conn.QueryMultipleAsync(sql, new { UserId = userId });
        var bookmarks = (await multi.ReadAsync<BookmarkDto>()).ToList();
        var tags = await multi.ReadAsync<BookmarkTagResult>();

        var tagsByBookmarkId = tags
            .GroupBy(t => t.BookmarkId)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(t => t.TagName).ToList()
            );

        foreach (var bookmark in bookmarks)
        {
            if (tagsByBookmarkId.TryGetValue(bookmark.Id, out var tagNames))
            {
                bookmark.Tags = tagNames;
            }
            else
            {
                bookmark.Tags = new List<string>();
            }
        }

        return bookmarks;
    }

    public async Task<IEnumerable<BookmarkDto>> GetFavoritesByUserIdAsync(int userId)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        var isFav = conn is NpgsqlConnection ? "b.isfavorite = true" : "b.IsFavorite = 1";

        var sql = $@"
            SELECT 
                b.Id, b.UserId, b.FolderId, b.Title, b.Url, b.IsFavorite, b.IconUrl, 
                b.ImportHash, b.LastVisitedAt, b.CreatedAt, b.UpdatedAt,
                f.Name AS FolderName 
            FROM Bookmarks b 
            LEFT JOIN Folders f ON b.FolderId = f.Id 
            WHERE b.UserId = @userId AND {isFav};

            SELECT 
                bt.BookmarkId, t.Name AS TagName 
            FROM Bookmarks b
            INNER JOIN BookmarkTags bt ON b.Id = bt.BookmarkId 
            INNER JOIN Tags t ON bt.TagId = t.Id 
            WHERE b.UserId = @userId AND {isFav};
        ";

        using var multi = await conn.QueryMultipleAsync(sql, new { UserId = userId });
        var bookmarks = (await multi.ReadAsync<BookmarkDto>()).ToList();
        var tags = await multi.ReadAsync<BookmarkTagResult>();

        var tagsByBookmarkId = tags
            .GroupBy(t => t.BookmarkId)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(t => t.TagName).ToList()
            );

        foreach (var bookmark in bookmarks)
        {
            if (tagsByBookmarkId.TryGetValue(bookmark.Id, out var tagNames))
            {
                bookmark.Tags = tagNames;
            }
            else
            {
                bookmark.Tags = new List<string>();
            }
        }

        return bookmarks;
                
    }

    public async Task<IEnumerable<BookmarkDto>> GetByFolderAsync(int userId, string folderName)
    {
        var sql = @"
            SELECT 
                b.Id, b.UserId, b.FolderId, b.Title, b.Url, b.IsFavorite, b.IconUrl, 
                b.ImportHash, b.LastVisitedAt, b.CreatedAt, b.UpdatedAt,
                f.Name AS FolderName 
            FROM Bookmarks b 
            INNER JOIN Folders f ON b.FolderId = f.Id 
            WHERE b.UserId = @userId AND f.Name = @folderName;

            SELECT 
                bt.BookmarkId, t.Name AS TagName 
            FROM Bookmarks b
            INNER JOIN Folders f ON b.FolderId = f.Id
            INNER JOIN BookmarkTags bt ON b.Id = bt.BookmarkId 
            INNER JOIN Tags t ON bt.TagId = t.Id 
            WHERE b.UserId = @userId AND f.Name = @folderName;
        ";

        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        using var multi = await conn.QueryMultipleAsync(sql, new { UserId = userId, FolderName = folderName });
        var bookmarks = (await multi.ReadAsync<BookmarkDto>()).ToList();
        var tags = await multi.ReadAsync<BookmarkTagResult>();

        var tagsByBookmarkId = tags
            .GroupBy(t => t.BookmarkId)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(t => t.TagName).ToList()
            );

        foreach (var bookmark in bookmarks)
        {
            if (tagsByBookmarkId.TryGetValue(bookmark.Id, out var tagNames))
            {
                bookmark.Tags = tagNames;
            }
            else
            {
                bookmark.Tags = new List<string>();
            }
        }

        return bookmarks;
    }

    public async Task<IEnumerable<BookmarkDto>> GetByTagAsync(int userId, string tagName)
    {
        var sql = @"
            SELECT 
                b.Id, b.UserId, b.FolderId, b.Title, b.Url, b.IsFavorite, b.IconUrl, 
                b.ImportHash, b.LastVisitedAt, b.CreatedAt, b.UpdatedAt,
                f.Name AS FolderName 
            FROM Bookmarks b 
            LEFT JOIN Folders f ON b.FolderId = f.Id 
            INNER JOIN BookmarkTags bt ON b.Id = bt.BookmarkId 
            INNER JOIN Tags t ON bt.TagId = t.Id 
            WHERE b.UserId = @userId AND t.Name = @tagName;

            SELECT 
                bt.BookmarkId, t.Name AS TagName 
            FROM Bookmarks b
            INNER JOIN BookmarkTags bt ON b.Id = bt.BookmarkId 
            INNER JOIN Tags t ON bt.TagId = t.Id 
            WHERE b.UserId = @userId AND b.Id IN (
                SELECT b2.Id FROM Bookmarks b2
                INNER JOIN BookmarkTags bt2 ON b2.Id = bt2.BookmarkId
                INNER JOIN Tags t2 ON bt2.TagId = t2.Id
                WHERE b2.UserId = @userId AND t2.Name = @tagName
            );
        ";

        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        using var multi = await conn.QueryMultipleAsync(sql, new { UserId = userId, TagName = tagName });
        var bookmarks = (await multi.ReadAsync<BookmarkDto>()).ToList();
        var tags = await multi.ReadAsync<BookmarkTagResult>();

        var tagsByBookmarkId = tags
            .GroupBy(t => t.BookmarkId)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(t => t.TagName).ToList()
            );

        foreach (var bookmark in bookmarks)
        {
            if (tagsByBookmarkId.TryGetValue(bookmark.Id, out var tagNames))
            {
                bookmark.Tags = tagNames;
            }
            else
            {
                bookmark.Tags = new List<string>();
            }
        }

        return bookmarks;
    }
}
