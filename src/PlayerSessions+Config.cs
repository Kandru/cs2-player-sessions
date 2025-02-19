using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;
using System.Text.Json.Serialization;

namespace PlayerSessions
{
    public class PlayerConfig
    {
        [JsonPropertyName("username")] public string Username { get; set; } = "";
        [JsonPropertyName("clantag")] public string ClanTag { get; set; } = "";
        [JsonPropertyName("city")] public string City { get; set; } = "";
        [JsonPropertyName("country")] public string Country { get; set; } = "";
        [JsonPropertyName("last_ip")] public string LastIp { get; set; } = "";
        [JsonPropertyName("connection_count")] public int ConnectionCount { get; set; } = 0;
        [JsonPropertyName("connection_last_connected")] public long LastConnected { get; set; } = 0;
        [JsonPropertyName("connection_last_disconnected")] public long LastDisconnected { get; set; } = 0;
        [JsonPropertyName("playtime_total")] public long PlaytimeTotal { get; set; } = 0;
        [JsonPropertyName("playtime_spectator")] public long PlaytimeSpectator { get; set; } = 0;
        [JsonPropertyName("playtime_t")] public long PlaytimeT { get; set; } = 0;
        [JsonPropertyName("playtime_t_alive")] public long PlaytimeTAlive { get; set; } = 0;
        [JsonPropertyName("playtime_ct")] public long PlaytimeCT { get; set; } = 0;
        [JsonPropertyName("playtime_ct_alive")] public long PlaytimeCTAlive { get; set; } = 0;
    }

    public class PluginConfig : BasePluginConfig
    {
        // disabled
        [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;
        // debug prints
        [JsonPropertyName("debug")] public bool Debug { get; set; } = false;
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
        // player data
        [JsonPropertyName("player")] public Dictionary<string, PlayerConfig> Player { get; set; } = new Dictionary<string, PlayerConfig>();
    }

    public partial class PlayerSessions : BasePlugin, IPluginConfig<PluginConfig>
    {
        public required PluginConfig Config { get; set; }

        private void CreateUserConfig(string steamId)
        {
            Config.Reload();
            if (!Config.Player.ContainsKey(steamId))
            {
                Config.Player.Add(steamId, new PlayerConfig());
            }
            // save to disk
            Config.Update();
        }

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            Console.WriteLine(Localizer["core.config"]);
        }
    }
}
