using Discord;

namespace SahneeBot.InteractionComponents.SelectMenu;

public class RemoveUserFromGuildSelectMenu : ISelectMenu<RemoveUserFromGuildSelectMenu.Args>
{

    public record struct Args(List<IUser> Users);

    public Task<SelectMenuBuilder> SelectMenu(Args arg)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select one or more Users that will be removed")
            .WithCustomId("remove-guild-users-from-db")
            .WithMinValues(1)
            .WithMaxValues(arg.Users.Count);
        //add all users
        foreach (var currentUser in arg.Users)
        {
            menuBuilder.AddOption(currentUser.Username + "#" + currentUser.Discriminator
                , currentUser.Id.ToString());
        }

        return Task.FromResult(menuBuilder);
    }
}
