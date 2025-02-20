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
            if (!_isDuringRound || !Config.SpawnMessageEnable) return;
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
                        duration: freezeTime
                    );
                });
            }
        }

        private void ShowPersonalStatistics(CCSPlayerController player, float duration = 10.0f)
        {
            if (player == null
                || !player.IsValid
                || !_playerConfigs.ContainsKey(player.NetworkIDString)) return;
            if (_playerHudPersonalStatistics.ContainsKey(player.NetworkIDString))
            {
                // do not kill if entity is no longer valid
                if (_playerHudPersonalStatistics[player.NetworkIDString] != null
                    && _playerHudPersonalStatistics[player.NetworkIDString].IsValid)
                    _playerHudPersonalStatistics[player.NetworkIDString].AcceptInput("kill");
                // remove hud from list
                _playerHudPersonalStatistics.Remove(player.NetworkIDString);
            }
            // build statistic message
            string message = Localizer["statistics.personal.title"].Value;
            message += "\n" + Localizer["statistics.personal.kills"].Value
                .Replace("{kills}", _playerConfigs[player.NetworkIDString].Kills.ToString());
            message += "\n" + Localizer["statistics.personal.deaths"].Value
                .Replace("{deaths}", _playerConfigs[player.NetworkIDString].Deaths.ToString());
            message += "\n" + Localizer["statistics.personal.kd"].Value
                .Replace("{kd}", _playerConfigs[player.NetworkIDString].Kills == 0
                    ? "0.00"
                    : string.Format("{0:0.00}", (float)_playerConfigs[player.NetworkIDString].Kills / (float)_playerConfigs[player.NetworkIDString].Deaths));
            message += "\n" + Localizer["statistics.personal.assists"].Value
                .Replace("{assists}", _playerConfigs[player.NetworkIDString].KillAssists.ToString());
            // find the weapon with the most kills
            var topWeapon = _playerConfigs[player.NetworkIDString].WeaponKills
                .Where(w => w.Key != "world")
                .OrderByDescending(w => w.Value.Kills)
                .FirstOrDefault();
            if (topWeapon.Key != null)
            {
                message += "\n" + Localizer["statistics.personal.topweapon"].Value
                    .Replace("{weapon}", topWeapon.Key.ToUpper())
                    .Replace("{kills}", topWeapon.Value.Kills.ToString())
                    .Replace("{headshots}", topWeapon.Value.AmountHeadshots.ToString())
                    .Replace("{distance}", string.Format("{0:0.00}", topWeapon.Value.LargestDistance));
            }
            // set background height dynamically
            float backgroundHeight = 0.05f * message.Split('\n').Length;
            if (backgroundHeight == 0) backgroundHeight = 0.05f;
            // set background width dynamically by counting the longest line
            float backgroundWidth = 0.02f;
            foreach (string line in message.Split('\n'))
            {
                if (backgroundWidth < line.Length * 0.02f) backgroundWidth = line.Length * 0.02f;
            }
            // create hud
            CPointWorldText? hudText = WorldTextManager.Create(
                    player,
                    message,
                    35,
                    Color.Orange,
                    "Verdana Bold",
                    -(backgroundWidth * 4.5f),
                    0f,
                    true,
                    backgroundHeight,
                    backgroundWidth
                );
            if (hudText == null) return;
            _playerHudPersonalStatistics.Add(player.NetworkIDString, hudText);
            // remove hud after duration
            AddTimer(duration, () =>
            {
                // return if player is no longer valid
                if (player == null
                    || !player.IsValid
                    || !_playerHudPersonalStatistics.ContainsKey(player.NetworkIDString)) return;
                // do not kill if entity is no longer valid
                if (_playerHudPersonalStatistics[player.NetworkIDString] != null
                    && _playerHudPersonalStatistics[player.NetworkIDString].IsValid)
                    _playerHudPersonalStatistics[player.NetworkIDString].AcceptInput("kill");
                // remove hud from list
                _playerHudPersonalStatistics.Remove(player.NetworkIDString);
            });
        }
    }
}
