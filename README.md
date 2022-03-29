# Sahnee-Bot

A bot for moderating your discord server. The Sahnee-Bot allows you to issue warnings to misbehaving users. The Bot also features several reporting commands to list the warnings of users on your server.

## Get the bot on your server

[Click, click, done](https://discordapp.com/oauth2/authorize?&client_id=689600370430836793&scope=bot&permissions=2416311382)  - Bot hosting provided by [sahnee.dev](https://sahnee.dev)

You're done, enjoy!

## Commands

The Sahnee-Bot uses Discord slash commands. The following commands are available:

Legend:
* `<name>` - Required
* `{name}` - Optional

### Warning commands

You can use these commands to issue warnings to your users:

* `/warn <@user> <reason>` - Issues a warning to the given user. The Sahnee-Bot also sends a private message to the user (unless opted out) *(Moderator permission required)*
* `/unwarn <@user> <reason>` - Revokes a warning of the given user. *(Moderator permission required)*

### Reporting commands

The commands below allow you to generate reports on the warnings of the people on your server:

* `/warnings top {max-rankings=10}` - Prints the `max-rankings` users with the most amount of warnings on your server.
* `/warnings history {max-amount=10} {user} {issuer=False}` - Gets the `max-amount` last warnings on your server. If `user` is specified, only the warnings of this user will be used. If `user` is specified and `issuer=True`, then warnings issued by this user will be used instead.
* `/warnings between <start> {end=Current date} {user} {issuer=False}` - Gets the all warnings between `start` and `end` on your server. If `user` is specified, only the warnings of this user will be used. If `user` is specified and `issuer=True`, then warnings issued by this user will be used instead.
* `/warnings random {amount=1} {user} {issuer=False}` - Gets `amount` random warnings on your server. If `user` is specified, only the warnings of this user will be used. If `user` is specified and `issuer=True`, then warnings issued by this user will be used instead.
* `/warnings today {user} {issuer=False}` - Gets all warnings on your server within the last 24 hours. If `user` is specified, only the warnings of this user will be used. If `user` is specified and `issuer=True`, then warnings issued by this user will be used instead.

### Configuration commands

The following commands can be used to customize the bot on your server:

* `/config bind set <channel>` - Sets the channel the bot is bound to. This means that the bot will only process commands sent in the given channel. *(This command ignores the bound channel)* *(Administrator permission required)*
* `/config bind unset` - Removes the bound channel from the bot. *(This command ignores the bound channel)* *(Administrator permission required)*
* `/config bind get` - Gets the channel the bot is currently bound to. *(This command ignores the bound channel)*
* `/config sahnee-permission add <role> <permission>` - Adds a permission to the given role. *(Administrator permission required)*
* `/config sahnee-permission remove <role> {permission=All}` - Removes a permission from the given role. *(Administrator permission required)*
* `/config sahnee-permission list` - Lists all roles on your server that have permissions for the bot attached to them. *(Administrator permission required)*
* `/config role enable` - Enables role creating, causing every user to be assigned a role that displays the amount of warnings they have. (Enabled by default) *(Administrator permission required)*
* `/config role disable` - Disables the role handling. *(Administrator permission required)*
* `/config role color <color>` - Sets the color of warning roles created by the bot. The role color has to he a hex string *(Administrator permission required)*
* `/config role prefix <prefix>` - Sets the prefix of the role names created by the bot (Defaults to `warning: `). *(Administrator permission required)*
* `/config role status` - Prints the current configuration of the role handling (if roles are created and the prefix). *(Administrator permission required)*
* `/config pm opt-out` - Opts out of receiving messages from this bot on the current server. *(This command ignores the bound channel)*
* `/config pm opt-in` - Opts back into receiving messages from this bot on the current server. *(This command ignores the bound channel)*
* `/config pm am-i-opted-out` - Checks if you are currently opted out of receiving messages from the bot on this server. *(This command ignores the bound channel)*
* `/config old-users remove-list` - Removes users that left, got banned or got their account deleted, from the current server. *(Administrator permission required)*

### Miscellaneous commands

The bot also has several other commands that don't fit into a category:

* `/help` - Prints some general information about the bot.
* `/changelog {version=latest} {all=False}` - Gets the changelog of the given `version` of the bot. If `all=True` all changelogs after this version will be returned as well.
* `/cleanup-roles` - Deletes unused roles created by the role handling system that assigns users roles based on their warning number. This command is automatically executed regularly.

## Permissions

The bot has three permissions:

* `Administrator` - Users with this permission have full access to the bot. Every user with "Administrator" discord permissions automatically has this bot permission.
* `Moderator` - Users with this permission can issue warnings with the bot. Every user with "Ban Members" discord permissions automatically has this bot permission.
* `None` - Users without permissions cannot configure the bot or issue warnings.

If the default permissions assigned do not suffice you can assign permissions to roles using the `/config sahnee-permission` commands.

## Self hosted guide

You can also host the bot on your own hardware if you want.

Currently we are building the sahnee-bot for linux-x64.
If you want to run the sahnee-bot on a Windows-system, you have to build the sahnee-bot on your own.

### Migrating from Version 0.9.X to Version > 1.0.0

Because of the change from LiteDb as Database to PostgreSQL as Database, you need to migrate you database to the new schema.

This will be a step by step guide.
1. Install a PostgreSQL server and create a database with a user that has full access to this database.
2. Get the modified version of the [LiteDbStudio](https://github.com/Sahnee-DE/LiteDB.Studio) and extract it. (We only increased the limit of entries that can be displayed at once)
3. Get our [migration tool](https://github.com/Sahnee-DE/sahnee-bot-migrator) and extract it.
4. Stop your bot.
5. Download the latest version of the sahnee-bot and run it once, so the database gets initialized with the default tables and columns.
6. Download the LiteDb files (If the log rotation hasn't been running since the last change there will be two files, one for the database and one log file)
7. Open the .db file (Make sure you have the log file in the same folder as the db file if you happen to have one)
8. Now double-click on a table and hit `Run`, next change the tab to `Text`, copy all of the content into the matching file in the migration-tools `db` folder. Repeat this for every table.
9. The tricky part: in your sahnee-bot-migrator folder there is a `appsettings.json` that needs the `ConnectionStrings:SahneeBotModelContext` to be configured with your PostgreSQL connection data. Please note, that this string differs from the `appsettings.json` used by the bot. If you use non-alphanumerical characters in your password, you need to encode them. Encoding can be done via [this page](https://www.w3schools.com/tags/ref_urlencode.asp). 
10. Now you run the migration tool. You need to launch the application via a command line (CMD and Powershell work as well).
11. After the successful migration you are ready to go. Start your bot and have fun!

### Prerequisites

Starting with version 1.0.0 the sahnee-bot switched from the previously used NoSQL LiteDb to a PostgreSQL.
Thus, for the sahnee-bot to work you now also need to setup a PostgreSQL database.
Please be aware, that we cannot provide support for your PostgreSQL server.

You need:
* PostgreSQL `Version 13.X` and later.
* A user that has full access to a database.

### How to get the bot up and running on linux

* Download the .zip file to your server with for example `wget`.
* Extract the zip file in a directory. `unzip SahneeBot.zip -d <your destination directory>` (For this you can use the `unzip` package. This can be installed via `sudo apt install unzip`)
* Copy the example configuration json from below in the `appsettings.json` file of the unzipped SahneeBot folder.
* If you haven't done so far, you need to create a `Discord Application`. A guide can be found here: [Creating a Discord Bot](https://discordnet.dev/guides/getting_started/first-bot.html) - (To be able to use all features you need to have the following Permission integer: `1101927607366`)
* Insert the Discord Application Token into the `Discord:Token` string.
* The next step would be to configure the `ConnectionStrings:SahneeBotModelContext`. (Please be aware, that if you use characters like \` or ; in your password, you need to wrap your password string in `'special;password:'`).
* Also required is the `BotSettings:ErrorWebhookUrl` parameter. You __need__ to provide this. [How to create a Webhook](https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhooks)
* For further configuration customization please refer to the *__Configuration__* part of this Readme.
* It's recommended to run the bot as a service. Please refer to a guide that suits your OS.
* Finally you can start the bot.

If you encounter any errors, you can join our [support server](https://discord.gg/kuFQZxkS)

### Configuration

If hosting the bot yourself you will need to adjust the `appsettings.json` file:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Debug",
            "Microsoft": "Debug"
        }
    },
    "MachineId": 1,
    "ConnectionStrings": {
      "SahneeBotModelContext": "Host=host;Database=database;Username=username;Password=password"
    },
    "Discord": {
      "Token": "ABCD1234",
      "Implementation": "Socket"
    },
    "BotSettings": {
        "IconUrl": "https://sahnee.dev/wp-content/uploads/2020/04/sahnee-bot-150x150.png",
        "Url": "https://sahnee.dev/en/project/sahnee-bot/",
        "WarningRolePrefix": "warning: ",
        "WarningRoleColor": "#607D8B",
        "SupportServer": "https://discord.gg/FfVurUzVfE",
        "Changelog": "./CHANGELOG.md",
        "ReleaseInformation": "./ReleaseInformation.txt",
        "InviteUrl": "https://discord.com/api/oauth2/authorize?client_id=689600370430836793&permissions=268627014&redirect_uri=https%3A%2F%2Fsahnee.dev%2Fen%2Fproject%2Fsahnee-bot%2F&scope=bot%20applications.commands",
        "ErrorWebhookUrl": "https://discord.com/api/webhooks/12345/abcde",
        "Jobs": {
            "CleanupWarningRoles": "1:00:00"
        }
    }
}
```

* `Logging` - All keys here allow you to customize the logging of the bot.
* `MachineId` - Used to modify the key generation of the bot. When using multiple bots in the same database every bot instance needs a unique `MachineId`.
* `ConnectionStrings:SahneeBotModelContext` - The credentials to the PostgreSQL database of the bot.
* `Discord:Token` - The discord token of your bot.
* `Discord:Implementation` - Allows you to set if you want to use the `Socket` or `Rest` discord API implementation. Don't change unless you know what you are doing and absolutely need it.
* `BotSettings:IconUrl` - The icon of the bot used in messages.
* `BotSettings:Url` - The homepage the bot links to.
* `BotSettings:WarningRolePrefix` - The default prefix for warning roles of servers that have not configured their own.
* `BotSettings:WarningRoleColor` - The default color for warning roles on a server.
* `BotSettings:SupportServer` - A link to the support page for the bot.
* `BotSettings:Changelog` - The path to the changelog file on disk. Will be scanned for a new version of bot startup.
* `BotSettings:ReleaseInformation` - A file that contains information about the release of the bot. Will be printed in the `/help` command.
* `BotSettings:InviteUrl` - The invite URL users should use to invite the bot to their servers.
* `BotSettings:ErrorWebhookUrl` - A webhook errors will be sent to.
* `BotSettings:Jobs:CleanupWarningRoles` - The frequency in how often warnings created by the bot that are no longer used will be deleted.

Time spans for jobs are formatted in the format documented [here](https://docs.microsoft.com/en-us/dotnet/api/system.timespan.parse?view=net-6.0#system-timespan-parse(system-string)) under the section "Remarks" (`[ws][-]{ d | [d.]hh:mm[:ss[.ff]] }[ws]`).
