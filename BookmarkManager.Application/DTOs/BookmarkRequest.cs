namespace BookmarkManager.Application.DTOs;

public class CreateBookmarkRequest
{
    public int UserId { get; set; }
    public required string Title { get; set; }
    public required string Url { get; set; }
    public string? FolderName { get; set; }
    public bool IsFavorite { get; set; }
    public string? IconUrl { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class UpdateBookmarkRequest
{
    public required string Title { get; set; }
    public required string Url { get; set; }
    public string? FolderName { get; set; }
    public bool IsFavorite { get; set; }
    public string? IconUrl { get; set; }
    public List<string> Tags { get; set; } = new();
}
