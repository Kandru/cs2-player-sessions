using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        [ConsoleCommand("stats", "toggle your stats overview")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 0, usage: "!stats")]
        public void CommandShowStats(CCSPlayerController player, CommandInfo command)
        {
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || !_playerConfigs.ContainsKey(player.NetworkIDString)) return;
            if (player.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
                if (_playerHudPersonalStatistics.ContainsKey(player.NetworkIDString))
                {
                    command.ReplyToCommand(Localizer["command.hidepersonalstatistics"]);
                    HidePersonalStatisticsGUI(player);
                    // save chosen user setting
                    _playerConfigs[player.NetworkIDString].Settings.Statistics.ShowAlways = false;
                }
                else
                {
                    command.ReplyToCommand(Localizer["command.showpersonalstatistics"]);
                    ShowPersonalStatisticsGUI(player, 0);
                    // save chosen user setting
                    _playerConfigs[player.NetworkIDString].Settings.Statistics.ShowAlways = true;
                }
            else
                command.ReplyToCommand(Localizer["command.notalive"]);
        }

        [ConsoleCommand("challenges", "toggle your challenge overview")]
        [ConsoleCommand("c", "toggle your challenge overview")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 0, usage: "!c")]
        public void CommandShowChallenges(CCSPlayerController player, CommandInfo command)
        {
            if (player == null
                || !player.IsValid
                || player.IsBot
                || player.PlayerPawn == null
                || !player.PlayerPawn.IsValid
                || player.PlayerPawn.Value == null
                || !_playerConfigs.ContainsKey(player.NetworkIDString)) return;
            if (_currentChallenge.Challenges.Count == 0)
            {
                command.ReplyToCommand(Localizer["command.nochallenges"]);
                return;
            }
            if (player.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
                if (_playerHudPersonalChallenges.ContainsKey(player.NetworkIDString))
                {
                    command.ReplyToCommand(Localizer["command.hidepersonalchallenges"]);
                    HidePersonalChallengesGUI(player);
                    // save chosen user setting
                    _playerConfigs[player.NetworkIDString].Settings.Challenges.ShowAlways = false;
                }
                else
                {
                    command.ReplyToCommand(Localizer["command.showpersonalchallenges"]);
                    ShowPersonalChallengesGUI(player, 0);
                    // save chosen user setting
                    _playerConfigs[player.NetworkIDString].Settings.Challenges.ShowAlways = true;
                }
            else
                command.ReplyToCommand(Localizer["command.notalive"]);
        }

        [ConsoleCommand("top", "top players")]
        [ConsoleCommand("top5", "top players")]
        [ConsoleCommand("top10", "top players")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY, minArgs: 0, usage: "!top")]
        public void CommandShowTopPlayers(CCSPlayerController player, CommandInfo command)
        {
            // sort player list by ranking points and print top 5
            var topPlayers = _playerList.Values
                .OrderByDescending(p => p.RankingPoints)
                .Take(5)
                .ToList();
            SendGlobalChatMessage(Localizer["command.topplayers.title"]);
            foreach (var topPlayer in topPlayers)
            {
                SendGlobalChatMessage(Localizer["command.topplayers.entry"].Value
                    .Replace("{rank}", (topPlayers.IndexOf(topPlayer) + 1).ToString())
                    .Replace("{name}", topPlayer.Username)
                    .Replace("{points}", topPlayer.RankingPoints.ToString("N0"))
                );
            }
            SendGlobalChatMessage(Localizer["command.topplayers.footer"]);
        }
    }
}
