using System.Security.Claims;

namespace GMCopilot.Client.Models
{
    /// <summary>
    /// Represents information about the currently logged in user.
    /// </summary>
    public sealed class UserInfo
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public required string UserId { get; init; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// The claim type for the user ID.
        /// </summary>
        public const string UserIdClaimType = "preferred_username";

        /// <summary>
        /// The claim type for the user name.
        /// </summary>
        public const string NameClaimType = "name";

        /// <summary>
        /// Creates a new <see cref="UserInfo"/> object from the provided <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> containing the claims.</param>
        /// <returns>A new <see cref="UserInfo"/> object.</returns>
        public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal) =>
            new()
            {
                UserId = GetRequiredClaim(principal, UserIdClaimType),
                Name = GetRequiredClaim(principal, NameClaimType),
            };

        /// <summary>
        /// Converts the <see cref="UserInfo"/> object to a <see cref="ClaimsPrincipal"/> object.
        /// </summary>
        /// <returns>A new <see cref="ClaimsPrincipal"/> object.</returns>
        public ClaimsPrincipal ToClaimsPrincipal() =>
            new(new ClaimsIdentity(
                [new(UserIdClaimType, UserId), new(NameClaimType, Name)],
                authenticationType: nameof(UserInfo),
                nameType: NameClaimType,
                roleType: null));

        /// <summary>
        /// Retrieves the value of the specified claim from the provided <see cref="ClaimsPrincipal"/> object.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> object containing the claims.</param>
        /// <param name="claimType">The type of the claim to retrieve.</param>
        /// <returns>The value of the claim, or null if the claim is not found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the required claim is not found.</exception>
        private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType) =>
            principal.FindFirst(claimType)?.Value ?? throw new InvalidOperationException($"Could not find required '{claimType}' claim.");
    }
}
