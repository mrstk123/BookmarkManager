using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkManager.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;

    public UserController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("profile/{userId}")]
    public async Task<IActionResult> GetProfile(int userId)
    {
        var profile = await _authService.GetProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPut("profile/{userId}")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UpdateProfileRequest request)
    {
        var profile = await _authService.UpdateProfileAsync(userId, request);
        return Ok(profile);
    }

    [HttpPut("password/{userId}")]
    public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordRequest request)
    {
        await _authService.ChangePasswordAsync(userId, request);
        return Ok(new { success = true });
    }
}
