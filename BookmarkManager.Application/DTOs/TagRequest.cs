namespace BookmarkManager.Application.DTOs;

public class CreateTagRequest
{
    public int UserId { get; set; }
    public required string Name { get; set; }
}

public class UpdateTagRequest
{
    public required string Name { get; set; }
}
