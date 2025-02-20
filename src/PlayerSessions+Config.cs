using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlayerSessions
{
    public class PlayerList
    {
        [JsonPropertyName("username")] public string Username { get; set; } = "";
        [JsonPropertyName("clantag")] public string ClanTag { get; set; } = "";
        [JsonPropertyName("kills")] public long Kills { get; set; } = 0;
        [JsonPropertyName("deaths")] public long Deaths { get; set; } = 0;
        [JsonPropertyName("ranking_points")] public long RankingPoints { get; set; } = 0;
    }

    public class PlayerConfigWeaponKills
    {
        [JsonPropertyName("kills")] public long Kills { get; set; } = 0;
        [JsonPropertyName("inair_counter")] public long AmountInAir { get; set; } = 0;
        [JsonPropertyName("blind_counter")] public long AmountBlind { get; set; } = 0;
        [JsonPropertyName("smoke_counter")] public long AmountSmoke { get; set; } = 0;
        [JsonPropertyName("headshot_counter")] public long AmountHeadshots { get; set; } = 0;
        [JsonPropertyName("noscope_counter")] public long AmountNoscope { get; set; } = 0;
        [JsonPropertyName("domination_counter")] public long AmountDominations { get; set; } = 0;
        [JsonPropertyName("penetration_counter")] public long AmountPenetrations { get; set; } = 0;
        [JsonPropertyName("revenge_counter")] public long AmountRevenges { get; set; } = 0;
        [JsonPropertyName("largest_distance")] public float LargestDistance { get; set; } = 0.0f;
    }

    public class PlayerConfigWeaponDeaths
    {
        [JsonPropertyName("deaths")] public long Deaths { get; set; } = 0;
        [JsonPropertyName("attacker_inair_counter")] public long AttackerAmountInAir { get; set; } = 0;
        [JsonPropertyName("attacker_blind_counter")] public long AttackerAmountBlind { get; set; } = 0;
        [JsonPropertyName("attacker_smoke_counter")] public long AttackerAmountSmoke { get; set; } = 0;
        [JsonPropertyName("attacker_headshot_counter")] public long AttackerAmountHeadshots { get; set; } = 0;
        [JsonPropertyName("attacker_noscope_counter")] public long AttackerAmountNoscope { get; set; } = 0;
        [JsonPropertyName("attacker_domination_counter")] public long AttackerAmountDominations { get; set; } = 0;
        [JsonPropertyName("attacker_penetration_counter")] public long AttackerAmountPenetrations { get; set; } = 0;
        [JsonPropertyName("attacker_revenge_counter")] public long AttackerAmountRevenges { get; set; } = 0;
        [JsonPropertyName("largest_distance")] public float LargestDistance { get; set; } = 0.0f;
    }

    public class PlayerConfig
    {
        [JsonPropertyName("username")] public string Username { get; set; } = "";
        [JsonPropertyName("clantag")] public string ClanTag { get; set; } = "";
        [JsonPropertyName("city")] public string City { get; set; } = "";
        [JsonPropertyName("country")] public string Country { get; set; } = "";
        [JsonPropertyName("last_ip")] public string LastIp { get; set; } = "";
        [JsonPropertyName("connection_count")] public long ConnectionCount { get; set; } = 0;
        [JsonPropertyName("connection_last_connected")] public long LastConnected { get; set; } = 0;
        [JsonPropertyName("connection_last_disconnected")] public long LastDisconnected { get; set; } = 0;
        [JsonPropertyName("playtime_total")] public long PlaytimeTotal { get; set; } = 0;
        [JsonPropertyName("playtime_spectator")] public long PlaytimeSpectator { get; set; } = 0;
        [JsonPropertyName("playtime_t")] public long PlaytimeT { get; set; } = 0;
        [JsonPropertyName("playtime_t_alive")] public long PlaytimeTAlive { get; set; } = 0;
        [JsonPropertyName("playtime_ct")] public long PlaytimeCT { get; set; } = 0;
        [JsonPropertyName("playtime_ct_alive")] public long PlaytimeCTAlive { get; set; } = 0;
        [JsonPropertyName("player_kills")] public long Kills { get; set; } = 0;
        [JsonPropertyName("player_kill_assists")] public long KillAssists { get; set; } = 0;
        [JsonPropertyName("player_deaths")] public long Deaths { get; set; } = 0;
        [JsonPropertyName("weapon_kills")] public Dictionary<string, PlayerConfigWeaponKills> WeaponKills { get; set; } = new Dictionary<string, PlayerConfigWeaponKills>();
        [JsonPropertyName("weapon_deaths")] public Dictionary<string, PlayerConfigWeaponDeaths> WeaponDeaths { get; set; } = new Dictionary<string, PlayerConfigWeaponDeaths>();
    }

    public class PluginConfig : BasePluginConfig
    {
        // disabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // debug prints
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
        // statistics
        [JsonPropertyName("show_personal_statistic_on_round_start")] public bool ShowPersonalStatisticsOnRoundStart { get; set; } = true;
        [JsonPropertyName("personal_statistic_on_round_start_duration")] public int PersonalStatisticsOnRoundStartDuration { get; set; } = 3; // buy time + this value
        [JsonPropertyName("personal_statistic_menu_font_size")] public int PersonalStatisticFontSize { get; set; } = 28;
        [JsonPropertyName("personal_statistic_menu_font_name")] public string PersonalStatisticFontName { get; set; } = "Impact";
        [JsonPropertyName("personal_statistic_menu_font_color")] public string PersonalStatisticFontColor { get; set; } = "#ffffff";
        [JsonPropertyName("personal_statistic_menu_pos_x")] public float PersonalStatisticPositionX { get; set; } = -7.5f; // for 16:9 & 16:10
        [JsonPropertyName("personal_statistic_menu_pos_y")] public float PersonalStatisticPositionY { get; set; } = 0f;
        [JsonPropertyName("personal_statistic_menu_background")] public bool PersonalStatisticBackground { get; set; } = true;
        // join / part message
        [JsonPropertyName("joinmessage_enable")] public bool JoinMessageEnable { get; set; } = true;
        [JsonPropertyName("partmessage_enable")] public bool PartMessageEnable { get; set; } = true;
        // welcome messages
        [JsonPropertyName("welcomemessage_enable")] public bool WelcomeMessageEnable { get; set; } = true;
        [JsonPropertyName("welcomemessage_delay")] public float WelcomeMesageDelay { get; set; } = 5.0f;
        // disable city (for privacy reasons)
        [JsonPropertyName("enable_city_lookup")] public bool EnableCityLookup { get; set; } = true;
        [JsonPropertyName("enable_country_lookup")] public bool EnableCountryLookup { get; set; } = true;
        // Geolite2 Country or City binary file (.mmdb)
        [JsonPropertyName("geolite2")] public string Geolite2 { get; set; } = "GeoLite2-City.mmdb";
        // ranking points configuration
        [JsonPropertyName("ranking_points_per_kill")] public int RankingPointsPerKill { get; set; } = 2;
        [JsonPropertyName("ranking_points_per_kill_assist")] public int RankingPointsPerKillAssist { get; set; } = 1;
        [JsonPropertyName("ranking_points_per_death")] public int RankingPointsPerDeath { get; set; } = -1;
    }

    public partial class PlayerSessions : BasePlugin, IPluginConfig<PluginConfig>
    {
        public required PluginConfig Config { get; set; }
        private Dictionary<string, PlayerConfig> _playerConfigs = [];
        private Dictionary<string, PlayerList> _playerList = [];

        private PlayerConfig LoadPlayerConfig(string steamId)
        {
            // check if player config does not exist
            if (!_playerConfigs.ContainsKey(steamId))
            {
                string safeSteamId = string.Concat(steamId.Split(Path.GetInvalidFileNameChars()));
                string playerConfigPath = Path.Combine(Path.GetDirectoryName(Config.GetConfigPath()) ?? "./", $"{safeSteamId}.json");
                if (!Path.Exists(playerConfigPath))
                {
                    _playerConfigs.Add(steamId, new PlayerConfig());
                }
                else
                {
                    var jsonString = File.ReadAllText(playerConfigPath);
                    var playerConfig = JsonSerializer.Deserialize<PlayerConfig>(jsonString);
                    if (playerConfig != null)
                    {
                        _playerConfigs.Add(steamId, playerConfig);
                    }
                }
            }
            // check if player list entry does not exist
            if (!_playerList.ContainsKey(steamId))
            {
                _playerList.Add(steamId, new PlayerList());
            }
            return _playerConfigs[steamId];
        }

        private void LoadActivePlayerConfigs()
        {
            foreach (CCSPlayerController entry in Utilities.GetPlayers())
            {
                if (entry == null
                    || !entry.IsValid
                    || entry.IsBot
                    || _playerConfigs.ContainsKey(entry.NetworkIDString)) return;
                LoadPlayerConfig(entry.NetworkIDString);
                // set data
                _playerConfigs[entry.NetworkIDString].LastConnected = GetCurrentTimestamp();
            }
        }

        private void SavePlayerConfig(string steamId)
        {
            if (!_playerConfigs.ContainsKey(steamId)) return;
            string safeSteamId = string.Concat(steamId.Split(Path.GetInvalidFileNameChars()));
            string playerConfigPath = Path.Combine(Path.GetDirectoryName(Config.GetConfigPath()) ?? "./", $"{safeSteamId}.json");
            DebugPrint($"Saving player config for {steamId} to {playerConfigPath}");
            var jsonString = JsonSerializer.Serialize(_playerConfigs[steamId], new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(playerConfigPath, jsonString);
        }

        private void SavePlayerConfigs()
        {
            foreach (var kvp in _playerConfigs)
            {
                SavePlayerConfig(kvp.Key);
            }
        }

        private void LoadPlayerlist()
        {
            string playerlistPath = Path.Combine(Path.GetDirectoryName(Config.GetConfigPath()) ?? "./", "playerlist.json");
            DebugPrint($"Loading player list");
            if (Path.Exists(playerlistPath))
            {
                var jsonString = File.ReadAllText(playerlistPath);
                _playerList = JsonSerializer.Deserialize<Dictionary<string, PlayerList>>(jsonString) ?? new Dictionary<string, PlayerList>();
            }
        }

        private void SavePlayerList()
        {
            string playerlistPath = Path.Combine(Path.GetDirectoryName(Config.GetConfigPath()) ?? "./", "playerlist.json");
            DebugPrint($"Saving player list");
            var jsonString = JsonSerializer.Serialize(_playerList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(playerlistPath, jsonString);
        }

        private void PlayerConfigsGarbageCollection()
        {
            DebugPrint("Starting Garbage Collection for Player Configs");
            var players = Utilities.GetPlayers();
            var playerIds = players.Select(p => p.NetworkIDString).ToHashSet();
            var keysToRemove = _playerConfigs.Keys.Where(key => !playerIds.Contains(key)).ToList();
            foreach (var key in keysToRemove)
            {
                DebugPrint($"gargarbage collecting player config for {key}");
                SavePlayerConfig(key);
                _playerConfigs.Remove(key);
            }
        }

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            Console.WriteLine(Localizer["core.config"]);
        }
    }
}
