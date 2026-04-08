using System.Data;

namespace BookmarkManager.Infrastructure.Connection;

public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}
