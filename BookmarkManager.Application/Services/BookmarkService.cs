using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Services;

public class BookmarkService : IBookmarkService
{
    private readonly IBookmarkQueries _bookmarkQueries;
    private readonly IBookmarkCommands _bookmarkCommands;
    private readonly IFolderQueries _folderQueries;
    private readonly ITagQueries _tagQueries;
    private readonly ITagCommands _tagCommands;

    public BookmarkService(IBookmarkQueries bookmarkQueries, IBookmarkCommands bookmarkCommands, IFolderQueries folderQueries, ITagQueries tagQueries, ITagCommands tagCommands)
    {
        _bookmarkQueries = bookmarkQueries;
        _bookmarkCommands = bookmarkCommands;
        _folderQueries = folderQueries;
        _tagQueries = tagQueries;
        _tagCommands = tagCommands;
    }

    public async Task<BookmarkDto?> GetBookmarkByIdAsync(int id)
    {
        return await _bookmarkQueries.GetByIdAsync(id);
    }

    public async Task<IEnumerable<BookmarkDto>> GetBookmarksByUserIdAsync(int userId)
    {
        return await _bookmarkQueries.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<BookmarkDto>> GetFavoritesByUserIdAsync(int userId)
    {
        return await _bookmarkQueries.GetFavoritesByUserIdAsync(userId);
    }

    public async Task<IEnumerable<BookmarkDto>> GetByFolderAsync(int userId, string folderName)
    {
        return await _bookmarkQueries.GetByFolderAsync(userId, folderName);
    }

    public async Task<IEnumerable<BookmarkDto>> GetByTagAsync(int userId, string tagName)
    {
        return await _bookmarkQueries.GetByTagAsync(userId, tagName);
    }

    public async Task<BookmarkDto> AddAsync(CreateBookmarkRequest request)
    {
        int? folderId = null;
        if (!string.IsNullOrWhiteSpace(request.FolderName))
        {
            var folder = await _folderQueries.GetByNameAsync(request.UserId, request.FolderName);
            folderId = folder?.Id;
        }

        var bookmark = new Bookmark
        {
            UserId = request.UserId,
            Title = request.Title,
            Url = request.Url,
            FolderId = folderId,
            IsFavorite = request.IsFavorite,
            IconUrl = request.IconUrl,
            ImportHash = GenerateImportHash(request.Url),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var id = await _bookmarkCommands.AddAsync(bookmark);
        bookmark.Id = id;

        var linkedTags = new List<string>();
        if (request.Tags != null && request.Tags.Any())
        {
            var tagIdsToLink = new List<int>();
            foreach (var tagName in request.Tags)
            {
                var tn = tagName.Trim();
                if (string.IsNullOrWhiteSpace(tn)) continue;

                var existingTag = await _tagQueries.GetByNameAsync(request.UserId, tn);
                if (existingTag != null)
                {
                    tagIdsToLink.Add(existingTag.Id);
                    linkedTags.Add(existingTag.Name);
                }
                else
                {
                    var newTag = new Tag
                    {
                        UserId = request.UserId,
                        Name = tn,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    var tagId = await _tagCommands.AddAsync(newTag);
                    tagIdsToLink.Add(tagId);
                    linkedTags.Add(tn);
                }
            }

            if (tagIdsToLink.Any())
            {
                await _bookmarkCommands.UpdateTagsAsync(bookmark.Id, tagIdsToLink);
            }
        }

        return new BookmarkDto
        {
            Id = bookmark.Id,
            UserId = bookmark.UserId,
            FolderId = bookmark.FolderId,
            Title = bookmark.Title,
            Url = bookmark.Url,
            IconUrl = bookmark.IconUrl,
            ImportHash = bookmark.ImportHash,
            LastVisitedAt = bookmark.LastVisitedAt,
            CreatedAt = bookmark.CreatedAt,
            UpdatedAt = bookmark.UpdatedAt,
            FolderName = request.FolderName,
            Tags = linkedTags
        };
    }

    public async Task<BookmarkDto> UpdateAsync(int id, UpdateBookmarkRequest request)
    {
        var existing = await _bookmarkQueries.GetByIdAsync(id);
        if (existing == null) throw new Exception("Bookmark not found");

        int? folderId = null;
        if (!string.IsNullOrWhiteSpace(request.FolderName))
        {
            var folder = await _folderQueries.GetByNameAsync(existing.UserId, request.FolderName);
            folderId = folder?.Id;
        }

        var bookmark = new Bookmark
        {
            Id = id,
            Title = request.Title,
            Url = request.Url,
            FolderId = folderId,
            IsFavorite = request.IsFavorite,
            IconUrl = request.IconUrl,
            ImportHash = GenerateImportHash(request.Url),
            UpdatedAt = DateTime.UtcNow
        };

        await _bookmarkCommands.UpdateAsync(bookmark);

        var linkedTags = new List<string>();
        if (request.Tags != null)
        {
            var tagIdsToLink = new List<int>();
            foreach (var tagName in request.Tags)
            {
                var tn = tagName.Trim();
                if (string.IsNullOrWhiteSpace(tn)) continue;

                var existingTag = await _tagQueries.GetByNameAsync(existing.UserId, tn);
                if (existingTag != null)
                {
                    tagIdsToLink.Add(existingTag.Id);
                    linkedTags.Add(existingTag.Name);
                }
                else
                {
                    var newTag = new Tag
                    {
                        UserId = existing.UserId,
                        Name = tn,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    var tagId = await _tagCommands.AddAsync(newTag);
                    tagIdsToLink.Add(tagId);
                    linkedTags.Add(tn);
                }
            }
            await _bookmarkCommands.UpdateTagsAsync(id, tagIdsToLink);
        }

        return new BookmarkDto
        {
            Id = id,
            FolderId = folderId,
            Title = request.Title,
            Url = request.Url,
            IconUrl = request.IconUrl,
            ImportHash = bookmark.ImportHash,
            UpdatedAt = bookmark.UpdatedAt,
            FolderName = request.FolderName,
            Tags = linkedTags
        };
    }

    private static string GenerateImportHash(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(url.Trim().ToLowerInvariant());
        var hashBytes = sha256.ComputeHash(bytes);

        var hashBuilder = new StringBuilder();
        foreach (var b in hashBytes)
        {
            hashBuilder.Append(b.ToString("x2"));
        }

        return hashBuilder.ToString();
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _bookmarkCommands.DeleteAsync(id);
    }

    public async Task<int> ToggleFavoriteAsync(int id, int userId)
    {
        return await _bookmarkCommands.ToggleFavoriteAsync(id, userId);
    }
}
