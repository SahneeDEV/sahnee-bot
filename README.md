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
* `/warn @UserName "Your reason here! <optional http/s link>"` - Issues a warning to the given user. The user will also be notified personally about their most evil transgression.
* `/unwarn @UserName "Your reason here! <optional http/s link>"` - Revokes a warning from the given user. The user will also be notified personally.
* `/unwarn @UserName` - Revokes a warning from the given user. This allows for not giving a reason why the user has been unwarned. The user will also be notified personally.
* `/warnall "Your reason here! <optional http/s link>"` - Issues a warning to all users and bots on your server.
* `/warnhistory @UserName` - Shows the warning history for the given user. The default amount is `10`. It can be changes via the configuration file
* `/warnhistory @UserName <Amount>` - Does the same as the default command but shows a custom amount of historical entries.
* `/warnhistory @UserName all` - Prints all warnings for a specific user.
* `/warnleaderboard` - Prints a leaderboard of the top warned people on your guild. The default amount can be set in your configuration.
* `/warnleaderboard <Amount>` - Does the same as above but prints out a custom amount of people.
* `/warningstoday <@UserName>` - Shows all warnings of the user that he got in the last 24 hours. Can be used without an username, will then show all warnings/unwarns for the last 24 hours
* `/cleanuproles` - Will manually run the cleanup for not needed roles on your server.
* `/migratedatabase` - Will migrate your database to the latest scheme. Will not do anything if scheme is already up-to-date.
* `/userstats @UserName` - Will show you some Debug information about the given user
* `/changelog <Amount>` - Will show you the latest changelog. With a given number will show you the last X changelogs
* `/changeprefix <NewPrefix>` - Will change the prefix the bot is listening on in your guild. Can be any single character.
* `/addmodrole @Role` - Will grant a role on your discord server the moderator permissions.
* `/addadminrole @Role` - Will grant a role on your discord server the administrator permissions -> Be careful with this one

 _Information_:
- Bots can be warned but will never receive any message of their warning/unwarn like users would.
- Links at the end of warn/unwarn/warnall commands will display the file behind that link.
- Warn/Unwarn/Warnall can have images as attachments. Attachments will not be shown in the history

## 3. Permissions
Since version 0.9.93 the following changes happens:
* roles can now be individually set and removed. That means, you no longer rely on our given roles.
* users with the `Serveradministrator` privilege will always have access to every command.

Permissions are handled by role names on your discord server.

### If self-hosted:

Anything here can be changed to your needs.
As of now, the `Admins` are not used by the bot anymore because they got obsolete. We use a new permission system. They will make their way out of the config in a future release.

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
| CleanupWarningRoles | This job will go through all of the roles on your server and delete unassigned `warning-roles`| Every 5 hours|
| ClearDatabaseLog | This job will lock all commands while it's active. The database-log file will be written into the database file| Every day|

## 5. External API's

Since version 0.9.93 we support an external API module.

As of now, we currently provide only one API: discordbotlist.com
In the future this list will expand.

You don't need to use this external API module. You can just leave anything related to this empty in the config file.

## 6. How to install on your own hardware

### 6.1 Download an run

#### Linux:

1. Clone the repo:
   ```
   git clone https://github.com/Sahnee-DE/sahnee-bot.git
   cd sahnee-bot
   ```
3. Build the project
   ```
   dotnet publish -c Release -r linux-x64 --self-contained true -o ./build
   ```
   Make sure to replace `linux-x64` with whatever OS & processor architecture you are using.
5. Create a `config.json` file in your `build` directory. You can find a template for it [here](https://github.com/Sahnee-DE/sahnee-bot/blob/master/sahnee-bot/config.json).
   Make sure to insert your bot `Id` and `Token`. You must also change the value of `ChangeLogPath` to `"./changelog.txt"` (remove one dot)
7. To start your bot run the `sahnee-bot` command in the `build` directory:
   ```
   cd build
   ./sahnee-bot
   ```
    
 You are done! Enjoy your bot.

#### Windows:

Not supported yet.

### 6.2. The configuration file

In case you get lost, this is the default config:

```json
{
    "General": {
        "Id": "<Your ID here>",
        "Permissions": 268856320,
        "Token": "<Your token here>",
        "CommandPrefix": "/",
        "DatabasePath": "./sahnee-bot.db",
        "ChangeLogPath": "../changelog.txt",
        "CommandChannel": "bot-commands",
        "DatabaseCleanup": "1.00:00:00",
        "LogLevel": 1
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
          "Server Admin"
       ],
       "Mods": [
          "Moderator",
          "Mod",
          "Mods",
          "Staff",
          "Community Manager"
       ],
        "PunishMessage": "Some insult text here"
    },
    "ExternalApi": {
      "DiscordBotList": {
         "ApiUrl": "",
         "BotId": "",
         "AuthToken": ""
      }
   }
}
```

__Let's go through the settings__

| Setting | Description |
| --- | --- |
| `General:Id` | The client ID of the bot. The client we are hosting publicly is `603990770667487243`, if you want to run your own bot insert your ID here.|
| `General:Permission`| The permissions of this bot. By default the bot has permission `268856320` which makes it capable of creating roles and adding/removing roles from users. |
| `General:Token` | Your super secret bot token. [See how to generate one](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token) [1] |
| `General:CommandPrefix` | Defines the prefix for using any command. Can be any single character. By default it's set to `/`. |
| `General:DatabasePath` | Defines the path/location of your database file. We're using LiteDB [2] This will also be the location of the database log file. |
| `General:ChangeLogPath`| Defines the path of the changelog file. It's plain text. Not needed if you don't want to let your users know about changes or new commands. |
| `General:CommandChannel` | Defines the channel where the bot will listen for commands. |
| `General:DatabaseCleanup` | Job that will write the database-log file to the database file. The format is a **TimeSpan**. `0(days).00(hours):00(minutes):00(seconds)`. Please notice the `.` between the _days_ and _hours_. |
| `General:LogLevel` | Defines the loglevel. Can go from 1 to 5 where with every increased number the output of logs will also increase.|
| `WarningBot:WarningPrefix` | Users will be given a role based on their warning count. If this value is e.g. set to `"warnings: `" the role of 4 warnings will be named "warnings: 4". |
| `WarningBot:WarningRoleCleanup` | This is the cleanup job for your `warning-roles`. The format is a **TimeSpan**. `0(days).00(hours):00(minutes):00(seconds)`. Please notice the `.` between the _days_ and _hours_. |
| `WarningBot:DatabaseCollection` | Name for the Collection(Table) in the database. Only change if you really need to. |
| `WarningBot:WarningHistoryCount` | The default amount of warnings to show if the `/warnhistory @UserName` command is issued. |
| `WarningBot:WarningLeaderboardCount` | The default amount of people to show if the `/warnleaderboard` command is issued. |
| `WarningBot:Admins` | In order to issue warnings a user must have at least one of these permissions. |
| `WarningBot:PunishMessage`| The Reason of the warning a user gets when he uses the bot incorrectly, like wrong command, too much/many arguments and so on|
| `ExternalAPI:DiscordBotList:ApiUrl`| This is the API Url that will be provided to you by the discordbotlist site.|
| `ExternalAPI:DiscordBotList:BotId` | This is the BotId that will be provided to you by the discordbotlist site. |
| `ExternalAPI:DiscordBotList:AuthToken` | This is the AuthToken that will be provided to you by the discordbotlist site. |
[1] Source: https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token

[2] Source: http://www.litedb.org/
