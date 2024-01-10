namespace GMCopilot.Core.Authorization;

public class AuthorizationScopes
{
    public const string UserRead = "User.Read";
    // TODO: Change this to get from a config file
    public const string GMCopilotRead = "api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.Read";
    public const string GMCopilotReadWrite = "api://b1a02918-c873-4c7f-ba43-248171c138fe/GMCopilot.ReadWrite";
}
