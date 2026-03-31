namespace BookmarkManager.Domain.Interfaces;

/// <summary>
/// Domain service for generating import hashes for bookmarks.
/// Import hashes are used to detect duplicate bookmarks based on URL.
/// </summary>
public interface IImportHashGenerator
{
    /// <summary>
    /// Generates a SHA256 hash of the given URL for import comparison.
    /// </summary>
    /// <param name="url">The URL to hash.</param>
    /// <returns>A hex-encoded SHA256 hash string.</returns>
    string Generate(string url);
}