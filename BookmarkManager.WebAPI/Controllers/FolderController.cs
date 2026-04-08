using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkManager.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
public class FolderController : ApiControllerBase
{
    private readonly IFolderService _folderService;

    public FolderController(IFolderService folderService)
    {
        _folderService = folderService;
    }

    // GET /api/folder/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetFolder(int id)
    {
        var folder = await _folderService.GetFolderByIdAsync(id);
        if (folder == null) return NotFound();

        var forbidden = EnforceOwnership(folder.UserId);
        if (forbidden != null) return forbidden;

        return Ok(folder);
    }

    // GET /api/folder
    [HttpGet]
    public async Task<IActionResult> GetFolders()
    {
        var userId = GetCurrentUserId();
        // EnforceOwnership is needed only when userId comes from the client (e.g. path/query param).
        // var forbidden = EnforceOwnership(userId);
        // if (forbidden != null) return forbidden;

        return Ok(await _folderService.GetFoldersByUserIdAsync(userId));
    }

    // POST /api/folder
    [HttpPost]
    public async Task<IActionResult> AddFolder([FromBody] CreateFolderRequest request)
    {
        // Override UserId from JWT to prevent ownership forgery
        request.UserId = GetCurrentUserId();

        var folder = await _folderService.AddAsync(request);
        return CreatedAtAction(nameof(GetFolder), new { id = folder.Id }, folder);
    }

    // PUT /api/folder/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateFolder(int id, [FromBody] UpdateFolderRequest request)
    {
        var existing = await _folderService.GetFolderByIdAsync(id);
        if (existing == null) return NotFound();

        var forbidden = EnforceOwnership(existing.UserId);
        if (forbidden != null) return forbidden;

        await _folderService.UpdateAsync(id, request);
        return NoContent();
    }

    // DELETE /api/folder/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteFolder(int id)
    {
        var existing = await _folderService.GetFolderByIdAsync(id);
        if (existing == null) return NotFound();

        var forbidden = EnforceOwnership(existing.UserId);
        if (forbidden != null) return forbidden;

        await _folderService.DeleteAsync(id);
        return NoContent();
    }
}
