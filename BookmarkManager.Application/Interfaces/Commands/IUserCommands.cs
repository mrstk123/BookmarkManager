using System.Threading.Tasks;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Interfaces;

public interface IUserCommands
{
    Task<int> AddAsync(User user);
    Task<int> UpdateAsync(User user);
    Task<int> DeleteAsync(int id);
}
