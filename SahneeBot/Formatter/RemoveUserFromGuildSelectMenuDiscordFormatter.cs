using Discord;

namespace SahneeBot.Formatter;

public class RemoveUserFromGuildSelectMenuDiscordFormatter : IDiscordFormatter<RemoveUserFromGuildSelectMenuDiscordFormatter.Args>
{
    public record struct Args(IEnumerable<IUser> Users);
    
    public Task<DiscordFormat> Format(Args arg)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select one or more Users that will be removed")
            .WithCustomId("remove-guild-users-from-db")
            .WithMinValues(1)
            .WithMaxValues(arg.Users.ToList().Count);
        //add all users
        foreach (var currentUser in arg.Users)
        {
            menuBuilder.AddOption(currentUser.Username + "#" + currentUser.Discriminator
                , currentUser.Id.ToString());
        }

        var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);

        return Task.FromResult(new DiscordFormat
        {
            Text = "Please select all users you want to have removed"
            , Components = builder
        });
    }
}