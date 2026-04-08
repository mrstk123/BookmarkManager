namespace BookmarkManager.Application.Exceptions;

/// <summary>
/// Represents an intentional business rule violation — maps to HTTP 400 Bad Request
/// in the exception middleware. Use this instead of "InvalidOperationException"
/// so the middleware can distinguish intentional business errors from unexpected runtime failures.
/// </summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}
