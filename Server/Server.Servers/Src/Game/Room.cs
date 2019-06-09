using System.Collections.Generic;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Server.Game {
    public class Room : BaseRecyclable {
        public int MapId;
        public int RoomId;
        public string GameHash;
        public int GameType;
        public Player[] Players;
        public Dictionary<long, byte> userId2LocalId = new Dictionary<long, byte>();

        private IGame _baseGame;
        public float TimeSinceCreate;

        public void OnRecvPlayerGameData(Player player){
            if (player == null || Players.Length <= player.LocalId || Players[player.LocalId] != player) {
                return;
            }
            player.HasRecvData = true;
            bool hasRecvAll = true;
            foreach (var user in Players) {
                if (user != null && !user.HasRecvData) {
                    hasRecvAll = false;
                    break;
                }
            }

            var playerCount = Players.Length;
            if (hasRecvAll) {
                var userInfos = new GameData[playerCount];
                for (int i = 0; i < playerCount; i++) {
                    userInfos[i] = Players[i]?.GameData;
                }

                _baseGame.GameStartInfo = new Msg_G2C_GameStartInfo() {
                    MapId = MapId,
                    RoomId = RoomId,
                    Seed = LRandom.Next(10000),
                    UserCount = Players.Length,
                    TcpEnd = new IPEndInfo() {Port = _baseGame.TcpPort, Ip = _baseGame.Ip},
                    UdpEnd = new IPEndInfo() {Port = _baseGame.UdpPort, Ip = _baseGame.Ip},
                    SimulationSpeed = 60,
                    UserInfos = userInfos
                };
                //all user data ready notify game start
                _baseGame.BorderMessageTcp(EMsgSC.G2C_GameStartInfo, _baseGame.GameStartInfo);
            }
        }

        public void Init(IGame game){
            _baseGame = game;
            userId2LocalId.Clear();
            TimeSinceCreate = Time.timeSinceLevelLoad;
            var userCount = Players.Length;
            for (byte i = 0; i < userCount; i++) {
                var player = Players[i];
                if (player != null) {
                    userId2LocalId.Add(player.UserId, player.LocalId);
                    player.HasRecvData = false;
                }
            }
        }


        public void OnRecvMsg(Player player, Deserializer reader){
            _baseGame?.OnRecvMsg(player, reader);
        }

        public override void OnRecycle(){
            if (Players == null) return;
            foreach (var player in Players) {
                Pool.Return(player);
            }

            Players = null;
            RoomId = -1;
        }

        public int GetUserLocalId(long userId){
            if (userId2LocalId.TryGetValue(userId, out var id)) {
                return id;
            }

            return -1;
        }

        public int CheckAndGetUserLocalId(GamePlayerInfo user){
            if (userId2LocalId.TryGetValue(user.UserId, out var id)) {
                if (Players[id].LoginHash == user.LoginHash)
                    return id;
            }

            return -1;
        }
    }
}