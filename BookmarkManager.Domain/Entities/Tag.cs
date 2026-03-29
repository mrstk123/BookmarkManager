using System;

namespace BookmarkManager.Domain.Entities;

public class Tag
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<BookmarkTag> BookmarkTags { get; set; } = new List<BookmarkTag>();
}
