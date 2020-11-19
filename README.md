# sahnee-bot
A bot for moderating warnings on your discord server.

With the sahnee-bot you can issue warnings. May it be a single user or everyone on your server. sahnee-bot got you covered.
Furthermore you can generate a neat looking leaderboard for your server. Not to forget the history about all warnings a user ever got.

---

## Index:
1. Get the bot on your server
2. Commands
3. Permissions
4. Jobs
5. How to install on your own hardware
    1. Download and run
    2. The configuration file

---

## 1. Get the bot on your server

[Click, click, done](https://discordapp.com/oauth2/authorize?&client_id=689600370430836793&scope=bot&permissions=268856320)  - Bot hosting provided by [sahnee.dev](https://sahnee.dev)

You're done, enjoy!

## 2. Commands
* `/warn @UserName "Your reason here!"` - Issues a warning to the given user. The user will also be notified personally about their most evil transgression.
* `/unwarn @UserName "Your reason here!"` - Revokes a warning from the given user. The user will also be notified personally.
* `/unwarn @UserName` - Revokes a warning from the given user. This allows for not giving a reason why the user has been unwarned. The user will also be notified personally.
* `/warnall "Your reason here!"` - Issues a warning to all users and bots on your server.
* `/warnhistory @UserName` - Shows the warning history for the given user. The default amount is `10`. It can be changes via the configuration file
* `/warnhistory @UserName <Amount>` - Does the same as the default command but shows a custom amount of historical entries.
* `/warnhistory @UserName all` - Prints all warnings for a specific user.
* `/warnleaderboard` - Prints a leaderboard of the top warned people on your guild. The default amount can be set in your configuration.
* `/warnleaderboard <Amount>` - Does the same as above but prints out a custom amount of people.
* `/warningstoday @UserName` - Shows all warnings of the user that he got in the last 24 hours.
* `/cleanuproles` - Will manually run the cleanup for not needed roles on your server.
* `/migratedatabase` - Will migrate your database to the latest scheme. Will not do anything if scheme is already up-to-date.
* `/userstats @UserName` - Will show you some Debug information about the given user

 _Information_:
Bots can be warned but will never receive any message of their warning like users would.

## 3. Permissions
Permissions are handled by role names on your discord server.

### If hosted by sahnee.dev:

You need to make sure that the default roles exist on your server.
Otherwise you cannot control the bot.

These are the default roles:

| Module | Role-name |
| ----| -----|
| WarningBot | `Owner`, `Administrator`, `Admin`, `Server Admin`, `Moderator`, `Mod`, `Mods`, `Staff`, `Community Manager` |

### If self-hosted:

The `config.json` provides a section called `Admins` for each "Module" of the bot to be accessed with separate roles.

If you host this bot yourself, you can configure the roles by your needs.
You can add any name into the configuration file `config.json` into the _`Admins`_ array. This array can contain **1+n** roles.

Example:
```json
    "WarningBot": {
        "WarningPrefix": "warning: ",
        "WarningRoleCleanup": "0.00:30:00",
        "DatabaseCollection": "warningbot",
        "WarningHistoryCount": 10,
        "WarningLeaderboardCount": 3,
        "Admins": [
            "Owner",
            "Administrator",
            "Admin",
            "Server Admin",
            "Moderator",
            "Mod",
            "Mods",
            "Staff",
            "Community Manager"
        ],
        "PunishMessage": "Some insulting text here"
    },
```

This means, everyone that is in one of the roles mentioned above will be able to access the **Warn** Module.


## 4. Jobs

Because we've encountered that having so many `warning-roles` on your server, will make creating and modifying your own roles a bit laggy.
Furthermore because there is a limit on how many roles your server can have, we've create the job-system.

__Currently these jobs are available:__

| Job-Name | Description | Default execution time
| -----| ----- | ------|
| CleanupWarningRoles | This job will go through all of the roles on your server and delete unassigned `warning-roles`| Every day|
| ClearDatabaseLog | This job will lock all commands while it's active. The database-log file will be written into the database file|


## 5. How to install on your own hardware

### 5.1 Download an run
#### Linux:
1. Compile for Linux
2. Edit the configuration
3. Get the bot working: Start your bot `./sahnee-bot`
    
 You are done! Enjoy your bot.

#### Windows:

1. Compile for Windows
2. Edit the configuration
3. start the __.exe__ file

### 5.2. The configuration file

In case you get lost, this is the default config:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        }
    },
    "General": {
        "Id": "<Your ID here>",
        "Permissions": 8,
        "Token": "<Your token here>",
        "CommandPrefix": "/",
        "DatabasePath": "./sahnee-bot.db",
        "CommandChannel": "bot-commands",
        "DatabaseCleanup": "1.00:00:00"
    },
    "WarningBot": {
        "WarningPrefix": "warning: ",
        "WarningRoleCleanup": "0.05:00:00",
        "DatabaseCollection": "warningbot",
        "WarningHistoryCount": 10,
        "WarningLeaderboardCount": 3,
        "Admins": [
            "Owner",
            "Administrator",
            "Admin",
            "Server Admin",
            "Moderator",
            "Mod",
            "Mods",
            "Staff",
            "Community Manager"
        ],
        "PunishMessage": "Some insult text here"
    }
}
```

__Let's go through the settings__

| Setting | Description |
| --- | --- |
| `General:Id` | The client ID of the bot. The client we are hosting publicly is `603990770667487243`, if you want to run your own bot insert your ID here.|
| `General:Permission`| The permissions of this bot. By default the bot has permission `8` which makes it an administrator. (This is subject to change in a later version) |
| `General:Token` | Your super secret bot token. [See how to generate one](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token) [1] |
| `General:CommandPrefix` | Defines the prefix for using any command. Can be any single character. By default it's set to `/`. |
| `General:DatabasePath` | Defines the path/location of your database file. We're using LiteDB [2] This will also be the location of the database log file. |
| `General:CommandChannel` | Defines the channel where the bot will listen for commands. |
| `General:DatabaseCleanup` | Job that will write the database-log file to the database file. |
| `WarningBot:WarningPrefix` | Users will be given a role based on their warning count. If this value is e.g. set to `"warnings: `" the role of 4 warnings will be named "warnings: 4". |
| `WarningBot:WarningRoleCleanup` | This is the cleanup job for your `warning-roles`. The format is a **TimeSpan**. `0(days).00(hours):00(minutes):00(seconds)`. Please notice the `.` between the _days_ and _hours_. |
| `WarningBot:DatabaseCollection` | Name for the Collection(Table) in the database. Only change if you really need to. |
| `WarningBot:WarningHistoryCount` | The default amount of warnings to show if the `/warnhistory @UserName` command is issued. |
| `WarningBot:WarningLeaderboardCount` | The default amount of people to show if the `/warnleaderboard` command is issued. |
| `WarningBot:Admins` | In order to issue warnings a user must have at least one of these permissions. |
| `WarningBot:PunishMessage`| The Reason of the warning a user gets when he uses the bot incorrectly, like wrong command, too much/many arguments and so on|

[1] Source: https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token

[2] Source: http://www.litedb.org/
