using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookmarkManager.WebAPI.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTag(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTags(int userId)
    {
        var tags = await _tagService.GetTagsByUserIdAsync(userId);
        return Ok(tags);
    }

    [HttpPost]
    public async Task<IActionResult> AddTag([FromBody] CreateTagRequest request)
    {
        var tag = await _tagService.AddAsync(request);
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagRequest request)
    {
        await _tagService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        await _tagService.DeleteAsync(id);
        return NoContent();
    }
}
