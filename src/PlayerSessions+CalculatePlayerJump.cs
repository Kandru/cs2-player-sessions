using CounterStrikeSharp.API.Core;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private void CalculatePlayerJump(EventPlayerJump @event, GameEventInfo info)
        {
            CCSPlayerController? player = @event.Userid;
            if (player == null
                || !player.IsValid
                || player.IsBot
                || !_playerConfigs.ContainsKey(player.NetworkIDString)) return;
            // jump challenge ahead
            CheckChallengeGoal(player, "jump", new Dictionary<string, string> {
                { "isduringround", _isDuringRound.ToString() }
            });
        }
    }
}
