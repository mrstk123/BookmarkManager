using BookmarkManager.Application.DTOs;

namespace BookmarkManager.Application.Interfaces.Queries;

public interface IBookmarkQueries
{
    Task<BookmarkDto?> GetByIdAsync(int id);
    Task<IEnumerable<BookmarkDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<BookmarkDto>> GetFavoritesByUserIdAsync(int userId);
    Task<IEnumerable<BookmarkDto>> GetByFolderAsync(int userId, string folderName);
    Task<IEnumerable<BookmarkDto>> GetByTagAsync(int userId, string tagName);
}
