using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlayerSessions
{
    public class PluginConfigStatistics
    {
        [JsonPropertyName("show_on_round_start")] public bool ShowOnRoundStart { get; set; } = true;
        [JsonPropertyName("on_round_start_duration")] public int OnRoundStartDuration { get; set; } = 3; // buy time + this value
        [JsonPropertyName("menu_font_size")] public int FontSize { get; set; } = 28;
        [JsonPropertyName("menu_font_name")] public string FontName { get; set; } = "Arial Black Standard";
        [JsonPropertyName("menu_font_color")] public string FontColor { get; set; } = "#ffffff";
        [JsonPropertyName("menu_pos_x")] public float PositionX { get; set; } = -7.5f; // for 16:9 & 16:10
        [JsonPropertyName("menu_pos_y")] public float PositionY { get; set; } = 0f; // for 16:9 & 16:10
        [JsonPropertyName("menu_background")] public bool Background { get; set; } = true;
        [JsonPropertyName("menu_backgroundfactor")] public float BackgroundFactor { get; set; } = 1f;
    }

    public class PluginConfigChallenges
    {
        [JsonPropertyName("show_on_round_start")] public bool ShowOnRoundStart { get; set; } = true;
        [JsonPropertyName("on_round_start_duration")] public int OnRoundStartDuration { get; set; } = 3; // buy time + this value
        [JsonPropertyName("on_update_duration")] public float OnUpdateDuration { get; set; } = 5f;
        [JsonPropertyName("menu_display_maximum")] public int DisplayMaximum { get; set; } = 4;
        [JsonPropertyName("menu_font_size")] public int FontSize { get; set; } = 28;
        [JsonPropertyName("menu_font_name")] public string FontName { get; set; } = "Arial Black Standard";
        [JsonPropertyName("menu_font_color")] public string FontColor { get; set; } = "#ffffff";
        [JsonPropertyName("menu_pos_x")] public float PositionX { get; set; } = 3.6f; // for 16:9 & 16:10
        [JsonPropertyName("menu_pos_y")] public float PositionY { get; set; } = 4f; // for 16:9 & 16:10
        [JsonPropertyName("menu_background")] public bool Background { get; set; } = true;
        [JsonPropertyName("menu_backgroundfactor")] public float BackgroundFactor { get; set; } = 1f;
    }

    public class PluginConfig : BasePluginConfig
    {
        // disabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // debug prints
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
        // statistics
        [JsonPropertyName("statistics")] public PluginConfigStatistics Statistics { get; set; } = new();
        // challenges
        [JsonPropertyName("challenges")] public PluginConfigChallenges Challenges { get; set; } = new();
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
        private ChallengesConfig _playerChallenges = new();

        private PlayerConfig LoadPlayerConfig(string steamId)
        {
            // check if player config does not exist
            if (!_playerConfigs.ContainsKey(steamId))
            {
                string safeSteamId = string.Concat(steamId.Split(Path.GetInvalidFileNameChars()));
                string playerConfigPath = Path.Combine(
                    $"{Path.GetDirectoryName(Config.GetConfigPath())}/players/" ?? "./players/", $"{safeSteamId}.json"
                );
                if (!Path.Exists(playerConfigPath))
                {
                    _playerConfigs.Add(steamId, new PlayerConfig());
                }
                else
                {
                    try
                    {
                        var jsonString = File.ReadAllText(playerConfigPath);
                        var playerConfig = JsonSerializer.Deserialize<PlayerConfig>(jsonString);
                        if (playerConfig != null)
                        {
                            _playerConfigs.Add(steamId, playerConfig);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(Localizer["core.faultyconfig"].Value.Replace("{config}", playerConfigPath).Replace("{error}", e.Message));
                        // save backup of faulty config to prevent data loss
                        if (Path.Exists(playerConfigPath))
                        {
                            File.Copy(playerConfigPath, playerConfigPath + ".bak", true);
                        }
                        _playerConfigs.Add(steamId, new PlayerConfig());
                    }
                }
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
                _playerConfigs[entry.NetworkIDString].LastConnected = GetUnixTimestamp();
            }
        }

        private void SavePlayerConfig(string steamId)
        {
            if (!_playerConfigs.ContainsKey(steamId)) return;
            string safeSteamId = string.Concat(steamId.Split(Path.GetInvalidFileNameChars()));
            string playerConfigPath = Path.Combine(
                    $"{Path.GetDirectoryName(Config.GetConfigPath())}/players/" ?? "./players/", $"{safeSteamId}.json"
                );
            // check if folder exists and create otherwise
            if (!Path.Exists(Path.GetDirectoryName(playerConfigPath)))
            {
                var directoryPath = Path.GetDirectoryName(playerConfigPath);
                if (directoryPath != null)
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
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

        private void LoadChallenges()
        {
            string challengesPath = Path.Combine(Path.GetDirectoryName(Config.GetConfigPath()) ?? "./", "challenges.json");
            DebugPrint($"Loading challenges");
            if (Path.Exists(challengesPath))
            {
                try
                {
                    var jsonString = File.ReadAllText(challengesPath);
                    _playerChallenges = JsonSerializer.Deserialize<ChallengesConfig>(jsonString) ?? new();
                }
                catch
                {
                    Console.WriteLine(Localizer["core.faultyconfig"].Value.Replace("{config}", challengesPath));
                }
            }
            else
            {
                SaveChallenges();
            }
        }

        private void SaveChallenges()
        {
            string challengesPath = Path.Combine(Path.GetDirectoryName(Config.GetConfigPath()) ?? "./", "challenges.json");
            DebugPrint($"Saving challenges");
            var jsonString = JsonSerializer.Serialize(_playerChallenges, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(challengesPath, jsonString);
        }

        private void LoadPlayerlist()
        {
            string playerlistPath = Path.Combine(Path.GetDirectoryName(Config.GetConfigPath()) ?? "./", "playerlist.json");
            DebugPrint($"Loading player list");
            if (Path.Exists(playerlistPath))
            {
                try
                {
                    var jsonString = File.ReadAllText(playerlistPath);
                    _playerList = JsonSerializer.Deserialize<Dictionary<string, PlayerList>>(jsonString) ?? new Dictionary<string, PlayerList>();
                }
                catch
                {
                    Console.WriteLine(Localizer["core.faultyconfig"].Value.Replace("{config}", playerlistPath));
                }
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
