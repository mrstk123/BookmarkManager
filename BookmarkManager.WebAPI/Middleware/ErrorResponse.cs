using System.Text.Json.Serialization;

namespace BookmarkManager.WebAPI.Middleware;

/// <summary>
/// Standard error response format returned by the API when an error occurs.
/// </summary>
public class ErrorResponse
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("details")]
    public string? Details { get; set; }

    [JsonPropertyName("errors")]
    public List<string>? Errors { get; set; }
}