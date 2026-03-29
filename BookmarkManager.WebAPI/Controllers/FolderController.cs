using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkManager.WebAPI.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class FolderController : ControllerBase
{
    private readonly IFolderService _folderService;

    public FolderController(IFolderService folderService)
    {
        _folderService = folderService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFolder(int id)
    {
        var folder = await _folderService.GetFolderByIdAsync(id);
        if (folder == null) return NotFound();
        return Ok(folder);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetFolders(int userId)
    {
        var folders = await _folderService.GetFoldersByUserIdAsync(userId);
        return Ok(folders);
    }

    [HttpPost]
    public async Task<IActionResult> AddFolder([FromBody] CreateFolderRequest request)
    {
        var folder = await _folderService.AddAsync(request);
        return CreatedAtAction(nameof(GetFolder), new { id = folder.Id }, folder);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFolder(int id, [FromBody] UpdateFolderRequest request)
    {
        await _folderService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFolder(int id)
    {
        await _folderService.DeleteAsync(id);
        return NoContent();
    }
}
