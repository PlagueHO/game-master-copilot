using System.Runtime.Serialization;

namespace GMCopilot.Services.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a user is not authenticated.
/// </summary>
[Serializable]
public class AccessServiceNotAuthenticatedException : Exception
{
    public AccessServiceNotAuthenticatedException()
    {
    }

    public AccessServiceNotAuthenticatedException(string? message) : base(message)
    {
    }

    public AccessServiceNotAuthenticatedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected AccessServiceNotAuthenticatedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
