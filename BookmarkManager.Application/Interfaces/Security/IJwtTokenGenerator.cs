using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
