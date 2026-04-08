using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkManager.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
public class TagController : ApiControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    // GET /api/tag/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTag(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null) return NotFound();

        var forbidden = EnforceOwnership(tag.UserId);
        if (forbidden != null) return forbidden;

        return Ok(tag);
    }

    // GET /api/tag
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var userId = GetCurrentUserId();
        // EnforceOwnership is needed only when userId comes from the client (e.g. path/query param).
        // var forbidden = EnforceOwnership(userId);
        // if (forbidden != null) return forbidden;

        return Ok(await _tagService.GetTagsByUserIdAsync(userId));
    }

    // POST /api/tag
    [HttpPost]
    public async Task<IActionResult> AddTag([FromBody] CreateTagRequest request)
    {
        // Override UserId from JWT to prevent ownership forgery
        request.UserId = GetCurrentUserId();

        var tag = await _tagService.AddAsync(request);
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
    }

    // PUT /api/tag/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagRequest request)
    {
        var existing = await _tagService.GetTagByIdAsync(id);
        if (existing == null) return NotFound();

        var forbidden = EnforceOwnership(existing.UserId);
        if (forbidden != null) return forbidden;

        await _tagService.UpdateAsync(id, request);
        return NoContent();
    }

    // DELETE /api/tag/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var existing = await _tagService.GetTagByIdAsync(id);
        if (existing == null) return NotFound();

        var forbidden = EnforceOwnership(existing.UserId);
        if (forbidden != null) return forbidden;

        await _tagService.DeleteAsync(id);
        return NoContent();
    }
}
