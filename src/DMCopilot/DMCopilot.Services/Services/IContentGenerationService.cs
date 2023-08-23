using DMCopilot.Entities.Models;

namespace DMCopilot.Services;

/// <summary>
/// This is an interface used to abstract the content generation service.
/// It is intended that there will be different implementations for different content generation services.
/// The following implementations are planned:
/// 1. Semantic Kernel within the Blazor Server as a service.
/// 2. Semantic Kernel within the Blazor Server as an API controller.
/// 3. Semantic Kernel as a standalone API (probably an Azure Function in Azure Container Apps).
/// 4. LangChain as a standalone API (probably an Azure Function in Azure Container Apps).
/// </summary>
public interface IContentGenerationService
{
    /// <summary>
    /// Generate a World from the provided content.
    /// </summary>
    /// <param name="details"></param>
    /// <returns></returns>
    public Task<World> GenerateWorldAsync(string details);
}