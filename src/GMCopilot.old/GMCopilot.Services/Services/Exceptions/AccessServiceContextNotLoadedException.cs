using System.Runtime.Serialization;

namespace GMCopilot.Services.Exceptions;

/// <summary>
/// Represents an exception that is thrown when the access service context is not loaded.
/// </summary>
[Serializable]
public class AccessServiceContextNotLoadedException : Exception
{
    public AccessServiceContextNotLoadedException()
    {
    }

    public AccessServiceContextNotLoadedException(string? message) : base(message)
    {
    }

    public AccessServiceContextNotLoadedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected AccessServiceContextNotLoadedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}