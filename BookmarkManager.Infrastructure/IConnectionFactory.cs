using System.Data;

namespace BookmarkManager.Infrastructure;

public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}
