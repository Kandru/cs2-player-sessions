using CounterStrikeSharp.API.Core;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private void CalculatePlayerBlind(EventPlayerBlind @event, GameEventInfo info)
        {
            // only calculate if we have a round start (to avoid errors on hot-reload)
            if (!_isDuringRound) return;
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
                { "attacker", attacker.PlayerName },
                { "attacker_isbot", attacker.IsBot.ToString() },
                { "victim", victim.PlayerName },
                { "victim_isbot", victim.IsBot.ToString() },
                { "blindduration", @event.BlindDuration.ToString() }
            });
        }
    }
}
