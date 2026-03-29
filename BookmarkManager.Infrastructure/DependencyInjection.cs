using BookmarkManager.Application.Interfaces;
using BookmarkManager.Infrastructure.Commands;
using BookmarkManager.Infrastructure.Persistence;
using BookmarkManager.Infrastructure.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookmarkManager.Infrastructure.Authentication;
using BookmarkManager.Infrastructure.Connection;
using System.Data;

namespace BookmarkManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Migrations
        DatabaseSetup.RunMigrations(configuration);
        
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Register a connection factory that chooses the right provider at runtime
        services.AddSingleton<IConnectionFactory, ConnectionFactory>();

        // Queries
        services.AddScoped<IUserQueries, UserQueries>();
        services.AddScoped<IFolderQueries, FolderQueries>();
        services.AddScoped<IBookmarkQueries, BookmarkQueries>();
        services.AddScoped<ITagQueries, TagQueries>();

        // Commands
        services.AddScoped<IUserCommands, UserCommands>();
        services.AddScoped<IFolderCommands, FolderCommands>();
        services.AddScoped<IBookmarkCommands, BookmarkCommands>();
        services.AddScoped<ITagCommands, TagCommands>();

        return services;
    }
}
