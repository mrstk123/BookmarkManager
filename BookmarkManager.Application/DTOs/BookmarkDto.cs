namespace BookmarkManager.Application.DTOs;

public class BookmarkDto
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

    public string? FolderName { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}
