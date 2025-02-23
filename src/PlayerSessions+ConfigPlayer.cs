using System.Text.Json.Serialization;

namespace PlayerSessions
{
    public class PlayerConfigSettingsStatistics
    {
        [JsonPropertyName("show_always")] public bool ShowAlways { get; set; } = false;
    }

    public class PlayerConfigSettingsChallenges
    {
        [JsonPropertyName("show_always")] public bool ShowAlways { get; set; } = true;
    }

    public class PlayerConfigSettings
    {
        [JsonPropertyName("statistics")] public PlayerConfigSettingsStatistics Statistics { get; set; } = new();
        [JsonPropertyName("challenges")] public PlayerConfigSettingsChallenges Challenges { get; set; } = new();
    }

    public class PlayerConfigChallenges
    {
        [JsonPropertyName("schedule_key")] public string ScheduleKey { get; set; } = "";
        [JsonPropertyName("amount")] public int Amount { get; set; } = 0;
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
        [JsonPropertyName("weapon_kills")] public Dictionary<string, PlayerConfigWeaponKills> WeaponKills { get; set; } = [];
        [JsonPropertyName("weapon_deaths")] public Dictionary<string, PlayerConfigWeaponDeaths> WeaponDeaths { get; set; } = [];
        [JsonPropertyName("challenges")] public Dictionary<string, PlayerConfigChallenges> Challenges { get; set; } = [];
        [JsonPropertyName("settings")] public PlayerConfigSettings Settings { get; set; } = new();
    }
}
