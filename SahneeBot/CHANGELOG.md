# CHANGELOG

## 1.0.2

Fixed an internal error that occurred when the bot does not have permission to post a changelog after a new release. Instead of throwing an error, admins will now be notified about the bot missing permissions.

## 1.0.1

The Sahnee-Bot is currently generating an increased amount of errors related to changelog permissions.
Functionality may be impacted while we work on a solution. Thank you for your patience.

## 1.0.0

Sahnee-Bot 1.0.0 is a complete rewrite of the Sahnee-Bot using the new Discord slash commands, with a more modern and stable architecture that will allow us to implement even more & better features in the future!

### Migration from 0.9.X

#### Using commands

We have migrated all your data for you automatically. One important thing has changed though: The bot used to only listen to a text channel called `bot-commands`. Now, by default the bot listens to all channels. If you want to restrict that, use the command `/config bind set <your channel name>` (Some commands can still be executed regardless of bound channel).

#### Roles & permissions

Users with the *Administrator* permission always have full access to the bot. Users with the *Ban Members* permission always have moderator permissions for the bot. If you need to customize these permissions you can additionally give roles Administrator/Moderator permissions via the `/config sahnee-permissions add <role> <permission>` command. 
We have also migrated the permissions you had configured in the old bot, make sure to double check that they are still correct using `/config sahnee-permissions list`.

#### Reporting

All commands to report warnings (such as `warnhistory`, `warningstoday`, ...) are now all under the `/warnings` command (e.g. `/warnings history`, `/warnings today`, ...). Many of the report commands now also support additional optional parameters!

### New Features

- The bot now uses Discord slash-commands. No more guessing what the commands are!
- Users can now opt out of receiving messages from the Bot by using the `/config pm opt-out` command.
- You can now customize the channel the Bot listens to for commands via `/config bind set <your channel name>`.
- You can now disable roles assigned by the bot using `/config role disable` or set their color using `/config role color <your hex color>`.
- You can now also get all warnings in a given timespan using `/warnings between <start date> <end date>`.
- You can now remove users that left your server (or got banned) using `/config old-users remove-list`.

### Bug Fixes

- Fixed #42 (https://github.com/Sahnee-DE/sahnee-bot/issues/42)
- Fixed #43 (https://github.com/Sahnee-DE/sahnee-bot/issues/43)
