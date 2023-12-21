using System.Text.Json;
using System.Text.Json.Serialization;
using GMCopilot.Core.Types;

namespace GMCopilot.Core.Models
{
    /// <summary>
    /// Represents a universe page entity.
    /// </summary>
    public class Universe : IStorageTenantedEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the universe.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the universe.
        /// </summary>
        [JsonPropertyName("tenantid")]
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the name of the universe.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the universe.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the creation of the universe.
        /// </summary>
        [JsonPropertyName("creation")]
        public string? Creation { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of other properties for the universe.
        /// </summary>
        [JsonPropertyName("properties")]
        public Dictionary<string, string>? Properties { get; set; }

        /// <summary>
        /// Information about how the storage entity was populated
        /// </summary>
        [JsonPropertyName("generatedcontent")]
        public PageGeneratedContent GeneratedContent { get; set; } = new PageGeneratedContent();

        /// <summary>
        /// Initializes a new instance of the <see cref="Universe"/> class.
        /// </summary>
        public Universe(string id, string tenantId, string name, string? description = null)
        {
            Id = id;
            TenantId = tenantId;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Converts the current <see cref="Universe"/> object to a JSON string.
        /// </summary>
        /// <returns>A JSON formatted <see cref="string"/>.</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Deserializes the provided JSON string into a new <see cref="Universe"/> object.
        /// </summary>
        /// <param name="json">The JSON <see cref="string"/> to deserialize.</param>
        /// <returns>A new <see cref="Universe"/> object based on the provided JSON string.</returns>
        public static Universe FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            try
            {
                var universe = JsonSerializer.Deserialize<Universe>(json);
                if (universe == null)
                {
                    throw new ArgumentException("Unable to deserialize the provided JSON string.", nameof(json));
                }
                return universe;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to deserialize the provided JSON string.", nameof(json), ex);
            }
        }
    }
}