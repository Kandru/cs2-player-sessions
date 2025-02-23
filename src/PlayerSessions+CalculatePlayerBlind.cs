using CounterStrikeSharp.API.Core;

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
                { "attacker", attacker.PlayerName },
                { "attacker_isbot", attacker.IsBot.ToString() },
                { "victim", victim.PlayerName },
                { "victim_isbot", victim.IsBot.ToString() },
                { "blindduration", @event.BlindDuration.ToString() }
            });
        }
    }
}
