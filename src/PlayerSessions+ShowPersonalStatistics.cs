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
            if (!_isDuringRound || !Config.Statistics.ShowOnRoundStart) return;
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
                    // check for user preferences
                    float duration = _playerConfigs[player.NetworkIDString].Settings.Statistics.ShowAlways
                        ? 0
                        : freezeTime + Config.Statistics.OnRoundStartDuration;
                    // show GUI
                    ShowPersonalStatisticsGUI(player, duration);
                });
            }
        }

        private void ShowPersonalStatisticsOnSpawn(CCSPlayerController player)
        {
            if (!Config.Statistics.ShowAfterRespawn) return;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || !_playerConfigs.ContainsKey(player.NetworkIDString)
                || (player.TeamNum != (int)CsTeam.CounterTerrorist && player.TeamNum != (int)CsTeam.Terrorist)) return;
            AddTimer(0.1f, () =>
            {
                // check for user preferences
                float duration = _playerConfigs[player.NetworkIDString].Settings.Statistics.ShowAlways
                    ? 0
                    : Config.Statistics.AfterRespawnDuration;
                // show GUI
                ShowPersonalStatisticsGUI(player, duration);
            });
        }

        private void ShowPersonalStatisticsGUI(CCSPlayerController player, float duration = 10.0f)
        {
            if (player == null
                || !player.IsValid
                || !_playerConfigs.ContainsKey(player.NetworkIDString)
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || player.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE) return;
            // build statistic message
            string message = Localizer["statistics.personal.title"].Value;
            // check if player got a rank
            if (_playerList.ContainsKey(player.NetworkIDString))
            {
                var playerRankings = _playerList.Values.OrderByDescending(p => p.RankingPoints).ToList();
                message += "\n" + Localizer["statistics.personal.rank"].Value
                    .Replace("{rank}", (playerRankings.FindIndex(p => p == _playerList[player.NetworkIDString]) + 1).ToString())
                    .Replace("{total}", playerRankings.Count().ToString());
            }
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
            // use our entity if it still exists
            if (_playerHudPersonalStatistics.ContainsKey(player.NetworkIDString))
            {
                if (_playerHudPersonalStatistics[player.NetworkIDString] != null
                    && _playerHudPersonalStatistics[player.NetworkIDString].IsValid)
                {
                    UpdatePersonalStatisticsGUI(player, message);
                    return;
                }
                else
                {
                    _playerHudPersonalStatistics.Remove(player.NetworkIDString);
                }
            }
            // create hud
            CPointWorldText? hudText = WorldTextManager.Create(
                    player,
                    message,
                    Config.Statistics.FontSize,
                    ColorTranslator.FromHtml(Config.Statistics.FontColor),
                    Config.Statistics.FontName,
                    Config.Statistics.PositionX,
                    Config.Statistics.PositionY,
                    Config.Statistics.Background,
                    Config.Statistics.BackgroundFactor
                );
            if (hudText == null) return;
            _playerHudPersonalStatistics.Add(player.NetworkIDString, hudText);
            // remove hud after duration
            if (duration > 0)
                AddTimer(duration, () =>
                {
                    HidePersonalStatisticsGUI(player);
                });
        }

        private void TriggerPersonalStatisticsUpdate(CCSPlayerController player)
        {
            if (player == null
                || !player.IsValid
                || !_playerHudPersonalStatistics.ContainsKey(player.NetworkIDString)
                || _playerHudPersonalStatistics[player.NetworkIDString] == null
                || !_playerHudPersonalStatistics[player.NetworkIDString].IsValid) return;
            ShowPersonalStatisticsGUI(player, 0);
        }

        private void UpdatePersonalStatisticsGUI(CCSPlayerController player, string message)
        {
            if (player == null
                || !player.IsValid
                || !_playerHudPersonalStatistics.ContainsKey(player.NetworkIDString)
                || _playerHudPersonalStatistics[player.NetworkIDString] == null
                || !_playerHudPersonalStatistics[player.NetworkIDString].IsValid) return;

            // set new message
            _playerHudPersonalStatistics[player.NetworkIDString].AcceptInput(
                "SetMessage",
                _playerHudPersonalStatistics[player.NetworkIDString],
                _playerHudPersonalStatistics[player.NetworkIDString],
                message
            );
        }

        private void HidePersonalStatisticsGUI(CCSPlayerController player)
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

        private void HideAllPersonalStatisticsGUI()
        {
            foreach (var playerHud in _playerHudPersonalStatistics)
            {
                if (playerHud.Value != null
                    && playerHud.Value.IsValid)
                    playerHud.Value.AcceptInput("kill");
            }
            _playerHudPersonalStatistics.Clear();
        }
    }
}
