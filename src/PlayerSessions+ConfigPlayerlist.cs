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
}
