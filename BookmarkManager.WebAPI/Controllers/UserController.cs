using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkManager.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UserController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public UserController(IAuthService authService)
    {
        _authService = authService;
    }

    // GET /api/user/profile
    // The userId comes from the JWT — no route parameter needed, prevents IDOR
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var profile = await _authService.GetProfileAsync(userId);
        return Ok(profile);
    }

    // PUT /api/user/profile
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        var profile = await _authService.UpdateProfileAsync(userId, request);
        return Ok(profile);
    }

    // PUT /api/user/password
    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetCurrentUserId();
        await _authService.ChangePasswordAsync(userId, request);
        return Ok(new { success = true });
    }
}
