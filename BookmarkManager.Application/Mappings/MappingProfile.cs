using AutoMapper;
using BookmarkManager.Application.DTOs;
using BookmarkManager.Domain.Entities;

namespace BookmarkManager.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User Mappings
        CreateMap<User, UserDto>();


        // Folder Mappings
        CreateMap<Folder, FolderDto>();


        // Bookmark Mappings
        CreateMap<Bookmark, BookmarkDto>();

    }
}