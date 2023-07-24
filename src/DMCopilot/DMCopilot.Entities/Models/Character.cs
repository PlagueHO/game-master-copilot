using DMCopilot.Entities.Types;
using System.Text.Json.Serialization;

namespace DMCopilot.Entities.Models;
/// <summary>
/// Class to represent a character in the game.
/// </summary>
public class Character : IGeneratedStorageEntity
{
    /// <summary>
    /// The unique identifier for the character.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The name of the character.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The age of the character.
    /// </summary>
    [JsonPropertyName("age")]
    public int? Age { get; set; }

    /// <summary>
    /// The class of the character.
    /// </summary>
    [JsonPropertyName("class")]
    public string? Class { get; set; }

    /// <summary>
    /// The race of the character.
    /// </summary>
    [JsonPropertyName("race")]
    public string? Race { get; set; }

    /// <summary>
    /// The height of the character.
    /// </summary>
    [JsonPropertyName("height")]
    public Height? Height { get; set; }

    /// <summary>
    /// The weight of the character.
    /// </summary>
    [JsonPropertyName("weight")]
    public Weight? Weight { get; set; }

    /// <summary>
    /// The physical characteristics of the character.
    /// </summary>
    [JsonPropertyName("physicalcharacteristics")]
    public string? PhysicalCharacteristics { get; set; }

    /// <summary>
    /// The voice of the character.
    /// </summary>
    [JsonPropertyName("voice")]
    public string? Voice { get; set; }

    /// <summary>
    /// The clothing of the character.
    /// </summary>
    [JsonPropertyName("clothing")]
    public string? Clothing { get; set; }

    /// <summary>
    /// The personality traits of the character.
    /// </summary>
    [JsonPropertyName("personalitytraits")]
    public string? PersonalityTraits { get; set; }

    /// <summary>
    /// The ideals of the character.
    /// </summary>
    [JsonPropertyName("ideals")]
    public string? Ideals { get; set; }

    /// <summary>
    /// The bonds of the character.
    /// </summary>
    [JsonPropertyName("bonds")]
    public string? Bonds { get; set; }

    /// <summary>
    /// The flaws of the character.
    /// </summary>
    [JsonPropertyName("flaws")]
    public string? Flaws { get; set; }

    /// <summary>
    /// Information about how the storage entity was populated
    /// </summary>
    [JsonPropertyName("generatedcontent")]
    public GeneratedContent GeneratedContent { get; set; } = new GeneratedContent();

    /// <summary>
    /// Initializes a new instance of the <see cref="Character"/> class.
    /// </summary>
    public Character(string id)
    {
        Id = id;
    }
}
