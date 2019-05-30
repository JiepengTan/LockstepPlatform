using Lockstep.Serialization;

namespace NetMsg.Lobby {
    public partial class RoomInfo:BaseFormater {
        public int roomId;
        public string name;
        public ushort mapId;
        public byte statu;
        public byte curCount;
        public byte maxCount;
    }

    public partial class Msg_ReqLogin :BaseFormater{
        public string account;
        public string password;
    }
    
    public partial class Msg_RepLogin :BaseFormater{
        public long playerId;
        public string ip;
        public int port;
        public int roomId;
        public byte[] childMsg;
        public RoomInfo[] roomInfos;
    }
    public partial class Msg_ReqRoomList :BaseFormater{
        public byte dump;
    }
    public partial class Msg_RepRoomList:BaseFormater {
        public RoomInfo[] Child;
    }

    public partial class Msg_JoinRoom :BaseFormater{
        public int roomId;
    }
    public partial class Msg_JoinRoomResult :BaseFormater{
        public byte statu;
        public int roomId;
    }
    public partial class Msg_CreateRoom:BaseFormater {
        public byte type;
        public byte size;
        public string name;
    }
    public partial class Msg_CreateRoomResult :BaseFormater{
        public int roomId;
        public byte type;
        public byte size;
        public string name;
    }
    
    public partial class Msg_LeaveRoom :BaseFormater{
        public int pad;
    }
    public partial class Msg_LeaveRoomResult:BaseFormater {
        public byte result;
    }  
    public partial class Msg_PlayerReady:BaseFormater {
        public byte pad;
    }
    public partial class Msg_PlayerReadyResult:BaseFormater {
        public byte result;
    } 
    public partial class Msg_StartGame :BaseFormater{
        public string ip;
        public int port;
        public int roomId;
        public byte localId;
        public byte[] allActorIds;
    }
    public partial class Msg_LobbyStatus :BaseFormater{
        public RoomInfo[] modifiedRooms;
        public int[] deleteRooms;
    }
    public partial class Msg_RoomStatus :BaseFormater {
        public RoomInfo roomInfo;
    }
}