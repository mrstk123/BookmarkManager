using BookmarkManager.Application.DTOs;

namespace BookmarkManager.Application.Interfaces.Queries;

public interface IFolderQueries
{
    Task<FolderDto?> GetByIdAsync(int id);
    Task<IEnumerable<FolderDto>> GetByUserIdAsync(int userId);
    Task<FolderDto?> GetByNameAsync(int userId, string name);
}
