using System.Text.Json.Serialization;

namespace GMCopilot.Core.Models;

/// <summary>
/// The type of method used to generate the content of the page.
/// </summary>
public enum PageContentGenerationMethod
{
    None,
    Manual,
    AI,
    Random
}

/// <summary>
/// Class to represent how the page content was generated.
/// </summary>
public class PageGeneratedContent
{
    /// <summary>
    /// The method that was used to generate this content.
    /// </summary>
    [JsonPropertyName("method")]
    public PageContentGenerationMethod Method { get; set; } = PageContentGenerationMethod.Manual;

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

    public PageGeneratedContent(PageContentGenerationMethod method, string? plugin, string? function, Dictionary<string, string>? parameters)
    {
        Method = method;
        Plugin = plugin;
        Function = function;
        Parameters = parameters;
    }

    public PageGeneratedContent()
    {
    }
}