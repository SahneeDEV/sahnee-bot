using SahneeBotController;

namespace SahneeBot;

public static class SahneeBotDiscordError
{
    public static ISuccess<T> GetMissingRolePermissionsError<T>(string prefix)
    {
        return new Error<T>("The Sahnee-Bot does not have the required permissions to edit the warning roles on your " +
                            "server. Please drag the Sahnee-Bot role above all other roles starting with " +
                            $"\"{prefix}\" in your Server Settings and make sure that it has the \"Manage Roles\" " +
                            "permission.");
    }

    /*public static ISuccess<T> GetMissingBotPermissionError<T>(string permission)
    {
        return new Error<T>("The Sahnee-Bot does not have the required permissions to " + permission + " on your " +
                            "server. Please edit the permissions of the Sahnee-Bot in your Server Settings and try " +
                            "again.");
    }*/
}