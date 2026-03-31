using System;

namespace BookmarkManager.Domain.Entities;

public class BookmarkTag
{
    public int BookmarkId { get; set; }
    public int TagId { get; set; }
    
    // Navigation properties - Unused with Dapper
    // public Bookmark Bookmark { get; set; } = null!;
    // public Tag Tag { get; set; } = null!;
}