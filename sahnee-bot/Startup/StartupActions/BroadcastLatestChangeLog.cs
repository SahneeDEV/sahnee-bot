#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using sahnee_bot.Configuration;
using sahnee_bot.Database;
using sahnee_bot.Database.Schema;
using sahnee_bot.Embeds;
using sahnee_bot.Logging;
using sahnee_bot.Util;

namespace sahnee_bot.Startup.StartupActions
{
    public static class BroadcastLatestChangeLog
    {
        //Variables
        private static readonly Logger _logger = new Logger();

        public static async Task BroadcastLatestChangeLogAsync(DiscordSocketClient bot)
        {
            try
            {
                //Wait 10 Seconds to rejoin every guild before executing
                Thread.Sleep(10000);
                //load the changelog once
                ChangeLogParser changeLogParser = new ChangeLogParser();
                string latestChangeLog = await changeLogParser.LatestChangeLog(StaticConfiguration.GetConfiguration().General.ChangeLogPath);
                //if file not found, don't publish a changelog
                if (latestChangeLog == "Could not read the changelog.")
                {
                    return;
                }
                //go through all guilds
                foreach (IGuild guild in bot.Guilds)
                {
                    //check if the bot already sent a message with the latest changelog
                    WarningBotChangeLogSchema guildChangeLogState = StaticDatabase.WarningChangeLogCollection()
                        .Query()
                        .Where(gid => gid.GuildId == guild.Id)
                        .FirstOrDefault();
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
                await _logger.Log(e.Message, LogLevel.Error, "BoradcastLatestChangeLog:BroadcastLatestChangeLogAsync");
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
                GetBotCommandsChannel getBotCommandsChannel = new GetBotCommandsChannel();
                //get all guild channels and filter the bot-commands channel out of them
                IGuildChannel botCommandsChannel = await getBotCommandsChannel.GetBotCommandsChannelAsync(guild);
                //Fancy Embed
                EmbedGenerator embedGenerator = new EmbedGenerator();
                List<EmbedBuilder> embeds = embedGenerator.GenerateSahneeBotEmbed("sahnee-bot got an update!",
                    "Look! I've got some new features/bugfixes",
                    "Over here! I got more news",
                    message, "Even more news");
                //send the message
                ISocketMessageChannel botCommandsMessageChannel = (ISocketMessageChannel)botCommandsChannel;
                foreach (EmbedBuilder embed in embeds)
                {
                    await botCommandsMessageChannel.SendMessageAsync(null, false, embed.Build());
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "BroadcastLatestChangeLog:SendChangeLogToGuildAsync");
            }
        }

        /// <summary>
        /// Sends a message with a custom amount of changes into the given guilds channel
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task SendChangeLogToGuildChannelAsync(IGuild guild, string message, ISocketMessageChannel channel)
        {
            try
            {
                //Fancy Embed
                EmbedGenerator embedGenerator = new EmbedGenerator();
                List<EmbedBuilder> embeds = embedGenerator.GenerateSahneeBotEmbed("sahnee-bot got an update!",
                    "Look! I've got some new features/bugfixes",
                    "Over here! I got more news",
                    message, "Even more news");
                //send the message
                foreach (EmbedBuilder embed in embeds)
                {
                    await channel.SendMessageAsync(null, false, embed.Build());
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message, LogLevel.Error, "BroadcastLatestChangeLog:SendChangeLogToGuildChannelAsync");
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
                //create new guid
                Guid g = Guid.NewGuid();

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
                        _id = g.ToString(), Seen = true,
                        Time = DateTime.Now, GuildId = guild.Id, LatestVersion = StaticBot.GetVersion()
                    };
                    //write to database
                    StaticDatabase.WarningChangeLogCollection().Insert(changeLogSchema);
                }
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message,LogLevel.Error,"BroadcastLatestChangeLog:ChangeGuildChangelogEntryAsync");
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
                await _logger.Log(e.Message,LogLevel.Error, "BroadcastLatestChangeLog:AddNewlyJoinedGuild");
            }
        }
    }
}
