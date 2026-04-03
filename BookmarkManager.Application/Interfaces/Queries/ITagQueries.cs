using BookmarkManager.Application.DTOs;

namespace BookmarkManager.Application.Interfaces.Queries;

public interface ITagQueries
{
    Task<TagDto?> GetByIdAsync(int id);
    Task<TagDto?> GetByNameAsync(int userId, string name);
    Task<IEnumerable<TagDto>> GetByUserIdAsync(int userId);
}
