using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private long _calculatePlaytimeRoundStart = 0;

        private void CalculatePlaytimeRoundStart()
        {
            _calculatePlaytimeRoundStart = GetCurrentTimestamp();
        }

        private void CalculatePlaytimePlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            // only calculate if we have a round start (to avoid errors on hot-reload)
            if (_calculatePlaytimeRoundStart == 0
                || !_isDuringRound) return;
            CCSPlayerController? victim = @event.Userid;
            if (victim == null
                || !victim.IsValid
                || victim.IsBot
                || !_playerConfigs.ContainsKey(victim.NetworkIDString)) return;
            // get seconds played
            long currentRoundTime = GetCurrentTimestamp() - _calculatePlaytimeRoundStart;
            if (victim.TeamNum == (int)CsTeam.Terrorist)
            {
                // add time to terrorist statistic
                _playerConfigs[victim.NetworkIDString].PlaytimeTAlive += currentRoundTime;
            }
            else if (victim.TeamNum == (int)CsTeam.CounterTerrorist)
            {
                // add time to counterterrorist statistic
                _playerConfigs[victim.NetworkIDString].PlaytimeCTAlive += currentRoundTime;
            }
        }

        private void CalculatePlaytimeRoundEnd()
        {
            // only calculate if we have a round start (to avoid errors on hot-reload)
            if (_calculatePlaytimeRoundStart == 0
                || !_isDuringRound) return;
            // get seconds played
            long currentRoundTime = GetCurrentTimestamp() - _calculatePlaytimeRoundStart;
            // iterate over all players currently on the server
            foreach (CCSPlayerController entry in Utilities.GetPlayers())
            {
                if (entry == null
                    || !entry.IsValid
                    || entry.IsBot
                    || entry.PlayerPawn == null
                    || !entry.PlayerPawn.IsValid
                    || entry.PlayerPawn.Value == null
                    || !_playerConfigs.ContainsKey(entry.NetworkIDString)) continue;
                if (entry.TeamNum == (int)CsTeam.Terrorist)
                {
                    // add time to terrorist statistic
                    _playerConfigs[entry.NetworkIDString].PlaytimeT += currentRoundTime;
                    // if player was alive the whole round add this to his alive stats
                    if (entry.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
                        _playerConfigs[entry.NetworkIDString].PlaytimeTAlive += currentRoundTime;
                }
                else if (entry.TeamNum == (int)CsTeam.CounterTerrorist)
                {
                    // add time to counterterrorist statistic
                    _playerConfigs[entry.NetworkIDString].PlaytimeCT += currentRoundTime;
                    // if player was alive the whole round add this to his alive stats
                    if (entry.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
                        _playerConfigs[entry.NetworkIDString].PlaytimeCTAlive += currentRoundTime;
                }
                else
                {
                    // add time to spectator statistic
                    _playerConfigs[entry.NetworkIDString].PlaytimeSpectator += currentRoundTime;
                }
            }
        }
    }
}
