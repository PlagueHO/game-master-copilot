using Newtonsoft.Json;

namespace DMCopilot.Data.Models;

public interface IGeneratedStorageEntity : IStorageEntity
{
    /// <summary>
    /// Information about how the storage entity was populated
    /// </summary>
    [JsonProperty(PropertyName = "generatedcontent")]
    public GeneratedContent GeneratedContent { get; set; }
}
