using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

using BookmarkManager.Infrastructure;
using BookmarkManager.Infrastructure.Connection;
using Npgsql;
using Microsoft.Data.SqlClient;

namespace BookmarkManager.Infrastructure.Tests;

public class ConnectionFactoryTests
{
    [Fact]
    public void CreateConnection_Returns_NpgsqlConnection_When_PostgresProvider()
    {
        // Arrange
        var inMemory = new Dictionary<string, string>
        {
            ["Provider"] = "PostgreSQL",
            ["ConnectionStrings:PostgresConnection"] = "Host=localhost;Database=TestDb;Username=user;Password=pass;"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();
        var factory = new ConnectionFactory(configuration);

        // Act
        var conn = factory.CreateConnection();

        // Assert
        Assert.IsType<NpgsqlConnection>(conn);
    }

    [Fact]
    public void CreateConnection_Returns_SqlConnection_When_DefaultProvider()
    {
        // Arrange
        var inMemory = new Dictionary<string, string>
        {
            ["Provider"] = "SqlServer",
            ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=TestDb;User Id=sa;Password=pass;"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();
        var factory = new ConnectionFactory(configuration);

        // Act
        var conn = factory.CreateConnection();

        // Assert
        Assert.IsType<SqlConnection>(conn);
    }
}
