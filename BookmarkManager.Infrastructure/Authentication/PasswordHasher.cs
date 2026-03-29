using BookmarkManager.Application.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace BookmarkManager.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BC.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BC.Verify(password, hash);
    }
}
