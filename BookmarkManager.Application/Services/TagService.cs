using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces.Commands;
using BookmarkManager.Application.Interfaces.Queries;
using BookmarkManager.Application.Interfaces.Services;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Services;

public class TagService : ITagService
{
    private readonly ITagQueries _tagQueries;
    private readonly ITagCommands _tagCommands;
    private readonly TimeProvider _timeProvider;

    public TagService(ITagQueries tagQueries, ITagCommands tagCommands, TimeProvider timeProvider)
    {
        _tagQueries = tagQueries;
        _tagCommands = tagCommands;
        _timeProvider = timeProvider;
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        return await _tagQueries.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TagDto>> GetTagsByUserIdAsync(int userId)
    {
        return await _tagQueries.GetByUserIdAsync(userId);
    }

    public async Task<TagDto> AddAsync(CreateTagRequest request)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var tag = new Tag
        {
            UserId = request.UserId,
            Name = request.Name,
            CreatedAt = now,
            UpdatedAt = now
        };

        var id = await _tagCommands.AddAsync(tag);

        return new TagDto
        {
            Id = id,
            UserId = request.UserId,
            Name = request.Name,
            CreatedAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt
        };
    }

    public async Task UpdateAsync(int id, UpdateTagRequest request)
    {
        var tag = new Tag
        {
            Id = id,
            Name = request.Name,
            UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime
        };

        await _tagCommands.UpdateAsync(tag);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _tagCommands.DeleteAsync(id);
    }
}
