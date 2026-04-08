using System.Security.Cryptography;
using System.Text;
using BookmarkManager.Application.Interfaces.Services;

namespace BookmarkManager.Infrastructure.Services;

/// <summary>
/// Implementation of IImportHashGenerator using SHA256.
/// </summary>
public class ImportHashGenerator : IImportHashGenerator
{
    public string Generate(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(url.Trim().ToLowerInvariant());
        var hashBytes = sha256.ComputeHash(bytes);

        var hashBuilder = new StringBuilder();
        foreach (var b in hashBytes)
        {
            hashBuilder.Append(b.ToString("x2"));
        }

        return hashBuilder.ToString();
    }
}