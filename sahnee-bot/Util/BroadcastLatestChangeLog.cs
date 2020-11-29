#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;

namespace sahnee_bot.Util
{
    public static class BroadcastLatestChangeLog
    {
        //Variables
        private static readonly Logging _logging = new Logging();

        public static async Task BroadcastLatestChangeLogAsync(DiscordSocketClient bot)
        {
            try
            {
                //Wait 10 Seconds to rejoin every guild before executing
                Thread.Sleep(10000);
                //load the changelog once
                ChangeLogParser changeLogParser = new ChangeLogParser();
                string latestChangeLog = await changeLogParser.LatestChangeLog(StaticConfiguration.GetConfiguration().General.ChangeLogPath);
                //go through all guilds
                foreach (IGuild guild in bot.Guilds)
                {
                    //check if the bot already sent a message with the latest changelog
                    WarningBotChangeLogSchema guildChangeLogState = StaticDatabase.WarningChangeLogCollection()
                        .Query()
                        .Where(gid => gid.GuildId == guild.Id)
                        .SingleOrDefault();
                    //check if there is an entry
                    if (guildChangeLogState != null)
                    {
                        //check if the guild already received a message at all
                        if (!guildChangeLogState.Seen)
                        {
                            //message guild
                            await SendChangeLogToGuildAsync(guild, latestChangeLog);
                            await ChangeGuildChangelogEntryAsync(guild, guildChangeLogState);
                        }
                        //check if the guild already received the latest message
                        if (guildChangeLogState.Seen
                            && guildChangeLogState.LatestVersion != StaticBot.GetVersion())
                        {
                            //message guild
                            await SendChangeLogToGuildAsync(guild, latestChangeLog);
                            await ChangeGuildChangelogEntryAsync(guild, guildChangeLogState);
                        }
                        //else do nothing because we already annoyed them with the changelog
                    }
                    else
                    {
                        //guild has never ever received a message
                        await SendChangeLogToGuildAsync(guild, latestChangeLog);
                        await ChangeGuildChangelogEntryAsync(guild, null);
                    }
                }
            }
            catch (Exception e)
            {
                await _logging.LogToConsoleBase(e.Message);
            }
        }

        /// <summary>
        /// Sends a message with the latest changelog into the given guild
        /// </summary>
        /// <param name="guild">the guild to send the message to</param>
        /// <param name="message">the message</param>
        /// <returns></returns>
        private static async Task SendChangeLogToGuildAsync(IGuild guild, string message)
        {
            try
            {
                //get all guild channels and filter the bot-commands channel out of them
                IGuildChannel botCommandsChannel = null!;
                IReadOnlyCollection<IGuildChannel> guildChannels = await guild.GetChannelsAsync();
                foreach (IGuildChannel guildChannel in guildChannels)
                {
                    if (guildChannel.Name == StaticConfiguration.GetConfiguration().General.CommandChannel)
                    {
                        botCommandsChannel = guildChannel;
                    }
                }
                //Fancy Embed
                string iconUrl = "https://sahnee.dev/wp-content/uploads/2020/04/sahnee-bot-300x300.png";
                var embed = new EmbedBuilder
                {
                    Title = "sahnee-bot got an update!",
                    Color = Color.Purple,
                    Description = "Look! I've got some new features/bugfixes",
                    Timestamp = DateTimeOffset.Now
                };
                embed.AddField(StaticBot.GetVersion(), message.Remove(0, message.IndexOf('\n')));
                embed.WithFooter(footer => footer.Text = "proudly presented by sahnee.dev");
                embed.WithThumbnailUrl(iconUrl);
                embed.WithAuthor(author => 
                { 
                    author.Name = "sahnee-bot";
                    author.Url = "https://github.com/Sahnee-DE/sahnee-bot";
                    author.IconUrl = iconUrl;
                });
                //send the message
                ISocketMessageChannel botCommandsMessageChannel = (ISocketMessageChannel)botCommandsChannel;
                await botCommandsMessageChannel.SendMessageAsync(null, false, embed.Build());
            }
            catch (Exception e)
            {
                await _logging.LogToConsoleBase(e.Message);
            }
        }

        /// <summary>
        /// Handles database update for the changelog seen and so on
        /// </summary>
        /// <param name="guild">the guild</param>
        /// <param name="changeLogSchema">schema if available, if not a new one will be created</param>
        /// <returns></returns>
        private static async Task ChangeGuildChangelogEntryAsync(IGuild guild, WarningBotChangeLogSchema? changeLogSchema)
        {
            try
            {
                //increment the id
                StaticDatabase.UpdateWarningChangeLogId();

                if (changeLogSchema != null)
                {
                    changeLogSchema.Seen = true;
                    changeLogSchema.LatestVersion = StaticBot.GetVersion();
                    changeLogSchema.Time = DateTime.Now;
                    //update the element in the database
                    StaticDatabase.WarningChangeLogCollection().Update(changeLogSchema);
                }
                else
                {
                    //create a new schema instance
                    changeLogSchema = new WarningBotChangeLogSchema
                    {
                        _id = StaticDatabase.GetWarningChangeLogId(), Seen = true,
                        Time = DateTime.Now, GuildId = guild.Id, LatestVersion = StaticBot.GetVersion()
                    };
                    //write to database
                    StaticDatabase.WarningChangeLogCollection().Insert(changeLogSchema);
                }
            }
            catch (Exception e)
            {
                await _logging.LogToConsoleBase(e.Message);
            }
        }

        /// <summary>
        /// Function that handles newly arriving guilds and prevents them from getting the changelog message with a restart of the server or service
        /// </summary>
        /// <param name="guild">the freshly joined guild</param>
        /// <returns></returns>
        public static async Task AddNewlyJoinedGuild(IGuild guild)
        {
            try
            {
                //insert the new guild in the database, so they dont get the current changelog
                await ChangeGuildChangelogEntryAsync(guild, null);
            }
            catch (Exception e)
            {
                await _logging.LogToConsoleBase(e.Message);
            }
        }
    }
}
