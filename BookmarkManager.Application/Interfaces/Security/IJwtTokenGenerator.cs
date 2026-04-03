using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
