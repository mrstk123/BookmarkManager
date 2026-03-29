using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkManager.WebAPI.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookmarkController : ControllerBase
{
    private readonly IBookmarkService _bookmarkService;

    public BookmarkController(IBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookmark(int id)
    {
        var bookmark = await _bookmarkService.GetBookmarkByIdAsync(id);
        if (bookmark == null) return NotFound();
        return Ok(bookmark);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBookmarks(int userId, [FromQuery] string? folderName, [FromQuery] string? tagName)
    {
        if (!string.IsNullOrWhiteSpace(tagName))
        {
            var tagged = await _bookmarkService.GetByTagAsync(userId, tagName);
            return Ok(tagged);
        }
        if (!string.IsNullOrWhiteSpace(folderName))
        {
            var folderBookmarks = await _bookmarkService.GetByFolderAsync(userId, folderName);
            return Ok(folderBookmarks);
        }
        var bookmarks = await _bookmarkService.GetBookmarksByUserIdAsync(userId);
        return Ok(bookmarks);
    }

    [HttpGet("favorites/{userId}")]
    public async Task<IActionResult> GetFavorites(int userId)
    {
        var bookmarks = await _bookmarkService.GetFavoritesByUserIdAsync(userId);
        return Ok(bookmarks);
    }

    [HttpPost]
    public async Task<IActionResult> AddBookmark([FromBody] CreateBookmarkRequest request)
    {
        var bookmark = await _bookmarkService.AddAsync(request);
        return CreatedAtAction(nameof(GetBookmark), new { id = bookmark.Id }, bookmark);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBookmark(int id, [FromBody] UpdateBookmarkRequest request)
    {
        var bookmark = await _bookmarkService.UpdateAsync(id, request);
        return Ok(bookmark);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBookmark(int id)
    {
        await _bookmarkService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("toggle-favorite")]
    public async Task<IActionResult> ToggleFavorite(int id, int userId)
    {
        await _bookmarkService.ToggleFavoriteAsync(id, userId);
        return NoContent();
    }
}
