using System.Runtime.Serialization;

namespace DMCopilot.Backend.Data
{
    [Serializable]
    internal class TenantNotFoundException : Exception
    {
        public TenantNotFoundException()
        {
        }

        public TenantNotFoundException(string? message) : base(message)
        {
        }

        public TenantNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TenantNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}