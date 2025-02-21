using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private Dictionary<string, CPointWorldText> _playerHudPersonalStatistics = [];

        private void ShowPersonalStatisticsOnRoundStart()
        {
            if (!_isDuringRound || !Config.ShowPersonalStatisticsOnRoundStart) return;
            int freezeTime = 0;
            ConVar? mpFreezeTime = ConVar.Find("mp_freezetime");
            if (mpFreezeTime != null)
            {
                freezeTime = mpFreezeTime.GetPrimitiveValue<int>();
            }
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player == null
                    || !player.IsValid
                    || player.IsBot
                    || !_playerConfigs.ContainsKey(player.NetworkIDString)
                    || (player.TeamNum != (int)CsTeam.CounterTerrorist && player.TeamNum != (int)CsTeam.Terrorist)) continue;
                AddTimer(0.1f, () =>
                {
                    if (player == null
                    || !player.IsValid) return;
                    ShowPersonalStatistics(
                        player,
                        duration: freezeTime + Config.PersonalStatisticsOnRoundStartDuration
                    );
                });
            }
        }

        private void ShowPersonalStatistics(CCSPlayerController player, float duration = 10.0f)
        {
            if (player == null
                || !player.IsValid
                || !_playerConfigs.ContainsKey(player.NetworkIDString)) return;
            // build statistic message
            string message = Localizer["statistics.personal.title"].Value;
            var playerRankings = _playerConfigs.Values.OrderByDescending(p => p.Kills).ToList();
            message += "\n" + Localizer["statistics.personal.rank"].Value
                .Replace("{rank}", (playerRankings.FindIndex(p => p == _playerConfigs[player.NetworkIDString]) + 1).ToString())
                .Replace("{total}", playerRankings.Select(p => p.Kills).Distinct().Count().ToString());
            message += "\n" + Localizer["statistics.personal.kills"].Value
                .Replace("{kills}", _playerConfigs[player.NetworkIDString].Kills.ToString("N0"));
            message += "\n" + Localizer["statistics.personal.deaths"].Value
                .Replace("{deaths}", _playerConfigs[player.NetworkIDString].Deaths.ToString("N0"));
            message += "\n" + Localizer["statistics.personal.kd"].Value
                .Replace("{kd}", _playerConfigs[player.NetworkIDString].Kills == 0
                    ? "0.00"
                    : string.Format("{0:0.00}", (float)_playerConfigs[player.NetworkIDString].Kills / (float)_playerConfigs[player.NetworkIDString].Deaths));
            message += "\n" + Localizer["statistics.personal.assists"].Value
                .Replace("{assists}", _playerConfigs[player.NetworkIDString].KillAssists.ToString("N0"));
            // find the weapon with the most kills
            var topWeapon = _playerConfigs[player.NetworkIDString].WeaponKills
                .Where(w => w.Key != "world")
                .OrderByDescending(w => w.Value.Kills)
                .FirstOrDefault();
            if (topWeapon.Key != null)
            {
                message += "\n" + Localizer["statistics.personal.topweapon"].Value
                    .Replace("{weapon}", topWeapon.Key.Replace("_", " ").ToUpper())
                    .Replace("{kills}", topWeapon.Value.Kills.ToString("N0"))
                    .Replace("{headshots}", topWeapon.Value.AmountHeadshots.ToString("N0"))
                    .Replace("{distance}", string.Format("{0:0.00}", topWeapon.Value.LargestDistance));
            }
            // set background height dynamically
            float backgroundHeight = 0.01f * message.Split('\n').Length;
            if (backgroundHeight == 0) backgroundHeight = 0.05f;
            // set background width dynamically by counting the longest line
            float backgroundWidth = 0.015f;
            foreach (string line in message.Split('\n'))
            {
                if (backgroundWidth < line.Length * 0.012f) backgroundWidth = line.Length * 0.012f;
            }
            // use our entity if it still exists
            if (_playerHudPersonalStatistics.ContainsKey(player.NetworkIDString)
                && _playerHudPersonalStatistics[player.NetworkIDString] != null
                    && _playerHudPersonalStatistics[player.NetworkIDString].IsValid)
            {
                _playerHudPersonalStatistics[player.NetworkIDString].AcceptInput(
                    "SetMessage",
                    _playerHudPersonalStatistics[player.NetworkIDString],
                    _playerHudPersonalStatistics[player.NetworkIDString],
                    message
                );
            }
            else
            {
                // create hud
                CPointWorldText? hudText = WorldTextManager.Create(
                        player,
                        message,
                        Config.PersonalStatisticFontSize,
                        ColorTranslator.FromHtml(Config.PersonalStatisticFontColor),
                        Config.PersonalStatisticFontName,
                        Config.PersonalStatisticPositionX,
                        Config.PersonalStatisticPositionY,
                        Config.PersonalStatisticBackground,
                        backgroundHeight,
                        backgroundWidth
                    );
                if (hudText == null) return;
                _playerHudPersonalStatistics.Add(player.NetworkIDString, hudText);
            }
            // remove hud after duration
            if (duration > 0)
                AddTimer(duration, () =>
                {
                    HidePersonalStatistics(player);
                });
        }

        private void HidePersonalStatistics(CCSPlayerController player)
        {
            if (player == null
                || !player.IsValid
                || !_playerHudPersonalStatistics.ContainsKey(player.NetworkIDString)) return;
            // do not kill if entity is no longer valid
            if (_playerHudPersonalStatistics[player.NetworkIDString] != null
                && _playerHudPersonalStatistics[player.NetworkIDString].IsValid)
                _playerHudPersonalStatistics[player.NetworkIDString].AcceptInput("kill");
            // remove hud from list
            _playerHudPersonalStatistics.Remove(player.NetworkIDString);
        }
    }
}
