using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;


namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        public override string ModuleName => "Player Sessions";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";

        Random _random = new Random(Guid.NewGuid().GetHashCode());

        public override void Load(bool hotReload)
        {
            // initialize IP lookup
            InitializeIP2Country();
            // register listeners
            RegisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
            RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            // print message if hot reload
            if (hotReload)
            {
                Console.WriteLine(Localizer["core.hotreload"]);
            }
        }

        public override void Unload(bool hotReload)
        {
            // unregister listeners
            DeregisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
            DeregisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            // save config
            Config.Update();
            Console.WriteLine(Localizer["core.unload"]);
        }

        private HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
        {
            // skip bots
            if (@event.Bot) return HookResult.Continue;
            CCSPlayerController player = @event.Userid!;
            string ipAddress = @event.Address;
            string steamId = @event.Networkid;
            string country = "";
            // create user config if not exists
            CreateUserConfig(steamId);
            // get data from ip resolver
            if (IpResolver != null)
            {
                var result = IpResolver.Resolve(ipAddress);
                country = result.Country;
            }
            // notify players
            if (country == "")
                SendGlobalChatMessage(Localizer["player.connected"].Value
                .Replace("{player}", player.PlayerName));
            else
                SendGlobalChatMessage(Localizer["player.connected"].Value
                .Replace("{player}", player.PlayerName)
                .Replace("{country}", country));
            // add data
            if (country != "") Config.Player[steamId].Country = country;
            Config.Player[steamId].Username = player.PlayerName;
            Config.Player[steamId].ClanTag = player.ClanName;
            Config.Player[steamId].LastConnected = GetCurrentTimestamp();
            // cooldown for connection counter (avoid counting rejoin)
            if (Config.Player[steamId].LastDisconnected <= (Config.Player[steamId].LastConnected - (60 * 5)))
                Config.Player[steamId].ConnectionCount += 1;
            Config.Player[steamId].LastDisconnected = Config.Player[steamId].LastConnected;
            return HookResult.Continue;
        }

        private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid!;
            string steamId = @event.Networkid;
            // notify players
            SendGlobalChatMessage(Localizer["player.disconnected"].Value
            .Replace("{player}", player.PlayerName));
            // add data
            Config.Player[steamId].Username = player.PlayerName;
            Config.Player[steamId].ClanTag = player.ClanName;
            Config.Player[steamId].LastDisconnected = GetCurrentTimestamp();
            // add total playtime
            Config.Player[steamId].PlaytimeTotal += Config.Player[steamId].LastConnected + Config.Player[steamId].LastDisconnected;
            return HookResult.Continue;
        }
    }
}
