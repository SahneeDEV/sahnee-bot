using System.Collections.Generic;
using Discord;

namespace sahnee_bot.Embeds
{
    public class TutorialEmbed
    {
        /// <summary>
        /// Returns the finished and styled tutorial embed for the initial tutorial
        /// </summary>
        /// <returns></returns>
        public List<EmbedBuilder> TutorialEmbedBuilder()
        {
            EmbedGenerator embedGenerator = new EmbedGenerator();

            string content = "If you want to see all available commands just use `/help`.\n" +
                             "**Basic commands are:**\n" +
                             "`/warn @UserName \"Your reason here\"` → Example: `/warn @SomeUser because you deserve it`\n" +
                             "`/unwarn @UserName \"Your reason here\"` → Example: `/unwarn @SomeUser because you did something good.`\n" +
                             "`/warnall \"Your reason here\"` → Example: `/warnall all of you did bad!`\n" +
                             "`/warnleaderboard` → Will show a leaderboard of the top 3.\n" +
                             "`/warnhistory @UserName` → Example: `/warnhistory @SomeUser`\n" +
                             "`/warningstoday @UserName` → Example: `/warningstoday @SomeUser`\n" +
                             "Permissions are set as follows:\n" +
                             "`/addmodrole @RoleName` or `/addadminrole @RoleName`\n" +
                             "Serverowners will always be able to execute every command.\n" + 
                             "If you want to get a full documentation of all commands visit our GitHub: [sahnee-bot](https://github.com/Sahnee-DE/sahnee-bot#2-commands)\n";
            
            List<EmbedBuilder> embeds = embedGenerator.GenerateSahneeBotEmbed("Welcome to sahnee-bot",
                "Thank you for using sahnee-bot!\n",
                "Getting started", content
                , "Even more Information for you");
            return embeds;
        }
    }
}
