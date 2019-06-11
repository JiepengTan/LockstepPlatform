using System.Collections.Generic;
using LitJson;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;
using NetMsg.Server;
using User = Server.Servers.Lobby.LobbyServer.User;

namespace Server.Servers.Lobby {
    public partial class LobbyServer {
        public class Room : BaseRecyclable {
            public RoomInfo Info = new RoomInfo();

            public bool IsPlaying {
                get => Info.State == 1;
                set => Info.State = (byte) (value ? 1 : 0);
            }

            public int GameType {
                get => Info.GameType;
                set => Info.GameType = value;
            }

            public int RoomId {
                get => Info.RoomId;
                set => Info.RoomId = value;
            }

            public int MapId {
                get => Info.MapId;
                set => Info.MapId = value;
            }

            public string Name {
                get => Info.Name;
                set => Info.Name = value;
            }

            public byte CurPlayerCount {
                get => Info.CurPlayerCount;
                set => Info.CurPlayerCount = value;
            }

            public byte MaxPlayerCount {
                get => Info.MaxPlayerCount;
                set => Info.MaxPlayerCount = value;
            }

            public string OwnerName {
                get => Info.OwnerName;
                set => Info.OwnerName = value;
            }

            public string GameHash;
            public bool IsFull => CurPlayerCount >= MaxPlayerCount;

            public bool IsEmpty => CurPlayerCount <= 0;
            public List<User> Users = new List<User>(10);
            public User Owner;
            private RoomPlayerInfo[] _roomPlayerInfos;

            public IPeer ServerPeer;
            public RoomPlayerInfo[] RoomPlayerInfos {
                get {
                    if (_roomPlayerInfos != null)
                        return _roomPlayerInfos;
                    _roomPlayerInfos = new RoomPlayerInfo[Users.Count];
                    for (int i = 0; i < Users.Count; i++) {
                        var user = Users[i];
                        var info = new RoomPlayerInfo() {
                            UserId = user.UserId,
                            Name = user.Name,
                            Status = user.IsReady
                        };
                        _roomPlayerInfos[i] = info;
                    }

                    return _roomPlayerInfos;
                }
            }


            public void BorderMessage(short opCode, ISerializablePacket packet){
                var msg = MessageHelper.Create(opCode, packet.ToBytes());
                foreach (var user in Users) {
                    user.SendMessage(msg);
                }
            }
            public void BorderMessage(short opCode, byte[] bytes){
                foreach (var user in Users) {
                    var msg = MessageHelper.Create(opCode, bytes);
                    user.SendMessage(msg);
                }
            }
            public void BorderMessage(IMessage msg){
                foreach (var user in Users) {
                    user.SendMessage(msg);
                }
            }

            public void GetReconnectInfo(){
                ServerPeer?.SendMessage((short) EMsgLS.L2G_UserReconnect, new Msg_L2G_CreateRoom() {
                    GameType = user.GameType,
                    Players = playerInfos,
                    MapId = room.MapId,
                    GameHash = gameHash
                }, (status, response) => {
                    
                }
            }

            public void Init(int type, int roomId, string name, int mapId, byte maxPlayerCount, User owner){
                GameType = type;
                RoomId = roomId;
                Name = name;
                MapId = mapId;
                MaxPlayerCount = maxPlayerCount;

                CurPlayerCount = 1;
                OwnerName = owner.Name;
                IsPlaying = false;
                Users.Add(owner);
                Owner = owner;
            }

            public override void OnRecycle(){
                Users.Clear();
                Owner = null;
                _roomPlayerInfos = null;
                IsPlaying = false;
                ServerPeer = null;
            }

            public void AddUser(User user){
                Users.Add(user);
                CurPlayerCount++;
                _roomPlayerInfos = null;
            }

            public void RemoveUser(User user){
                if (Users.Remove(user)) {
                    CurPlayerCount--;
                    _roomPlayerInfos = null;
                }
            }

            public override string ToString(){
                return JsonMapper.ToJson(Info);
            }
        }
    }
}