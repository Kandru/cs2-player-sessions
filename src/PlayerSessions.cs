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
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
            RegisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
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
            DeregisterEventHandler<EventRoundEnd>(OnRoundEnd);
            DeregisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
            DeregisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            DeregisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            // save config
            Config.Update();
            Console.WriteLine(Localizer["core.unload"]);
        }

        private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            // save config
            Config.Update();
            return HookResult.Continue;
        }

        private HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
        {
            // skip bots
            if (@event.Bot) return HookResult.Continue;
            string username = @event.Name;
            string ipAddress = @event.Address;
            if (@event.Address.Contains(Separator)) ipAddress = ipAddress.Split(Separator)[0];
            string steamId = @event.Networkid;
            string city = "";
            string country = "";
            // create user config if not exists
            CreateUserConfig(steamId);
            // get data from ip resolver
            Dictionary<string, string> ipData = IP2Country(ipAddress);
            if (ipData.ContainsKey("city")) city = ipData["city"];
            if (ipData.ContainsKey("country")) country = ipData["country"];
            // notify players
            if (Config.JoinMessageEnable)
                if ((!Config.EnableCityLookup && !Config.EnableCountryLookup)
                    || !Config.EnableCountryLookup
                    || (city == "" && country == "")
                    || country == "")
                    SendGlobalChatMessage(Localizer["player.connect"].Value
                        .Replace("{player}", username));
                else if (Config.EnableCityLookup && city != "" && country != "")
                    SendGlobalChatMessage(Localizer["player.connect.city"].Value
                        .Replace("{player}", username)
                        .Replace("{city}", ipData["city"])
                        .Replace("{country}", ipData["country"]));
                else
                    SendGlobalChatMessage(Localizer["player.connect.country"].Value
                        .Replace("{player}", username)
                        .Replace("{country}", ipData["country"]));
            // add data
            Config.Player[steamId].LastIp = ipAddress;
            if (country != "") Config.Player[steamId].City = city;
            if (country != "") Config.Player[steamId].Country = country;
            Config.Player[steamId].Username = username;
            Config.Player[steamId].ClanTag = "";
            Config.Player[steamId].LastConnected = GetCurrentTimestamp();
            // cooldown for connection counter (avoid counting rejoin)
            if (Config.Player[steamId].LastConnected == 0
                || Config.Player[steamId].LastConnected >= (Config.Player[steamId].LastDisconnected + (60 * 5)))
                Config.Player[steamId].ConnectionCount += 1;
            Config.Player[steamId].LastDisconnected = Config.Player[steamId].LastConnected;
            return HookResult.Continue;
        }

        private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid!;
            // skip bots
            if (player.IsBot) return HookResult.Continue;
            // skip if not added
            if (!Config.Player.ContainsKey(player.NetworkIDString)) return HookResult.Continue;
            // add data
            Config.Player[player.NetworkIDString].ClanTag = player.ClanName;
            // show welcome message
            if (Config.WelcomeMessageEnable)
                AddTimer(Config.WelcomeMesageDelay, () =>
                {
                    // bounds check
                    if (player == null || !player.IsValid) return;
                    // print welcome message(s)
                    player.PrintToChat(Localizer["player.welcome"].Value
                        .Replace("{player}", player.PlayerName));
                    if (Config.Player[player.NetworkIDString].ConnectionCount > 1)
                        player.PrintToChat(Localizer["player.welcome.visit"].Value
                            .Replace("{visit}", Config.Player[player.NetworkIDString].ConnectionCount.ToString()));
                    if (Config.Player[player.NetworkIDString].PlaytimeTotal > 0)
                        player.PrintToChat(Localizer["player.welcome.playtime"].Value
                            .Replace(
                                "{playtime}",
                                String.Format("{0:0.00}", Config.Player[player.NetworkIDString].PlaytimeTotal / 60.0f / 60.0f)
                            ));
                });
            return HookResult.Continue;
        }

        private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid!;
            // skip bots
            if (player.IsBot) return HookResult.Continue;
            string steamId = @event.Networkid;
            // notify players
            if (Config.PartMessageEnable)
                SendGlobalChatMessage(Localizer["player.disconnect"].Value
                    .Replace("{player}", player.PlayerName));
            // add data
            if (!Config.Player.ContainsKey(steamId)) return HookResult.Continue;
            Config.Player[steamId].Username = player.PlayerName;
            Config.Player[steamId].ClanTag = player.ClanName;
            Config.Player[steamId].LastDisconnected = GetCurrentTimestamp();
            // add total playtime
            Config.Player[steamId].PlaytimeTotal += Config.Player[steamId].LastDisconnected - Config.Player[steamId].LastConnected;
            return HookResult.Continue;
        }
    }
}
