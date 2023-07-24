using System.Runtime.Serialization;

namespace DMCopilot.Data.Repositories
{
    [Serializable]
    public class WorldNotFoundException : Exception
    {
        public WorldNotFoundException()
        {
        }

        public WorldNotFoundException(string? message) : base(message)
        {
        }

        public WorldNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected WorldNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}