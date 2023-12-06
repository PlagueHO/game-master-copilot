using System.Runtime.Serialization;

namespace GMCopilot.Services.Exceptions;

/// <summary>
/// Exception thrown when there is a failure in creating a tenant.
/// </summary>
[Serializable]
public class AccessServiceTenantCreateException : Exception
{
    public AccessServiceTenantCreateException()
    {
    }

    public AccessServiceTenantCreateException(string message) : base(message)
    {
    }

    public AccessServiceTenantCreateException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected AccessServiceTenantCreateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
