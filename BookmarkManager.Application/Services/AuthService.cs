using System;
using System.Threading.Tasks;
using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserQueries _userQueries;
    private readonly IUserCommands _userCommands;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserQueries userQueries, 
        IUserCommands userCommands, 
        IPasswordHasher passwordHasher, 
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userQueries = userQueries;
        _userCommands = userCommands;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check if user already exists
        var existingUser = await _userQueries.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new Exception("Email is already taken.");
        }

        // Hash password and create user
        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var userId = await _userCommands.AddAsync(user);
        user.Id = userId;

        // Generate Token
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
        var existingUser = await _userQueries.GetByEmailAsync(email);
        return existingUser != null;
    }

    public async Task<UserDto> GetProfileAsync(int userId)
    {
        var user = await _userQueries.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        return user;
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _userQueries.GetUserEntityByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        user.FullName = request.FullName;
        user.UpdatedAt = DateTime.UtcNow;
        await _userCommands.UpdateAsync(user);

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _userQueries.GetUserEntityByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var isCurrentPasswordValid = _passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash);
        if (!isCurrentPasswordValid)
        {
            throw new Exception("Current password is incorrect.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _userCommands.UpdateAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userQueries.GetUserEntityByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("Invalid credentials.");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new Exception("Invalid credentials.");
        }

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
}
