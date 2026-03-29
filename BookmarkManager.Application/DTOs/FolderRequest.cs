namespace BookmarkManager.Application.DTOs;

public class CreateFolderRequest
{
    public int UserId { get; set; }
    public required string Name { get; set; }
}

public class UpdateFolderRequest
{
    public required string Name { get; set; }
}
