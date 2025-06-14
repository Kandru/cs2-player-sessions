> [!CAUTION]
> Work in progress. DOES NOT WORK ON YOUR SERVER.

> [!TIP]
> This plug-in got created prior to our CS2 Challenges plug-in and will be re-created to fullfill another purpose. Challenges are a unique way to offer your players the possibility to solve challenges in order to get something in return (e.g. giving VIP or other stuff). Check out our CS2 Challenges repository to get a good working version!

# CounterstrikeSharp - Player Sessions

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![Discord Support](https://img.shields.io/discord/289448144335536138?label=Discord%20Support&color=darkgreen)](https://discord.gg/bkuF8xKHUt)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-player-sessions?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-player-sessions/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-player-sessions](https://img.shields.io/github/issues/Kandru/cs2-player-sessions?color=darkgreen)](https://github.com/Kandru/cs2-player-sessions/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

This plugin logs connecting and disconnecting players as well as showing a player his playing stats (like how many connections, playtime, etc.).

## Current Features

- show player connection and disconnection (with Country and City)
- show player his session (after joined the game)
- save total player connections (with cooldown to avoid reconnections within a specific time frame)
- save player overall play time

## Road Map

- save player overall play time per team (Spectator / T / CT)
- save player overall alive/death time

## Plugin Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-player-sessions/releases/).
2. Move the "PlayerSessions" folder to the `/addons/counterstrikesharp/plugins/` directory of your gameserver.
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
  "joinmessage_enable": true,
  "partmessage_enable": true,
  "welcomemessage_enable": true,
  "welcomemessage_delay": 5,
  "enable_city_lookup": false,
  "enable_country_lookup": true,
  "geolite2": "GeoLite2-City.mmdb",
  "player": {
    "[U:X:XXXXXXXX]": {
      "username": "TEST",
      "clantag": "TEST",
      "city": "Example City",
      "country": "Example Country",
      "last_ip": "127.0.0.1",
      "connection_count": 1,
      "connection_last_connected": 1739982463,
      "connection_last_disconnected": 1739982512,
      "playtime_total": 49
    }
  },
  "ConfigVersion": 1
}
```

You can either disable or enable the complete PlayerSessions Plugin by simply setting the *enable* boolean to *false* or *true*.

### debug

Shows debug messages useful when developing for this plugin.

### joinmessage_enable

Whether the join message is sent to all players when someone enters your server.

### partmessage_enable

Whether the part message is sent to all players when someone leaves your server.

### welcomemessage_enable

Whether the player will get a welcome message after he joined the server.

### welcomemessage_delay

The delay before this message gets shown.

### enable_city_lookup

When the geolite2 city database was added, should the city be displayed on join?

### enable_country_lookup

When the geolite2 city database was added, should the country be displayed on join?

### geolite2

The file name of the MaxMind GeoLite2 city mmdb file. Needs to be downloaded manually from the MaxMind website (free account necessary).

### player

All data about all players. Make sure to delete or change data only when user is not on the server. Otherwise user actions will overwrite it.

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
