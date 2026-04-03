using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkManager.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
public class BookmarkController : ApiControllerBase
{
    private readonly IBookmarkService _bookmarkService;

    public BookmarkController(IBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    // GET /api/bookmark/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBookmark(int id)
    {
        var bookmark = await _bookmarkService.GetBookmarkByIdAsync(id);
        if (bookmark == null) return NotFound();

        // Ownership: only the owner may read their bookmark
        var forbidden = EnforceOwnership(bookmark.UserId);
        if (forbidden != null) return forbidden;

        return Ok(bookmark);
    }

    // GET /api/bookmark/user/{userId}[?folderName=x][?tagName=y]
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetBookmarks(
        int userId,
        [FromQuery] string? folderName,
        [FromQuery] string? tagName)
    {
        var forbidden = EnforceOwnership(userId);
        if (forbidden != null) return forbidden;

        if (!string.IsNullOrWhiteSpace(tagName))
            return Ok(await _bookmarkService.GetByTagAsync(userId, tagName));

        if (!string.IsNullOrWhiteSpace(folderName))
            return Ok(await _bookmarkService.GetByFolderAsync(userId, folderName));

        return Ok(await _bookmarkService.GetBookmarksByUserIdAsync(userId));
    }

    // GET /api/bookmark/favorites/{userId}
    [HttpGet("favorites/{userId:int}")]
    public async Task<IActionResult> GetFavorites(int userId)
    {
        var forbidden = EnforceOwnership(userId);
        if (forbidden != null) return forbidden;

        return Ok(await _bookmarkService.GetFavoritesByUserIdAsync(userId));
    }

    // POST /api/bookmark
    [HttpPost]
    public async Task<IActionResult> AddBookmark([FromBody] CreateBookmarkRequest request)
    {
        // Override UserId from JWT so the client cannot forge ownership
        request.UserId = GetCurrentUserId();

        var bookmark = await _bookmarkService.AddAsync(request);
        return CreatedAtAction(nameof(GetBookmark), new { id = bookmark.Id }, bookmark);
    }

    // PUT /api/bookmark/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBookmark(int id, [FromBody] UpdateBookmarkRequest request)
    {
        // Fetch first to get the owner, then check ownership before updating
        var existing = await _bookmarkService.GetBookmarkByIdAsync(id);
        if (existing == null) return NotFound();

        var forbidden = EnforceOwnership(existing.UserId);
        if (forbidden != null) return forbidden;

        var bookmark = await _bookmarkService.UpdateAsync(id, request);
        return Ok(bookmark);
    }

    // DELETE /api/bookmark/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBookmark(int id)
    {
        var existing = await _bookmarkService.GetBookmarkByIdAsync(id);
        if (existing == null) return NotFound();

        var forbidden = EnforceOwnership(existing.UserId);
        if (forbidden != null) return forbidden;

        await _bookmarkService.DeleteAsync(id);
        return NoContent();
    }

    // PUT /api/bookmark/{id}/toggle-favorite
    [HttpPut("{id:int}/toggle-favorite")]
    public async Task<IActionResult> ToggleFavorite(int id)
    {
        var existing = await _bookmarkService.GetBookmarkByIdAsync(id);
        if (existing == null) return NotFound();

        var forbidden = EnforceOwnership(existing.UserId);
        if (forbidden != null) return forbidden;

        await _bookmarkService.ToggleFavoriteAsync(id, existing.UserId);
        return NoContent();
    }

    // PUT /api/bookmark/{id}/visit
    [HttpPut("{id:int}/visit")]
    public async Task<IActionResult> RecordVisit(int id)
    {
        var existing = await _bookmarkService.GetBookmarkByIdAsync(id);
        if (existing == null) return NotFound();

        var forbidden = EnforceOwnership(existing.UserId);
        if (forbidden != null) return forbidden;

        await _bookmarkService.RecordVisitAsync(id);
        return NoContent();
    }
}
