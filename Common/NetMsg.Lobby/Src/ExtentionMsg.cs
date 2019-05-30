//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace NetMsg.Lobby{
#if !DONT_USE_GENERATE_CODE

    public partial class Msg_CreateRoom{
        public override void Serialize(Serializer writer){
			writer.PutString(name);
			writer.PutByte(size);
			writer.PutByte(type);
        }
    
        public override void Deserialize(Deserializer reader){
			name = reader.GetString();
			size = reader.GetByte();
			type = reader.GetByte();
        }
    }


    public partial class Msg_CreateRoomResult{
        public override void Serialize(Serializer writer){
			writer.PutString(name);
			writer.PutInt32(roomId);
			writer.PutByte(size);
			writer.PutByte(type);
        }
    
        public override void Deserialize(Deserializer reader){
			name = reader.GetString();
			roomId = reader.GetInt32();
			size = reader.GetByte();
			type = reader.GetByte();
        }
    }


    public partial class Msg_JoinRoom{
        public override void Serialize(Serializer writer){
			writer.PutInt32(roomId);
        }
    
        public override void Deserialize(Deserializer reader){
			roomId = reader.GetInt32();
        }
    }


    public partial class Msg_JoinRoomResult{
        public override void Serialize(Serializer writer){
			writer.PutInt32(roomId);
			writer.PutByte(statu);
        }
    
        public override void Deserialize(Deserializer reader){
			roomId = reader.GetInt32();
			statu = reader.GetByte();
        }
    }


    public partial class Msg_LeaveRoom{
        public override void Serialize(Serializer writer){
			writer.PutInt32(pad);
        }
    
        public override void Deserialize(Deserializer reader){
			pad = reader.GetInt32();
        }
    }


    public partial class Msg_LeaveRoomResult{
        public override void Serialize(Serializer writer){
			writer.PutByte(result);
        }
    
        public override void Deserialize(Deserializer reader){
			result = reader.GetByte();
        }
    }


    public partial class Msg_LobbyStatus{
        public override void Serialize(Serializer writer){
			writer.PutArray(deleteRooms);
			writer.PutArray(modifiedRooms);
        }
    
        public override void Deserialize(Deserializer reader){
			deleteRooms = reader.GetArray(ref this.deleteRooms);
			modifiedRooms = reader.GetArray(ref this.modifiedRooms);
        }
    }


    public partial class Msg_PlayerReady{
        public override void Serialize(Serializer writer){
			writer.PutByte(pad);
        }
    
        public override void Deserialize(Deserializer reader){
			pad = reader.GetByte();
        }
    }


    public partial class Msg_PlayerReadyResult{
        public override void Serialize(Serializer writer){
			writer.PutByte(result);
        }
    
        public override void Deserialize(Deserializer reader){
			result = reader.GetByte();
        }
    }


    public partial class Msg_RepLogin{
        public override void Serialize(Serializer writer){
			writer.PutString(ip);
			writer.PutInt64(playerId);
			writer.PutInt32(port);
			writer.PutInt32(roomId);
			writer.PutArray(childMsg);
			writer.PutArray(roomInfos);
        }
    
        public override void Deserialize(Deserializer reader){
			ip = reader.GetString();
			playerId = reader.GetInt64();
			port = reader.GetInt32();
			roomId = reader.GetInt32();
			childMsg = reader.GetArray(ref this.childMsg);
			roomInfos = reader.GetArray(ref this.roomInfos);
        }
    }


    public partial class Msg_RepRoomList{
        public override void Serialize(Serializer writer){
			writer.PutArray(Child);
        }
    
        public override void Deserialize(Deserializer reader){
			Child = reader.GetArray(ref this.Child);
        }
    }


    public partial class Msg_ReqLogin{
        public override void Serialize(Serializer writer){
			writer.PutString(account);
			writer.PutString(password);
        }
    
        public override void Deserialize(Deserializer reader){
			account = reader.GetString();
			password = reader.GetString();
        }
    }


    public partial class Msg_ReqRoomList{
        public override void Serialize(Serializer writer){
			writer.PutByte(dump);
        }
    
        public override void Deserialize(Deserializer reader){
			dump = reader.GetByte();
        }
    }


    public partial class Msg_RoomStatus{
        public override void Serialize(Serializer writer){
			writer.Put(roomInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			roomInfo = reader.Get(ref this.roomInfo);
        }
    }


    public partial class Msg_StartGame{
        public override void Serialize(Serializer writer){
			writer.PutString(ip);
			writer.PutByte(localId);
			writer.PutInt32(port);
			writer.PutInt32(roomId);
			writer.PutArray(allActorIds);
        }
    
        public override void Deserialize(Deserializer reader){
			ip = reader.GetString();
			localId = reader.GetByte();
			port = reader.GetInt32();
			roomId = reader.GetInt32();
			allActorIds = reader.GetArray(ref this.allActorIds);
        }
    }


    public partial class RoomInfo{
        public override void Serialize(Serializer writer){
			writer.PutByte(curCount);
			writer.PutUInt16(mapId);
			writer.PutByte(maxCount);
			writer.PutString(name);
			writer.PutInt32(roomId);
			writer.PutByte(statu);
        }
    
        public override void Deserialize(Deserializer reader){
			curCount = reader.GetByte();
			mapId = reader.GetUInt16();
			maxCount = reader.GetByte();
			name = reader.GetString();
			roomId = reader.GetInt32();
			statu = reader.GetByte();
        }
    }


#endif
}
