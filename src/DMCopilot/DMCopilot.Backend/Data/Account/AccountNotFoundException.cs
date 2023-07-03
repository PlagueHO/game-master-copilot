using System.Runtime.Serialization;

namespace DMCopilot.Backend.Data
{
    [Serializable]
    internal class AccountNotFoundException : Exception
    {
        public AccountNotFoundException()
        {
        }

        public AccountNotFoundException(string? message) : base(message)
        {
        }

        public AccountNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AccountNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}