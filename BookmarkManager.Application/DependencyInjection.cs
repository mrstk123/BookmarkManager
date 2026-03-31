using Microsoft.Extensions.DependencyInjection;
using BookmarkManager.Application.Interfaces;
using BookmarkManager.Application.Services;

namespace BookmarkManager.Application;

public static class DependencyInjection  // was: DepencyInjection (typo fixed)
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBookmarkService, BookmarkService>();
        services.AddScoped<IFolderService, FolderService>();
        services.AddScoped<ITagService, TagService>();

        return services;
    }
}
