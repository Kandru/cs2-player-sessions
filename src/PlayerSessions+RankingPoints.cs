using CounterStrikeSharp.API.Core;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private void UpdateRankingPoints(CCSPlayerController player, int points, Dictionary<string, string>? data = null)
        {
            if (player == null
                || !player.IsValid
                || player.IsBot) return;
            data ??= [];
            if (!_playerList.ContainsKey(player.NetworkIDString)) return;
            _playerList[player.NetworkIDString].RankingPoints += points;
            if (data.ContainsKey("translation"))
            {
                string pointsString = points > 0 ? $"+{points:N0}" : points.ToString("N0");
                string message = Localizer[$"rankingpoints.added.{data["translation"]}"].Value.Replace("{points}", pointsString);
                foreach (var kvp in data)
                {
                    if (kvp.Key == "translation") continue;
                    message = message.Replace($"{{{kvp.Key}}}", kvp.Value);
                }
                player.PrintToChat(message);
            }
        }
    }
}
