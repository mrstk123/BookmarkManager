using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Exceptions;
using BookmarkManager.Application.Interfaces;
using BookmarkManager.Application.Interfaces.Commands;
using BookmarkManager.Application.Interfaces.Queries;
using BookmarkManager.Domain.Entities;
using BookmarkManager.Domain.Interfaces;

namespace BookmarkManager.Application.Services;

/// <summary>
/// Service for managing bookmarks, coordinating between queries, commands, and domain services.
/// </summary>
public class BookmarkService : IBookmarkService
{
    private readonly IBookmarkQueries _bookmarkQueries;
    private readonly IBookmarkCommands _bookmarkCommands;
    private readonly IFolderQueries _folderQueries;
    private readonly ITagQueries _tagQueries;
    private readonly ITagCommands _tagCommands;
    private readonly IImportHashGenerator _importHashGenerator;

    public BookmarkService(
        IBookmarkQueries bookmarkQueries,
        IBookmarkCommands bookmarkCommands,
        IFolderQueries folderQueries,
        ITagQueries tagQueries,
        ITagCommands tagCommands,
        IImportHashGenerator importHashGenerator)
    {
        _bookmarkQueries = bookmarkQueries;
        _bookmarkCommands = bookmarkCommands;
        _folderQueries = folderQueries;
        _tagQueries = tagQueries;
        _tagCommands = tagCommands;
        _importHashGenerator = importHashGenerator;
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
            ImportHash = _importHashGenerator.Generate(request.Url),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var id = await _bookmarkCommands.AddAsync(bookmark);
        bookmark.Id = id;

        var linkedTags = await ProcessTagsAsync(request.Tags, request.UserId, bookmark.Id);

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
        if (existing == null) throw new InvalidOperationException("Bookmark not found");

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
            ImportHash = _importHashGenerator.Generate(request.Url),
            UpdatedAt = DateTime.UtcNow
        };

        await _bookmarkCommands.UpdateAsync(bookmark);

        var linkedTags = await ProcessTagsAsync(request.Tags, existing.UserId, id);

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

    /// <summary>
    /// Processes tag names: finds existing or creates new tags, then links them to the bookmark.
    /// </summary>
    private async Task<List<string>> ProcessTagsAsync(List<string> tagNames, int userId, int bookmarkId)
    {
        var linkedTags = new List<string>();
        if (tagNames == null || !tagNames.Any()) return linkedTags;

        var tagIdsToLink = new List<int>();
        foreach (var tagName in tagNames)
        {
            var trimmedName = tagName.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName)) continue;

            var existingTag = await _tagQueries.GetByNameAsync(userId, trimmedName);
            if (existingTag != null)
            {
                tagIdsToLink.Add(existingTag.Id);
                linkedTags.Add(existingTag.Name);
            }
            else
            {
                var newTag = new Tag
                {
                    UserId = userId,
                    Name = trimmedName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                var tagId = await _tagCommands.AddAsync(newTag);
                tagIdsToLink.Add(tagId);
                linkedTags.Add(trimmedName);
            }
        }

        if (tagIdsToLink.Any())
        {
            await _bookmarkCommands.UpdateTagsAsync(bookmarkId, tagIdsToLink);
        }

        return linkedTags;
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