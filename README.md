# CounterstrikeSharp - Player Sessions

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![Discord Support](https://img.shields.io/discord/289448144335536138?label=Discord%20Support&color=darkgreen)](https://discord.gg/bkuF8xKHUt)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-player-sessions?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-player-sessions/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-player-sessions](https://img.shields.io/github/issues/Kandru/cs2-player-sessions?color=darkgreen)](https://github.com/Kandru/cs2-player-sessions/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

This plugin logs connecting and disconnecting players as well as showing a player his playing stats (like how many connections, playtime, etc.).

## Current Features

- show player connection and disconnection (with Country/City)

## Road Map

- show player connection (and country) and disconnection
- show player session (when in game)
- save total player connections (with cooldown to avoid reconnections within a specific time frame)
- save player overall connection time
- save player overall play time
- save player overall play time per team (Spectator / T / CT)
- save player overall alive/death time

## Plugin Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-player-sessions/releases/).
2. Move the "PlayerSessions" folder to the `/addons/counterstrikesharp/configs/plugins/` directory of your gameserver.
3. Restart the server.

## Plugin Update

Simply overwrite all plugin files and they will be reloaded automatically or just use the [Update Manager](https://github.com/Kandru/cs2-update-manager/) itself for an easy automatic or manual update by using the *um update PlayerSessions* command.

## Commands

There is currently no client-side command.

## Configuration

This plugin automatically creates a readable JSON configuration file. This configuration file can be found in `/addons/counterstrikesharp/configs/plugins/PlayerSessions/PlayerSessions.json`.

```json
{
  "enabled": true,
  "debug": false,
  "ConfigVersion": 1
}
```

You can either disable the complete PlayerSessions Plugin by simply setting the *enable* boolean to *false* or specify a specific map where you want this plugin to be disabled. This allows for a maximum customizability.

## Compile Yourself

Clone the project:

```bash
git clone https://github.com/Kandru/cs2-player-sessions.git
```

Go to the project directory

```bash
  cd cs2-player-sessions
```

Install dependencies

```bash
  dotnet restore
```

Build debug files (to use on a development game server)

```bash
  dotnet build
```

Build release files (to use on a production game server)

```bash
  dotnet publish
```

## License

Released under [GPLv3](/LICENSE) by [@Kandru](https://github.com/Kandru).

## Authors

- [@derkalle4](https://www.github.com/derkalle4)

## Dependencies

- Depends on [IP2Country](https://github.com/RobThree/IP2Country?tab=readme-ov-file)
