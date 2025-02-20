using CounterStrikeSharp.API.Core;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private void CalculateWeaponKillsPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            // only calculate if we have a round start (to avoid errors on hot-reload)
            if (!_isDuringRound) return;
            CCSPlayerController? attacker = @event.Attacker;
            CCSPlayerController? victim = @event.Userid;
            if (attacker == null
                || !attacker.IsValid
                || attacker.IsBot
                || victim == null
                || !victim.IsValid
                || !_playerConfigs.ContainsKey(attacker.NetworkIDString)) return;
            // check if we have an assister and add kill assists
            CCSPlayerController? assister = @event.Assister;
            if (assister != null
                && assister.IsValid
                && !assister.IsBot
                && _playerConfigs.ContainsKey(assister.NetworkIDString))
            {
                _playerConfigs[assister.NetworkIDString].KillAssists++;
                // update ranking points
                UpdateRankingPoints(assister, Config.RankingPointsPerKillAssist, new Dictionary<string, string>
                {
                    { "translation", "assist" },
                    { "player", attacker.PlayerName }
                });
            }
            // increase kill counter
            _playerConfigs[attacker.NetworkIDString].Kills++;
            // update player list
            _playerList[attacker.NetworkIDString].Kills++;
            // update ranking points
            UpdateRankingPoints(attacker, Config.RankingPointsPerKill, new Dictionary<string, string>
            {
                { "translation", "kill" },
                { "player", victim.PlayerName }
            });
            // add weapon to list if not added already
            if (!_playerConfigs[attacker.NetworkIDString].WeaponKills.ContainsKey(@event.Weapon))
            {
                _playerConfigs[attacker.NetworkIDString].WeaponKills.Add(@event.Weapon,
                new PlayerConfigWeaponKills
                {
                    Kills = 1,
                    AmountInAir = @event.Attackerinair ? 1 : 0,
                    AmountBlind = @event.Attackerblind ? 1 : 0,
                    AmountSmoke = @event.Thrusmoke ? 1 : 0,
                    AmountHeadshots = @event.Headshot ? 1 : 0,
                    AmountNoscope = @event.Noscope ? 1 : 0,
                    AmountDominations = @event.Dominated > 0 ? 1 : 0,
                    AmountPenetrations = @event.Penetrated > 0 ? 1 : 0,
                    AmountRevenges = @event.Revenge > 0 ? 1 : 0,
                    LargestDistance = @event.Distance
                });
            }
            else
            {
                _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].Kills++;
                if (@event.Attackerinair) _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].AmountInAir++;
                if (@event.Attackerblind) _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].AmountBlind++;
                if (@event.Thrusmoke) _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].AmountSmoke++;
                if (@event.Headshot) _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].AmountHeadshots++;
                if (@event.Noscope) _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].AmountNoscope++;
                if (@event.Dominated > 0) _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].AmountDominations++;
                if (@event.Penetrated > 0) _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].AmountPenetrations++;
                if (@event.Revenge > 0) _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].AmountRevenges++;
                if (@event.Distance > _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].LargestDistance)
                    _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].LargestDistance = @event.Distance;
            }
        }
    }
}
