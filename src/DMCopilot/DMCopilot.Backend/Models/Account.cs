namespace DMCopilot.Backend.Models
{
    /// <summary>
    /// Represents an account in the system.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the email address associated with the account.
        /// This is used as the unique identifier and partition key for the account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name of the account.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the image associated with the account.
        /// </summary>
        public byte[]? Image { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        /// <param name="name">The name of the account.</param>
        /// <param name="email">The email address associated with the account.</param>
        /// <param name="image">The image associated with the account.</param>
        public Account(string email, string ? name = null, byte[]? image = null)
        {
            Email = email;
            Name = name;
            Image = image;
        }
    }
}
