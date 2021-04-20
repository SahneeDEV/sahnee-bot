using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Logging;

namespace sahnee_bot.commands.CommandActions
{
    public class ChangePrefixAction
    {
        //Variables
        private readonly Logger _logger = new Logger();
        
        /// <summary>
        /// Action for changing the prefix for a specific guild
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="channel"></param>
        /// <param name="newPrefix"></param>
        /// <returns></returns>
        public async Task ChangePrefixActionAsync(IGuild guild, ISocketMessageChannel channel, string newPrefix)
        {
            try
            {
                //check that the new prefix only consists of one character
                if (newPrefix.Length > 1)
                    //check if there are white spaces that count
                    if (newPrefix.Replace(" ", "").Length > 1)
                    {
                        //well input is incorrect let the user know about that
                        await channel.SendMessageAsync(
                            $"🤨 The new prefix {newPrefix} you gave me consists of more than one character. But a prefix only can consist of a single character.");
                        return;
                    }

                var tempNewPrefix = newPrefix.ToCharArray();
                //change the prefix
                //check if there already exists a custom prefix for the guild
                WarningBotPrefixSchema customPrefix = null;
                try
                {
                    customPrefix = StaticDatabase.WarningPrefixCollection().Query()
                        .Where(g => g.GuildId == guild.Id)
                        .Single();
                }
                catch (Exception e)
                {if (e.GetType() == typeof(System.InvalidOperationException))
                    {
                        //ignore, because no custom prefix has been set
                    }
                    else
                    {
                        await _logger.Log(e.Message, LogLevel.Error, "ChangePrefixAction:ChangePrefixActionAsyn");
                    }
                }

                if (customPrefix == null)
                {
                    //create new unique guid
                    Guid g = Guid.NewGuid();
                    //create new
                    customPrefix = new WarningBotPrefixSchema
                    {
                        _id = g.ToString(), Time = DateTime.Now, GuildId = guild.Id,
                        CustomPrefix = tempNewPrefix[0]
                    };
                    //add to the database
                    StaticDatabase.WarningPrefixCollection().Insert(customPrefix);
                }
                else
                {
                    customPrefix.CustomPrefix = tempNewPrefix[0];
                    customPrefix.Time = DateTime.Now;
                    //update in the database
                    StaticDatabase.WarningPrefixCollection().Update(customPrefix);
                }

                //send feedback to the user
                await channel.SendMessageAsync(
                    $"🙃 Your prefix has been changed! You may now use `{tempNewPrefix[0]}` as your new command prefix.");
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "ChangePrefixAction:ChangePrefixActionAsync");
            }
        }
    }
}