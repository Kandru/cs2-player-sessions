using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        [ConsoleCommand("stats", "toggle your stats")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 0, usage: "!stats")]
        public void CommandGiveDice(CCSPlayerController player, CommandInfo command)
        {
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null) return;
            if (player.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
                if (_playerHudPersonalStatistics.ContainsKey(player.NetworkIDString))
                {
                    command.ReplyToCommand(Localizer["command.hidepersonalstatistics"]);
                    HidePersonalStatistics(player);
                }
                else
                {
                    command.ReplyToCommand(Localizer["command.showpersonalstatistics"]);
                    ShowPersonalStatistics(player, 0);
                }
            else
                command.ReplyToCommand(Localizer["command.notalive"]);
        }
    }
}
