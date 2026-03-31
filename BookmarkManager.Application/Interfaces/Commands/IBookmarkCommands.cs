using System.Threading.Tasks;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Interfaces.Commands;

public interface IBookmarkCommands
{
    Task<int> AddAsync(Bookmark bookmark);
    Task<int> UpdateAsync(Bookmark bookmark);
    Task<int> DeleteAsync(int id);
    Task<int> ToggleFavoriteAsync(int id, int userId);
    Task UpdateTagsAsync(int bookmarkId, IEnumerable<int> tagIds);
}
