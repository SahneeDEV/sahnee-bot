using System.Collections.Generic;
using Discord;

namespace sahnee_bot.Embeds
{
    public class HelpEmbed
    {
        
        /// <summary>
        /// Returns the finished and styled help embed for the help dialog
        /// </summary>
        /// <returns></returns>
        public List<EmbedBuilder> HelpEmbedBuilder()
        {
            EmbedGenerator embedGenerator = new EmbedGenerator();

            string content = "`/help` → shows this dialog.\n" +
                             "`/warn @UserName \"Your reason here\" <optional: http/s link>`" +
                             " → Issues a warning to the given user. If a link is provided, the destination will be shown.\n" +
                             "`/warn @Role \"Your reason here\" <optional: http/s link>`" +
                             " → Issues a warning to all users in the given role. If a link is provided, the destination will be shown.\n" +
                             "`/unwarn @UserName \"Your reason here\" <optional: http/s link>`" +
                             " → Revokes a warning from the given user. If a link is provided, the destination will be shown.\n" +
                             "`/unwarn @UserName`" +
                             " → Revokes a warning from the given user. No reason will be given.\n" +
                             "`/unwarn @Role \"Your reason here\" <optional: http/s link>`" +
                             " → Revokes a warning from all users within that role.\n" +
                             "`/unwarn @Role`" +
                             " → Revokes a warning from all users withing that role. No reason will be given.\n" +
                             "`/warnall \"Your reason here\" <optional: http/s link>`" +
                             " → Issues a warning to all users and bots on your server.\n" +
                             "`/warnhistory @UserName`" +
                             " → Shows the warning history for a given user.\n" +
                             "`/warnhistory @UserName <Amount>`" +
                             " → Shows the warning history for a given user with a depth or your given _Amount_.\n" +
                             "`/warnhistory @UserName all`" +
                             " → Prints all warnings for the specified user.\n" +
                             "`/warnerhistory @UserName <Amount>`" +
                             " → Prints all warnings/unwarns a the user ever issued.\n" +
                             "`/warnleaderboard`" +
                             " → Shows you a leaderboard with your top warned users.\n" +
                             "`/warnleaderboard <Amount>`" +
                             " → Shows you a leaderboard with your top warned users but instead of the top 3 it will display your _Amount_.\n" +
                             "`/warningstoday`" +
                             " → Shows you all warnings that have been issued today.\n" +
                             "`/warningstoday @UserName`" +
                             " → Shows you all warnings the specified user got today.\n" +
                             "`/cleanuproles`" +
                             " → Will manually run the cleanup-job for unused warning roles on your server.\n" +
                             "`/changelog <Amount>`" +
                             " → Will show the latest or the last <Amount> changelogs.\n" +
                             "`/changeprefix <NewPrefix>`" +
                             " → Will change your current prefix to a custom one.\n" +
                             "`/add<mod/admin>role` @RoleName" +
                             " → Will enable all users in the given role(s) to execute the commands based on the given privileges\n" +
                             "`/list<mod/admin>roles`" +
                             " → Will list you all available roles\n";
            
            List<EmbedBuilder> embeds = embedGenerator.GenerateSahneeBotEmbed("sahnee-bot help",
                "Here we have a list of all available commands and their features.\n" +
                "In this case we are using '/' as prefix, replace it if you've chosen another one.\n" +
                "By using the sahnee-bot you agree to our [privacy policy](https://sahnee.dev/en/sahnee-bot-privacy-policy/)."
                ,
                "Commands:", content
                , "Even more commands 😮");
            
            //Build embed and return
            return embeds;
        }
    }
}
