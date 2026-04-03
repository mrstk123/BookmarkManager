using BookmarkManager.Application.DTOs;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Interfaces.Queries;

/// <summary>
/// Query-side operations for authentication and user queries.
/// </summary>
public interface IAuthQueries
{
    /// <summary>
    /// Checks if an email is already registered.
    /// </summary>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Gets user profile by user ID.
    /// </summary>
    Task<UserDto?> GetProfileAsync(int userId);

    /// <summary>
    /// Retrieves the full User entity by email
    /// </summary>
    Task<User?> GetUserByEmailOrUsernameAsync(string emailOrUsername);

    /// <summary>
    /// Retrieves the full User entity by ID 
    /// </summary>
    Task<User?> GetUserByIdAsync(int userId);
}
