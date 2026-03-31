using System;

namespace BookmarkManager.Domain.Entities;

public class Folder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties - Unused with Dapper
    // public User User { get; set; } = null!;
    // public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
}
