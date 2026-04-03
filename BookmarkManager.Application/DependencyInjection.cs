using Microsoft.Extensions.DependencyInjection;
using BookmarkManager.Application.Interfaces.Services;
using BookmarkManager.Application.Services;

namespace BookmarkManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBookmarkService, BookmarkService>();
        services.AddScoped<IFolderService, FolderService>();
        services.AddScoped<ITagService, TagService>();

        return services;
    }
}
