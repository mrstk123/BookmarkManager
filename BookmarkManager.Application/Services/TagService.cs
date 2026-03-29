using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Services;

public class TagService : ITagService
{
    private readonly ITagQueries _tagQueries;
    private readonly ITagCommands _tagCommands;

    public TagService(ITagQueries tagQueries, ITagCommands tagCommands)
    {
        _tagQueries = tagQueries;
        _tagCommands = tagCommands;
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
        var tag = new Tag
        {
            UserId = request.UserId,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
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
            UpdatedAt = DateTime.UtcNow
        };

        await _tagCommands.UpdateAsync(tag);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _tagCommands.DeleteAsync(id);
    }
}
