namespace BookmarkManager.Application.DTOs;

public class UpdateProfileRequest
{
    public required string FullName { get; set; }
}

public class ChangePasswordRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
