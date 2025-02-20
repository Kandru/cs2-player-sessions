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
                if (@event.Distance > _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].LargestDistance)
                    _playerConfigs[victim.NetworkIDString].WeaponDeaths[@event.Weapon].LargestDistance = @event.Distance;
            }
        }
    }
}
