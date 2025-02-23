﻿using System.Text.Json.Serialization;

namespace PlayerSessions
{
    public class ChallengesSchedule
    {
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("date_start")] public string StartDate { get; set; } = "2025-01-01 00:00:00";
        [JsonPropertyName("date_end")] public string EndDate { get; set; } = "2025-02-01 00:00:00";
        [JsonPropertyName("challenges")] public List<string> Challenges { get; set; } = [];
    }

    public class ChallengesBlueprintRules
    {
        [JsonPropertyName("key")] public string Key { get; set; } = "";
        [JsonPropertyName("operator")] public string Operator { get; set; } = "";
        [JsonPropertyName("value")] public string Value { get; set; } = "";
    }

    public class ChallengesBlueprint
    {
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("type")] public string Type { get; set; } = "";
        [JsonPropertyName("points")] public int Points { get; set; } = 0;
        [JsonPropertyName("amount")] public int Amount { get; set; } = 0;
        [JsonPropertyName("rules")] public List<ChallengesBlueprintRules> Rules { get; set; } = [];
    }

    public class ChallengesConfig
    {
        [JsonPropertyName("schedule")] public Dictionary<string, ChallengesSchedule> Schedule { get; set; } = [];
        [JsonPropertyName("blueprints")] public Dictionary<string, ChallengesBlueprint> Blueprints { get; set; } = [];
    }
}
