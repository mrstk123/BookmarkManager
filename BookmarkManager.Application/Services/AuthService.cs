using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Exceptions;
using BookmarkManager.Application.Interfaces.Commands;
using BookmarkManager.Application.Interfaces.Queries;
using BookmarkManager.Application.Interfaces.Security;
using BookmarkManager.Application.Interfaces.Services;

namespace BookmarkManager.Application.Services;

/// <summary>
/// Orchestrates all authentication and user-management business logic.
/// This service owns: email uniqueness checks, password hashing/verification,
/// and JWT generation. Infrastructure is only responsible for raw DB reads/writes.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAuthQueries _authQueries;
    private readonly IAuthCommands _authCommands;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IAuthQueries authQueries,
        IAuthCommands authCommands,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _authQueries = authQueries;
        _authCommands = authCommands;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Business rule: email must be unique
        var emailTaken = await _authQueries.EmailExistsAsync(request.Email);
        if (emailTaken)
            throw new BusinessRuleException("Email is already taken.");

        // Hash password before persisting
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Extract username from email (part before @) if no explicit username provided
        var userName = string.IsNullOrWhiteSpace(request.UserName)
            ? request.Email.Split('@')[0]
            : request.UserName;

        // Delegate the raw insert to Infrastructure
        var user = await _authCommands.CreateUserAsync(userName, request.FullName, request.Email, passwordHash);

        // Generate JWT
        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FullName,
            Email = user.Email,
            Token = token
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _authQueries.GetUserByEmailOrUsernameAsync(request.Email);

        // Use a single generic error to prevent user enumeration
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new BusinessRuleException("Invalid credentials.");

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FullName,
            Email = user.Email,
            Token = token
        };
    }

    public async Task<bool> CheckEmailAsync(string email)
    {
        return await _authQueries.EmailExistsAsync(email);
    }

    public async Task<UserDto> GetProfileAsync(int userId)
    {
        var user = await _authQueries.GetProfileAsync(userId);
        return user ?? throw new KeyNotFoundException($"User {userId} not found.");
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        return await _authCommands.UpdateProfileAsync(userId, request);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        // Business rule: verify current password before allowing change
        var user = await _authQueries.GetUserByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User {userId} not found.");

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            throw new BusinessRuleException("Current password is incorrect.");

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        await _authCommands.ChangePasswordAsync(userId, newHash);
    }
}