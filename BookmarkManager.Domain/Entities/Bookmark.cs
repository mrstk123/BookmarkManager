using System;

namespace BookmarkManager.Domain.Entities;

public class Bookmark
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? FolderId { get; set; }
    public required string Title { get; set; }
    public required string Url { get; set; }
    public bool IsFavorite { get; set; }
    public string? IconUrl { get; set; }
    public string? ImportHash { get; set; }
    public DateTime? LastVisitedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    protected User User { get; set; } = null!;
    protected Folder? Folder { get; set; }
    protected ICollection<BookmarkTag> BookmarkTags { get; set; } = new List<BookmarkTag>();

}
