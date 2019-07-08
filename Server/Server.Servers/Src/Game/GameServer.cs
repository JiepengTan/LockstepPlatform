using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using Lockstep.Logging;
using NetMsg.Server;
using Lockstep.Server.Common;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Server.Game {
    public class GameServer : Common.Server, IGameServer {
        private NetServer<EMsgSC> _netServerSCUdp;
        private NetServer<EMsgSC> _netServerSC;
        private NetClient<EMsgDS> _netClientDS;
        protected NetClient<EMsgLS> _netClientLG; //其他类型的Master 用于提供服务

        private Dictionary<int, BaseGame> _gameId2Games = new Dictionary<int, BaseGame>();

        private Dictionary<long, Player> _uid2Players = new Dictionary<long, Player>();

        private int CurGameId;

        private BaseGame[] _cachedBaseGames;

        public IPEndInfo TcpEnd { get; set; }
        public IPEndInfo UdpEnd { get; set; }

        private BaseGame[] CachedBaseGames {
            get {
                if (_cachedBaseGames == null) {
                    _cachedBaseGames = _gameId2Games.Values.ToArray();
                }

                return _cachedBaseGames;
            }
        }

        public override void DoStart(){
            base.DoStart();
            InitServerSC();
            TcpEnd = new IPEndInfo() {Ip = Ip, Port = _serverConfig.tcpPort};
            UdpEnd = new IPEndInfo() {Ip = Ip, Port = _serverConfig.udpPort};
        }

        public override void DoUpdate(int deltaTimeMs){
            base.DoUpdate(deltaTimeMs);
            var games = CachedBaseGames;
            foreach (var game in games) {
                game.DoUpdate(deltaTimeMs);
            }
        }

        public void TickOut(Player player, int reason){ }

        public void OnGameEmpty(IGame game){
            RemoveGame(game);
        }

        public void OnGameFinished(IGame game){
            Log($" OnGameFinished " + game.GameId);
            RemoveGame(game);
        }

        void RemoveGame(IGame game){
            if (_gameId2Games.TryGetValue(game.GameId, out var tGame)) {
                _netClientLG.SendMessage(EMsgLS.G2L_OnGameFinished, new Msg_G2L_OnGameFinished() {
                    GameId = tGame.GameId,
                    RoomId =tGame.RoomId
                });
                tGame.IsFinished = true;
                foreach (var uid in tGame.UserIds) {
                    _uid2Players.Remove(uid);
                }

                _gameId2Games.Remove(tGame.GameId);
                _cachedBaseGames = null;
                tGame.DoDestroy();
            }
        }

        private void InitServerSC(){
            InitNetServer(ref _netServerSC, _serverConfig.tcpPort, OnPlayerDisconnected);
            InitNetServer(ref _netServerSCUdp, _serverConfig.udpPort);
        }

        void OnPlayerDisconnected(IPeer peer){
            if (_peerId2Player.TryGetValue(peer.Id, out var player)) {
                Log("OnPlayerDisconnected  " + player.Account);
                _peerId2Player.Remove(peer.Id);
                player.Game.OnPlayerDisconnect(player);
            }
        }

        protected override void OnMasterServerInfo(ServerIpInfo info){
            if (info.ServerType == (byte) EServerType.DatabaseServer) {
                ReqOtherServerInfo(EServerType.DatabaseServer, (status, respond) => {
                    if (status != EResponseStatus.Failed) {
                        InitClientDS(respond.Parse<Msg_RepOtherServerInfo>().ServerInfo);
                    }
                });
            }

            if (info.ServerType == (byte) EServerType.LobbyServer) {
                ReqOtherServerInfo(EServerType.LobbyServer, (status, respond) => {
                    if (status != EResponseStatus.Failed) {
                        InitClientLG(respond.Parse<Msg_RepOtherServerInfo>().ServerInfo);
                    }
                });
            }
        }

        private void InitClientLG(ServerIpInfo info){
            InitNetClient(ref _netClientLG, info.Ip, info.Port, OnLobbyConn);
        }

        private void InitClientDS(ServerIpInfo info){
            InitNetClient(ref _netClientDS, info.Ip, info.Port, OnDBConn);
        }

        private void OnLobbyConn(){
            _netClientLG.SendMessage(EMsgLS.G2L_RegisterServer, new Msg_RegisterServer {
                ServerInfo = new ServerIpInfo() {
                    ServerType = (byte) serverType,
                    Ip = this.Ip,
                    Port = _serverConfig.tcpPort
                }
            });
            Debug.Log("OnLobbyConn");
        }

        private void OnDBConn(){
            Debug.Log("OnDBConn");
        }

        protected void L2G_UserLeave(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2G_UserLeave>();
            Log("L2G_UserLeave " + msg);
            if (_gameId2Games.TryGetValue(msg.GameId, out var game)) {
                game.OnPlayerLeave(msg.UserId);
            }
        }

        protected void L2G_UserReconnect(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2G_UserReconnect>();
            Log("L2G_UserReconnect " + msg);
            if (_uid2Players.TryGetValue(msg.PlayerInfo.UserId, out var player)) {
                var game = player.Game;
                if (game != null) {
                    game.OnPlayerReconnect(msg.PlayerInfo);
                    reader.Respond(1, EResponseStatus.Success);
                    return;
                }
            }

            reader.Respond(0, EResponseStatus.Failed);
        }

        protected void L2G_CreateGame(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2G_CreateGame>();
            var game = CreateGame(msg.GameType) as BaseGame;
            if (game == null) {
                reader.Respond(-1, EResponseStatus.Failed);
                return;
            }

            game.TcpEnd = TcpEnd;
            game.UdpEnd = UdpEnd;
            game.RoomId = msg.RoomId;
            Debug.Log("L2G_CreateGame" + msg);
            var gameId = CurGameId++;
            _gameId2Games.Add(gameId, game);
            _cachedBaseGames = null;

            game.DoStart(this, gameId, msg.GameType, msg.MapId, msg.Players, msg.GameHash);
            var players = game.Players;
            foreach (var player in players) {
                _uid2Players[player.UserId] = player;
                //get game data from database
                _netClientDS.SendMessage(EMsgDS.S2D_ReqGameData, new Msg_S2D_ReqGameData() {
                        account = player.Account
                    }, (status, respond) => {
                        var rMsg = respond.Parse<Msg_D2S_RepGameData>();
                        Debug.Log("Msg_D2S_RepGameData" + rMsg);
                        if (_uid2Players.TryGetValue(player.UserId, out var user)) {
                            user.GameData = rMsg.data;
                            if (user.Game != null && user.Game.GameId == gameId) { //防止玩家重新进入房间
                                user.Game.OnRecvPlayerGameData(user);
                            }
                        }
                    }
                );
            }


            //Load dlls
            reader.Respond(gameId, EResponseStatus.Success);
        }

        private Dictionary<int, Player> _peerId2Player = new Dictionary<int, Player>();

        protected void C2G_Hello(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2G_Hello>().Hello;
            Debug.Log("C2G_Hello" + msg + " id" + reader.Peer.Id);
            var userInfo = msg.UserInfo;
            if (userInfo == null)
                return;
            var game = _gameId2Games.GetRefVal(msg.GameId);
            if (game != null
                && game.GameType == msg.GameType
                && game.GameHash == msg.GameHash
            ) {
                var localId = game.GetUserLocalId(userInfo.UserId);
                if (localId != -1) {
                    var player = game.Players[localId];
                    if (player.LoginHash == userInfo.LoginHash) {
                        player.PeerTcp = reader.Peer;
                        _peerId2Player[reader.Peer.Id] = player;
                        reader.Peer.AddExtension(player);
                        reader.Respond(EMsgSC.G2C_Hello, new Msg_G2C_Hello() {
                            LocalId = (byte) localId,
                            UserCount = (byte) game.MaxPlayerCount,
                            MapId = game.MapId,
                            GameId = game.GameId,
                            Seed = game.Seed,
                            UdpEnd = new IPEndInfo() {
                                Ip = this.Ip,
                                Port = _serverConfig.udpPort
                            }
                        });
                        game.OnPlayerConnect(player);
                        return;
                    }
                }
            }

            reader.Respond(1, EResponseStatus.Failed);
        }


        protected void C2G_UdpHello(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2G_UdpHello>().Hello;
            Debug.Log("C2G_UdpHello" + msg + " id" + reader.Peer.Id);
            var userInfo = msg.UserInfo;
            if (userInfo == null)
                return;
            var game = _gameId2Games.GetRefVal(msg.GameId);
            if (game != null
                && game.GameType == msg.GameType
                && game.GameHash == msg.GameHash
            ) {
                var localId = game.GetUserLocalId(userInfo.UserId);
                if (localId != -1) {
                    var player = game.Players[localId];
                    if (player.LoginHash == userInfo.LoginHash) {
                        player.PeerUdp = reader.Peer;
                        reader.Peer.AddExtension(player);
                        reader.Respond(EMsgSC.G2C_UdpHello, new Msg_G2C_Hello() {
                            MapId = game.MapId,
                            LocalId = (byte) localId
                        });
                    }
                }
            }
        }

        protected void C2G_UdpMessage(IIncommingMessage reader){
            var player = reader.Peer?.GetExtension<Player>();
            if (player == null) {
                reader.Peer?.Disconnect("");
                Debug.Log("C2G_UdpMessage: Error msg unknown user peerId = " + reader.Peer.Id);
                return;
            }

            var deserializer = reader.GetData();
            if (deserializer == null) return;
            player.Game.OnRecvMsg(player, deserializer);
        }

        protected void C2G_TcpMessage(IIncommingMessage reader){
            var player = reader.Peer?.GetExtension<Player>();
            if (player == null) {
                reader.Peer.Disconnect("");
                Debug.Log("C2G_TcpMessage: Error msg unknown user peerId = " + reader.Peer.Id);
                return;
            }

            var deserializer = reader.GetData();
            if (deserializer == null) return;
            player.Game.OnRecvMsg(player, deserializer);
        }

        private delegate IGame FuncCreateGame();

        private Dictionary<int, FuncCreateGame> _gameFactoryFuncs = new Dictionary<int, FuncCreateGame>();

        string GameType2DllPath(int type){
            return "Lockstep.Server.CommonGame" + ".dll"; //TODO ReadFromConfig
        }

        /// Create From Dll by reflect 
        private IGame CreateGame(int type){
            //TODO Pool
            if (_gameFactoryFuncs.TryGetValue(type, out FuncCreateGame _func)) {
                return _func?.Invoke();
            }

            var path = GameType2DllPath(type);
            if (path == null) {
                return null;
            }

            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            Debug.Log("Load dll " + dllPath);
            if (!File.Exists(dllPath)) {
                Debug.LogError("Load dll failed  " + dllPath);
                _gameFactoryFuncs[type] = null;
                return null;
            }

            var assembly = Assembly.LoadFrom(dllPath);
            var types = assembly.GetTypes().Where(
                t => t.IsSubclassOf(typeof(BaseGame))
            ).ToArray();
            if (types.Length != 1) {
                Debug.LogError("dll do not have type of IGame :" + dllPath);
                _gameFactoryFuncs[type] = null;
                return null;
            }

            FuncCreateGame factory = () => { return (IGame) System.Activator.CreateInstance(types[0], true); };
            _gameFactoryFuncs[type] = factory;
            var game = factory();
            if (game == null) {
                Debug.LogError($"Can not load Game Dll type = {type}");
                return null;
            }

            return game;
        }
    }
}