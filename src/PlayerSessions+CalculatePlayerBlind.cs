using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private void CalculatePlayerBlind(EventPlayerBlind @event, GameEventInfo info)
        {
            CCSPlayerController? attacker = @event.Attacker;
            CCSPlayerController? victim = @event.Userid;
            if (attacker == null
                || !attacker.IsValid
                || attacker.IsBot
                || !_playerConfigs.ContainsKey(attacker.NetworkIDString)
                || victim == null
                || !victim.IsValid) return;
            // check for challenge
            CheckChallengeGoal(attacker, "blind", new Dictionary<string, string>
            {
                { "isduringround", _isDuringRound.ToString() },
                { "isteamflash", (attacker.TeamNum == victim.TeamNum).ToString() },
                { "isselfflash", (attacker == victim).ToString() },
                { "attacker", attacker.PlayerName },
                { "attacker_isbot", attacker.IsBot.ToString() },
                { "attacker_team", attacker.Team.ToString() },
                { "victim", victim.PlayerName },
                { "victim_isbot", victim.IsBot.ToString() },
                { "victim_team", victim.Team.ToString() },
                { "blindduration", @event.BlindDuration.ToString() }
            });
        }
    }
}
