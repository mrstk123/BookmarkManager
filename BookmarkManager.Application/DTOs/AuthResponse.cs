namespace BookmarkManager.Application.DTOs;

public class AuthResponseDto
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
}
