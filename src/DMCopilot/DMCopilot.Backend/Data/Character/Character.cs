namespace DMCopilot.Data
{
    /// <summary>
    /// Class to represent a character in the game.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// The unique identifier for the character.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// The name of the character.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The age of the character.
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// The class of the character.
        /// </summary>
        public string? Class { get; set; }

        /// <summary>
        /// The race of the character.
        /// </summary>
        public string? Race { get; set; }

        /// <summary>
        /// The height of the character.
        /// </summary>
        public Height? Height { get; set; }

        /// <summary>
        /// The weight of the character.
        /// </summary>
        public Weight? Weight { get; set; }

        /// <summary>
        /// The physical characteristics of the character.
        /// </summary>
        public string? PhysicalCharacteristics { get; set; }

        /// <summary>
        /// The voice of the character.
        /// </summary>
        public string? Voice { get; set; }

        /// <summary>
        /// The clothing of the character.
        /// </summary>
        public string? Clothing { get; set; }

        /// <summary>
        /// The personality traits of the character.
        /// </summary>
        public string? PersonalityTraits { get; set; }

        /// <summary>
        /// The ideals of the character.
        /// </summary>
        public string? Ideals { get; set; }

        /// <summary>
        /// The bonds of the character.
        /// </summary>
        public string? Bonds { get; set; }

        /// <summary>
        /// The flaws of the character.
        /// </summary>
        public string? Flaws { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// </summary>
        public Character()
        {
        }
    }
}
