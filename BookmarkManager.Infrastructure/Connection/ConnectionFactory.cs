using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace BookmarkManager.Infrastructure.Connection;

public class ConnectionFactory : IConnectionFactory
{
    private readonly IConfiguration _configuration;

    public ConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        var provider = _configuration["Provider"];
        if (provider != null && provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
        {
            var conn = _configuration.GetConnectionString("PostgresConnection");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("Postgres connection string is missing in configuration.");
            return new NpgsqlConnection(conn);
        }
        else
        {
            var conn = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("SQL Server connection string is missing in configuration.");
            return new SqlConnection(conn);
        }
    }
}
