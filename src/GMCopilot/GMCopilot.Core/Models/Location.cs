using System.Text.Json;
using System.Text.Json.Serialization;
using GMCopilot.Core.Types;
using System.ComponentModel.DataAnnotations;

namespace GMCopilot.Core.Models
{
    /// <summary>
    /// Representsa llencount within a world where an encounter can take placeke place.
    /// </summary>
    public class Location : IPageStorageEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the location.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <inheritdoc/>
        [JsonPropertyName("type")]
        public PageStorageEntityTypes Type { get; set; } = PageStorageEntityTypes.Location;

        /// <summary>
        /// Gets or sets the unique identifier for the world.
        /// </summary>
        [JsonPropertyName("tenantid")]
        public Guid TenantId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for
        /// </summary>
        [JsonPropertyName("worldid")]
        public Guid WorldId { get; set; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the location.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the address of the location.
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address of the location.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of other properties for the location.
        /// </summary>
        [JsonPropertyName("properties")]
        public Dictionary<string, string>? Properties { get; set; }
        /// <summary>
        /// Information about how the storage entity was populated.
        /// </summary>
        [JsonPropertyName("generatedcontent")]
        public PageGeneratedContent GeneratedContent { get; set; } = new PageGeneratedContent();

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        public Location(Guid id, Guid worldId, string name, string? description = null)
        {
            Id = id;
            WorldId = worldId;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Converts the current <see cref="Location"/> object to a JSON string.
        /// </summary>
        /// <returns>A JSON formatted <see cref="string"/>.</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Deserializes the provided JSON string into a new <see cref="Location"/> object.
        /// </summary>
        /// <param name="json">The JSON <see cref="string"/> to deserialize.</param>
        /// <returns>A new <see cref="Location"/> object based on the provided JSON string.</returns>
        public static Location FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            try
            {
                var location = JsonSerializer.Deserialize<Location>(json);
                if (location == null)
                {
                    throw new ArgumentException("Unable to deserialize the provided JSON string.", nameof(json));
                }
                return location;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to deserialize the provided JSON string.", nameof(json), ex);
            }
        }
    }
}