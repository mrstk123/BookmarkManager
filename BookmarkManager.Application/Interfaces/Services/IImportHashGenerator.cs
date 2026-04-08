namespace BookmarkManager.Application.Interfaces.Services;

/// <summary>
/// Contract for generating import hashes used to detect duplicate bookmarks based on URL.
/// </summary>
public interface IImportHashGenerator
{
    /// <summary>
    /// Generates a hash of the given URL for duplicate-import detection.
    /// </summary>
    /// <param name="url">The URL to hash.</param>
    /// <returns>A hex-encoded hash string.</returns>
    string Generate(string url);
}
