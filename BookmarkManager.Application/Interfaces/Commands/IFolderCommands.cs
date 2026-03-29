using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Interfaces;

public interface IFolderCommands
{
    Task<int> AddAsync(Folder folder);
    Task<int> UpdateAsync(Folder folder);
    Task<int> DeleteAsync(int id);
}
