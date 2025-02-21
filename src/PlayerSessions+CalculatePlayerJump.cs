using CounterStrikeSharp.API.Core;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private void CalculatePlayerJump(EventPlayerJump @event, GameEventInfo info)
        {
            // only calculate if we have a round start (to avoid errors on hot-reload)
            if (!_isDuringRound) return;
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || !_playerConfigs.ContainsKey(player.NetworkIDString)) return;
            // jump challenge ahead
            CheckChallengeGoal(player, "jump", []);
        }
    }
}
