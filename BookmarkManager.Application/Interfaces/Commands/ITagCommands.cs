using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Interfaces;

public interface ITagCommands
{
    Task<int> AddAsync(Tag tag);
    Task<int> UpdateAsync(Tag tag);
    Task<int> DeleteAsync(int id);
}
