using System;

namespace BookmarkManager.Application.DTOs;

public class FolderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Name { get; set; }
}
