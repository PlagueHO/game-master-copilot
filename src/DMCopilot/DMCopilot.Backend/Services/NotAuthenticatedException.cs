using System.Runtime.Serialization;

namespace DMCopilot.Backend.Services
{
    [Serializable]
    internal class NotAuthenticatedException : Exception
    {
        public NotAuthenticatedException()
        {
        }

        public NotAuthenticatedException(string? message) : base(message)
        {
        }

        public NotAuthenticatedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NotAuthenticatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
