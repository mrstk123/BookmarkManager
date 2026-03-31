namespace BookmarkManager.Application.Exceptions;

/// <summary>
/// Represents a domain rule violation — maps to HTTP 400 Bad Request in the exception middleware.
/// Use this instead of the generic InvalidOperationException so the middleware can
/// distinguish intentional business errors from unexpected runtime failures.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
