using BookmarkManager.Application.DTOs;

namespace BookmarkManager.Application.Interfaces.Services;

public interface ITagService
{
    Task<TagDto?> GetTagByIdAsync(int id);
    Task<IEnumerable<TagDto>> GetTagsByUserIdAsync(int userId);
    Task<TagDto> AddAsync(CreateTagRequest request);
    Task UpdateAsync(int id, UpdateTagRequest request);
    Task<int> DeleteAsync(int id);
}
