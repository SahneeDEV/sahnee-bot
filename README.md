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
* `/config role color <color>` - Sets the color of warning roles created by the bot. *(Administrator permission required)*
* `/config role prefix <prefix>` - Sets the prefix of the role names created by the bot (Defaults to `warning: `). *(Administrator permission required)*
* `/config role status` - Prints the current configuration of the role handling (if roles are created and the prefix). *(Administrator permission required)*
* `/config pm opt-out` - Opts out of receiving messages from this bot on the current server. *(This command ignores the bound channel)*
* `/config pm opt-in` - Opts back into receiving messages from this bot on the current server. *(This command ignores the bound channel)*
* `/config pm am-i-opted-out` - Checks if you are currently opted out of receiving messages from the bot on this server. *(This command ignores the bound channel)*

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

TODO TODO TODO

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
      "Token": "ABCD1234"
    },
    "BotSettings": {
        "IconUrl": "https://sahnee.dev/wp-content/uploads/2020/04/sahnee-bot-150x150.png",
        "Url": "https://sahnee.dev/en/project/sahnee-bot/",
        "WarningRolePrefix": "warning: ",
        "SupportServer": "https://discord.gg/FfVurUzVfE",
        "Changelog": "./CHANGELOG.md",
        "ErrorWebhookUrl": "https://discord.com/api/webhooks/12345/abcde",
        "Jobs": {
            "CleanupWarningRoles": 60
        }
    }
}
```

* `Logging` - All keys here allow you to customize the logging of the bot.
* `MachineId` - Used to modify the key generation of the bot. When using multiple bots in the same database every bot instance needs a unique `MachineId`.
* `ConnectionStrings:SahneeBotModelContext` - The credentials to the PostgreSQL database of the bot.
* `Discord:Token` - The discord token of your bot.
* `BotSettings:IconUrl` - The icon of the bot used in messages.
* `BotSettings:Url` - The homepage the bot links to.
* `BotSettings:WarningRolePrefix` - The default prefix for warning roles of servers that have not configured their own.
* `BotSettings:SupportServer` - A link to the support page for the bot.
* `BotSettings:Changelog` - The path to the changelog file on disk. Will be scanned for a new version of bot startup.
* `BotSettings:ErrorWebhookUrl` - A webhook errors will be sent to.
* `BotSettings:Jobs:CleanupWarningRoles` - The frequency in minutes how often warnings created by the bot that are no longer used will be deleted.