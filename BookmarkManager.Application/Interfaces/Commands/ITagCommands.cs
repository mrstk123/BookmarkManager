using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Interfaces.Commands;

public interface ITagCommands
{
    Task<int> AddAsync(Tag tag);
    Task<int> UpdateAsync(Tag tag);
    Task<int> DeleteAsync(int id);
}
