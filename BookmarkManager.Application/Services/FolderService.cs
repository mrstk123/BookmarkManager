using BookmarkManager.Application.DTOs;
using BookmarkManager.Application.Interfaces;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Services;

public class FolderService : IFolderService
{
    private readonly IFolderQueries _folderQueries;
    private readonly IFolderCommands _folderCommands;

    public FolderService(IFolderQueries folderQueries, IFolderCommands folderCommands)
    {
        _folderQueries = folderQueries;
        _folderCommands = folderCommands;
    }

    public async Task<FolderDto?> GetFolderByIdAsync(int id)
    {
        return await _folderQueries.GetByIdAsync(id);
    }

    public async Task<IEnumerable<FolderDto>> GetFoldersByUserIdAsync(int userId)
    {
        return await _folderQueries.GetByUserIdAsync(userId);
    }

    public async Task<FolderDto> AddAsync(CreateFolderRequest request)
    {
        var folder = new Folder
        {
            UserId = request.UserId,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var id = await _folderCommands.AddAsync(folder);

        return new FolderDto
        {
            Id = id,
            UserId = request.UserId,
            Name = request.Name
        };
    }

    public async Task UpdateAsync(int id, UpdateFolderRequest request)
    {
        var folder = new Folder
        {
            Id = id,
            Name = request.Name,
            UpdatedAt = DateTime.UtcNow
        };

        await _folderCommands.UpdateAsync(folder);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _folderCommands.DeleteAsync(id);
    }
}
