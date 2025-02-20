using CounterStrikeSharp.API.Core;

namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        private void CalculateWeaponDeathsPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            // only calculate if we have a round start (to avoid errors on hot-reload)
            if (!_isDuringRound) return;
            CCSPlayerController? victim = @event.Userid;
            if (victim == null
                || !victim.IsValid
                || victim.IsBot
                || !_playerConfigs.ContainsKey(victim.NetworkIDString)) return;
            // increase death counter
            _playerConfigs[victim.NetworkIDString].Deaths++;
            // add weapon to list if not added already
            if (!_playerConfigs[victim.NetworkIDString].WeaponDeaths.ContainsKey(@event.Weapon))
            {
                _playerConfigs[victim.NetworkIDString].WeaponDeaths.Add(@event.Weapon,
                new PlayerConfigWeaponDeaths
                {
                    Deaths = 1,
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
                _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].Deaths++;
                if (@event.Attackerinair) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AmountInAir++;
                if (@event.Attackerblind) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AmountBlind++;
                if (@event.Thrusmoke) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AmountSmoke++;
                if (@event.Headshot) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AmountHeadshots++;
                if (@event.Noscope) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AmountNoscope++;
                if (@event.Dominated > 0) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AmountDominations++;
                if (@event.Penetrated > 0) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AmountPenetrations++;
                if (@event.Revenge > 0) _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].AmountRevenges++;
                if (@event.Distance > _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].LargestDistance)
                    _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].LargestDistance = @event.Distance;
            }
        }
    }
}
