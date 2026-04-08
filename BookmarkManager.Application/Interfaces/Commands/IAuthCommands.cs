using BookmarkManager.Application.DTOs;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Interfaces.Commands;

/// <summary>
/// Command-side operations for authentication — pure DB writes only.
/// </summary>
public interface IAuthCommands
{
    /// <summary>
    /// Inserts a new user row and returns the created User entity.
    /// Accepts a pre-hashed password — hashing must be done by the caller.
    /// </summary>
    Task<User> CreateUserAsync(string userName, string fullName, string email, string passwordHash);

    /// <summary>
    /// Updates user profile information and returns the updated profile DTO.
    /// </summary>
    Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request);

    /// <summary>
    /// Persists a new password hash for the given user.
    /// The caller is responsible for verifying the old password and hashing the new one.
    /// </summary>
    Task ChangePasswordAsync(int userId, string newPasswordHash);
}

