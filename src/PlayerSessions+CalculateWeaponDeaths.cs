using CounterStrikeSharp.API.Core;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private void CalculateWeaponDeathsPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            CCSPlayerController? attacker = @event.Attacker;
            CCSPlayerController? victim = @event.Userid;
            if (victim == null
                || !victim.IsValid
                || victim.IsBot
                || attacker == null
                || !attacker.IsValid
                || !_playerConfigs.ContainsKey(victim.NetworkIDString)) return;
            // increase death counter
            _playerConfigs[victim.NetworkIDString].Deaths++;
            // update player list
            _playerList[victim.NetworkIDString].Deaths++;
            // update ranking points
            UpdateRankingPoints(victim, Config.RankingPointsPerDeath, new Dictionary<string, string>
            {
                { "type", "death" },
                { "isduringround", _isDuringRound.ToString() },
                { "isteamkill", (attacker.TeamNum == victim.TeamNum).ToString() },
                { "isselfkill", (attacker == victim).ToString() },
                { "attacker", attacker.PlayerName },
                { "attacker_isbot", attacker.IsBot.ToString() },
                { "attacker_team", attacker.Team.ToString() },
                { "victim", victim.PlayerName },
                { "victim_isbot", victim.IsBot.ToString() },
                { "victim_team", victim.Team.ToString() },
                { "assistedflash", @event.Assistedflash.ToString() },
                { "attackerblind", @event.Attackerblind.ToString() },
                { "attackerinair", @event.Attackerinair.ToString() },
                { "distance", @event.Distance.ToString() },
                { "dmgarmor", @event.DmgArmor.ToString() },
                { "dmghealth", @event.DmgHealth.ToString() },
                { "dominated", @event.Dominated.ToString() },
                { "headshot", @event.Headshot.ToString() },
                { "hitgroup", @event.Hitgroup.ToString() },
                { "noscope", @event.Noscope.ToString() },
                { "penetrated", @event.Penetrated.ToString() },
                { "revenge", @event.Revenge.ToString() },
                { "thrusmoke", @event.Thrusmoke.ToString() },
                { "weapon", @event.Weapon },
                { "weaponitemid", @event.WeaponItemid }
            });
            // add weapon to list if not added already
            if (!_playerConfigs[victim.NetworkIDString].WeaponDeaths.ContainsKey(@event.Weapon))
            {
                _playerConfigs[victim.NetworkIDString].WeaponDeaths.Add(@event.Weapon,
                new PlayerConfigWeaponDeaths
                {
                    Deaths = 1,
                    AttackerAmountInAir = @event.Attackerinair ? 1 : 0,
                    AttackerAmountBlind = @event.Attackerblind ? 1 : 0,
                    AttackerAmountSmoke = @event.Thrusmoke ? 1 : 0,
                    AttackerAmountHeadshots = @event.Headshot ? 1 : 0,
                    AttackerAmountNoscope = @event.Noscope ? 1 : 0,
                    AttackerAmountDominations = @event.Dominated > 0 ? 1 : 0,
                    AttackerAmountPenetrations = @event.Penetrated > 0 ? 1 : 0,
                    AttackerAmountRevenges = @event.Revenge > 0 ? 1 : 0,
                    LargestDistance = @event.Distance
                });
            }
            else
            {
                _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].Deaths++;
                if (@event.Attackerinair) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AttackerAmountInAir++;
                if (@event.Attackerblind) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AttackerAmountBlind++;
                if (@event.Thrusmoke) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AttackerAmountSmoke++;
                if (@event.Headshot) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AttackerAmountHeadshots++;
                if (@event.Noscope) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AttackerAmountNoscope++;
                if (@event.Dominated > 0) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AttackerAmountDominations++;
                if (@event.Penetrated > 0) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AttackerAmountPenetrations++;
                if (@event.Revenge > 0) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AttackerAmountRevenges++;
                if (@event.Distance > _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].LargestDistance)
                    _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].LargestDistance = @event.Distance;
            }
            // trigger live statistics update for player(s)
            TriggerPersonalStatisticsUpdate(attacker);
            TriggerPersonalStatisticsUpdate(victim);
        }
    }
}
