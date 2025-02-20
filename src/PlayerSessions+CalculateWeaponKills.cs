using CounterStrikeSharp.API.Core;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private void CalculateWeaponKillsPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            // only calculate if we have a round start (to avoid errors on hot-reload)
            if (!_isDuringRound) return;
            CCSPlayerController? attacker = @event.Userid;
            if (attacker == null
                || !attacker.IsValid
                || attacker.IsBot
                || !_playerConfigs.ContainsKey(attacker.NetworkIDString)) return;
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
                if (@event.Distance > _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].LargestDistance)
                    _playerConfigs[attacker.NetworkIDString].WeaponKills[@event.Weapon].LargestDistance = @event.Distance;
            }
        }
    }
}
