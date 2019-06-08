using LitJson;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Util;

using Room = Server.Servers.Lobby.LobbyServer.Room;
namespace Server.Servers.Lobby {
    public partial class LobbyServer {
        public class User : BaseRecyclable {
            public int GameType;
            public long UserId;
            public string Name;
            public byte IsReady;
            public string Account;
            public string LoginHash; //用于校验玩家
            public IPeer Peer;
            public Room Room;

            public void Init(long userID, int gameType, string name, string account, string loginHash){
                this.UserId = userID;
                this.GameType = gameType;
                this.Name = name;
                this.Account = account;
                this.LoginHash = loginHash;
            }

            public override void OnRecycle(){
                Room = null;
                Peer = null;
            }

            public override string ToString(){
                return JsonMapper.ToJson(this);
            }

            public void SendMessage(short opCode, ISerializablePacket packet){
                Peer?.SendMessage(opCode, packet);
            }

            public void SendMessage(IMessage message){
                Peer?.SendMessage(message);
            }

            public void SendMessage(IMessage message, ResponseCallback responseCallback){
                Peer?.SendMessage(message, responseCallback);
            }
        }
    }
}