using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BookmarkManager.WebAPI.Controllers;

/// <summary>
/// Base controller that provides ownership-verification helpers.
/// All authenticated controllers should inherit from this instead of ControllerBase directly.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Returns the authenticated user's ID extracted from the JWT Sub claim.
    /// </summary>
    protected int GetCurrentUserId()
    {
        var raw = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (raw == null || !int.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("User identity could not be determined.");

        return id;
    }

    /// <summary>
    /// Returns 403 Forbidden if <paramref name="resourceOwnerId"/> does not match the
    /// authenticated user's ID. Call this before processing any request that accesses
    /// user-scoped data by a route parameter.
    /// </summary>
    protected IActionResult? EnforceOwnership(int resourceOwnerId)
    {
        if (resourceOwnerId != GetCurrentUserId())
            return Forbid();

        return null; // ownership confirmed, caller may proceed
    }
}
