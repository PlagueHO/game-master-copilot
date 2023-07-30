using System.Text.Json.Serialization;

namespace DMCopilot.Entities.Models;

public enum GeneratedContentMethod
{
    None,
    Manual,
    AI,
    Random
}

/// <summary>
/// Class to represent how the source content was generated.
/// </summary>
public class GeneratedContent
{
    /// <summary>
    /// The method that was used to generate this content.
    /// </summary>
    [JsonPropertyName("method")]
    public GeneratedContentMethod Method { get; set; } = GeneratedContentMethod.Manual;

    /// <summary>
    /// The Semantic Kernel plugin that was used to generate this content if the method was FoundationalModel.
    /// </summary>
    [JsonPropertyName("plugin")]
    public string? Plugin { get; set; }

    /// <summary>
    /// The Semantic Kernel function in the plugin that was used to generate this content if the method was FoundationalModel.
    /// </summary>
    [JsonPropertyName("function")]
    public string? Function { get; set; }

    /// <summary>
    /// The input parameters that was passed to the Semantic Kernel function that was used to generate this content if the method was FoundationalModel.
    /// </summary>
    [JsonPropertyName("parameters")]
    public Dictionary<string, string>? Parameters { get; set; }

    public GeneratedContent(GeneratedContentMethod method, string? plugin, string? function, Dictionary<string, string>? parameters)
    {
        Method = method;
        Plugin = plugin;
        Function = function;
        Parameters = parameters;
    }

    public GeneratedContent()
    {
    }
}