using DbUp;
using Microsoft.Extensions.Configuration;

namespace BookmarkManager.Infrastructure.Persistence;

public static class DatabaseSetup
{
    public static void RunMigrations(IConfiguration configuration)
    {
        var provider = configuration["Provider"];

        if (provider?.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase) == true)
        {
            string connectionString = configuration.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("Postgres connection string is missing in configuration.");
            // 1. Ensure the actual database exists (Creates it if missing)
            EnsureDatabase.For.PostgresqlDatabase(connectionString);

            // 2. Configure the migrator to look for embedded .sql files in this assembly
            var migrator = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(DatabaseSetup).Assembly, s => s.Contains(".PostgreSql."))
                .LogToConsole()
                .Build();

            // 3. Execute migrations
            var result = migrator.PerformUpgrade();
            if (!result.Successful)
            {
                // Stop the application from starting if the database is in a bad state
                throw new InvalidOperationException("Database migration failed.", result.Error);
            }
        }
        else
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("SQL Server connection string is missing in configuration.");

            // 1. Ensure the actual database exists (Creates it if missing)
            EnsureDatabase.For.SqlDatabase(connectionString);

            // 2. Configure the migrator to look for embedded .sql files in this assembly
            var migrator = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(DatabaseSetup).Assembly, s => s.Contains(".SqlServer."))
                .LogToConsole()
                .Build();

            // 3. Execute migrations
            var result = migrator.PerformUpgrade();
            if (!result.Successful)
            {
                // Stop the application from starting if the database is in a bad state
                throw new InvalidOperationException("Database migration failed.", result.Error);
            }
        }

    }
}