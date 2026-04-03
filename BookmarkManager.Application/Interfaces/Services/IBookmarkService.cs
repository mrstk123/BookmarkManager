using BookmarkManager.Application.DTOs;

namespace BookmarkManager.Application.Interfaces.Services;

public interface IBookmarkService
{
    Task<BookmarkDto?> GetBookmarkByIdAsync(int id);
    Task<IEnumerable<BookmarkDto>> GetBookmarksByUserIdAsync(int userId);
    Task<IEnumerable<BookmarkDto>> GetFavoritesByUserIdAsync(int userId);
    Task<IEnumerable<BookmarkDto>> GetByFolderAsync(int userId, string folderName);
    Task<IEnumerable<BookmarkDto>> GetByTagAsync(int userId, string tagName);
    Task<BookmarkDto> AddAsync(CreateBookmarkRequest request);
    Task<BookmarkDto> UpdateAsync(int id, UpdateBookmarkRequest request);
    Task<int> DeleteAsync(int id);
    Task<int> ToggleFavoriteAsync(int id, int userId);
    Task<int> RecordVisitAsync(int id);
}
