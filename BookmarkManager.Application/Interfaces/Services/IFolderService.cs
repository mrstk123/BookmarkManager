using BookmarkManager.Application.DTOs;

namespace BookmarkManager.Application.Interfaces.Services;

public interface IFolderService
{
    Task<FolderDto?> GetFolderByIdAsync(int id);
    Task<IEnumerable<FolderDto>> GetFoldersByUserIdAsync(int userId);
    Task<FolderDto> AddAsync(CreateFolderRequest request);
    Task UpdateAsync(int id, UpdateFolderRequest request);
    Task<int> DeleteAsync(int id);
}
