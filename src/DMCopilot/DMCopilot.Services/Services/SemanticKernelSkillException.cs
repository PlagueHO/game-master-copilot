using System.Runtime.Serialization;

namespace DMCopilot.Services;

[Serializable]
internal class SemanticKernelSkillException : Exception
{
    public SemanticKernelSkillException()
    {
    }

    public SemanticKernelSkillException(string? message) : base(message)
    {
    }

    public SemanticKernelSkillException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected SemanticKernelSkillException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}