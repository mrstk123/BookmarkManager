using BookmarkManager.Application.DTOs;

namespace BookmarkManager.Application.Interfaces.Queries;

public interface IUserQueries
{
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<BookmarkManager.Domain.Entities.User?> GetUserEntityByEmailAsync(string email);
    Task<BookmarkManager.Domain.Entities.User?> GetUserEntityByIdAsync(int id);
}
