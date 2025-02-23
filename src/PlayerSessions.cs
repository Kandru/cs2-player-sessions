using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Extensions;


namespace PlayerSessions
{
    public partial class PlayerSessions : BasePlugin
    {
        public override string ModuleName => "Player Sessions";
        public override string ModuleAuthor => "Kalle <kalle@kandru.de>";

        private Random _random = new Random(Guid.NewGuid().GetHashCode());
        private bool _isDuringRound = false;

        public override void Load(bool hotReload)
        {
            // update config if changed after update
            Config.Update();
            // initialize IP lookup
            InitializeIP2Country();
            // load player list
            LoadPlayerlist();
            // load challenges
            LoadChallenges();
            CheckForRunningChallenge();
            // register listeners
            // map events
            RegisterListener<Listeners.OnServerHibernationUpdate>(OnServerHibernationUpdate);
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
            RegisterEventHandler<EventRoundStart>(OnRoundStart);
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
            // player events
            RegisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
            RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            RegisterEventHandler<EventPlayerJump>(OnPlayerJump);
            RegisterEventHandler<EventPlayerBlind>(OnPlayerBlind);
            // print message if hot reload
            if (hotReload)
            {
                // load player configs
                LoadActivePlayerConfigs();
                Console.WriteLine(Localizer["core.hotreload"]);
            }
        }

        public override void Unload(bool hotReload)
        {
            // unregister listeners
            // map events
            RemoveListener<Listeners.OnServerHibernationUpdate>(OnServerHibernationUpdate);
            RemoveListener<Listeners.OnMapStart>(OnMapStart);
            RemoveListener<Listeners.OnMapEnd>(OnMapEnd);
            DeregisterEventHandler<EventRoundStart>(OnRoundStart);
            DeregisterEventHandler<EventRoundEnd>(OnRoundEnd);
            // player events
            DeregisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
            DeregisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            DeregisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            DeregisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
            DeregisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            DeregisterEventHandler<EventPlayerJump>(OnPlayerJump);
            DeregisterEventHandler<EventPlayerBlind>(OnPlayerBlind);
            // save config(s)
            Config.Update();
            SavePlayerConfigs();
            SavePlayerList();
            // hide GUI(s)
            HideAllPersonalStatisticsGUI();
            HideAllPersonalChallengesGUI();
            Console.WriteLine(Localizer["core.unload"]);
        }

        private void OnServerHibernationUpdate(bool isHibernating)
        {
            if (isHibernating)
            {
                // save config(s)
                SavePlayerConfigs();
                SavePlayerList();
                // hide GUI(s)
                HideAllPersonalStatisticsGUI();
                HideAllPersonalChallengesGUI();
            }
            else
            {
                // garbage collection
                PlayerConfigsGarbageCollection();
                // load
                LoadPlayerlist();
                LoadActivePlayerConfigs();
                LoadChallenges();
                CheckForRunningChallenge();
            }
        }

        private void OnMapStart(string mapName)
        {
            // garbage collection
            PlayerConfigsGarbageCollection();
            // load
            LoadPlayerlist();
            LoadActivePlayerConfigs();
            LoadChallenges();
            CheckForRunningChallenge();
        }

        private void OnMapEnd()
        {
            // save config(s)
            Config.Update();
            SavePlayerConfigs();
            SavePlayerList();
        }

        private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            // set variables
            _isDuringRound = true;
            // run functions
            CalculatePlaytimeRoundStart();
            ShowPersonalStatisticsOnRoundStart();
            ShowPersonalChallengesOnRoundStart();
            return HookResult.Continue;
        }

        private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            // run functions
            CalculatePlaytimeRoundEnd();
            // reset variables
            _isDuringRound = false;
            // garbage collection
            PlayerConfigsGarbageCollection();
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
            // read user configuration
            LoadPlayerConfig(steamId);
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
            _playerConfigs[steamId].LastIp = ipAddress;
            if (country != "") _playerConfigs[steamId].City = city;
            if (country != "") _playerConfigs[steamId].Country = country;
            _playerConfigs[steamId].Username = username;
            _playerConfigs[steamId].LastConnected = GetUnixTimestamp();
            // update player list
            _playerList[steamId].Username = username;
            _playerList[steamId].Kills = _playerConfigs[steamId].Kills;
            _playerList[steamId].Deaths = _playerConfigs[steamId].Deaths;
            // cooldown for connection counter (avoid counting rejoin)
            if (_playerConfigs[steamId].LastConnected == 0
                || _playerConfigs[steamId].LastConnected >= (_playerConfigs[steamId].LastDisconnected + (60 * 5)))
                _playerConfigs[steamId].ConnectionCount += 1;
            _playerConfigs[steamId].LastDisconnected = _playerConfigs[steamId].LastConnected;
            return HookResult.Continue;
        }

        private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            CCSPlayerController player = @event.Userid!;
            // skip bots
            if (player.IsBot) return HookResult.Continue;
            // show join message
            if (Config.JoinMessageEnable)
                SendGlobalChatMessage(
                    message: Localizer["player.connected"].Value
                        .Replace("{player}", player.PlayerName)
                    ,
                    player: player
                );
            // skip if not added
            if (!_playerConfigs.ContainsKey(player.NetworkIDString))
            {
                // read user configuration
                LoadPlayerConfig(player.NetworkIDString);
                if (!_playerConfigs.ContainsKey(player.NetworkIDString)) return HookResult.Continue;
            }
            // add data
            _playerConfigs[player.NetworkIDString].ClanTag = player.ClanName;
            // update player list
            _playerList[player.NetworkIDString].ClanTag = player.ClanName;
            // show welcome message
            if (Config.WelcomeMessageEnable)
                AddTimer(Config.WelcomeMesageDelay, () =>
                {
                    // bounds check
                    if (player == null || !player.IsValid) return;
                    // print welcome message(s)
                    player.PrintToChat(Localizer["player.welcome"].Value
                        .Replace("{player}", player.PlayerName));
                    if (_playerConfigs[player.NetworkIDString].ConnectionCount > 1)
                        player.PrintToChat(Localizer["player.welcome.visit"].Value
                            .Replace("{visit}", _playerConfigs[player.NetworkIDString].ConnectionCount.ToString()));
                    if (_playerConfigs[player.NetworkIDString].PlaytimeTotal > 0)
                        player.PrintToChat(Localizer["player.welcome.playtime"].Value
                            .Replace(
                                "{playtime}",
                                String.Format("{0:0.00}", _playerConfigs[player.NetworkIDString].PlaytimeTotal / 60.0f / 60.0f)
                            ));
                });
            // bugfix: show empty worldtext on connect to allow instant display of worldtext entity
            WorldTextManager.Create(player, "");
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
            if (!_playerConfigs.ContainsKey(steamId)) return HookResult.Continue;
            _playerConfigs[steamId].Username = player.PlayerName;
            _playerConfigs[steamId].ClanTag = player.ClanName;
            _playerConfigs[steamId].LastDisconnected = GetUnixTimestamp();
            // update player list
            _playerList[steamId].Username = player.PlayerName;
            _playerList[steamId].ClanTag = player.ClanName;
            // add total playtime
            _playerConfigs[steamId].PlaytimeTotal += _playerConfigs[steamId].LastDisconnected - _playerConfigs[steamId].LastConnected;
            return HookResult.Continue;
        }

        private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            // show GUIs
            ShowPersonalStatisticsOnSpawn(@event.Userid!);
            ShowPersonalChallengesOnSpawn(@event.Userid!);
            return HookResult.Continue;
        }

        private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            // run functions
            CalculatePlaytimePlayerDeath(@event, info);
            CalculateWeaponKillsPlayerDeath(@event, info);
            CalculateWeaponDeathsPlayerDeath(@event, info);
            // hide GUIs
            HidePersonalStatisticsGUI(@event.Userid!);
            HidePersonalChallengesGUI(@event.Userid!);
            return HookResult.Continue;
        }

        private HookResult OnPlayerJump(EventPlayerJump @event, GameEventInfo info)
        {
            // run functions
            CalculatePlayerJump(@event, info);
            return HookResult.Continue;
        }

        private HookResult OnPlayerBlind(EventPlayerBlind @event, GameEventInfo info)
        {
            // run functions
            CalculatePlayerBlind(@event, info);
            return HookResult.Continue;
        }
    }
}
