using System;

namespace BookmarkManager.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}
