//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace NetMsg.Game{
#if !DONT_USE_GENERATE_CODE

    public partial class Msg_AllLoadingProgress{
        public override void Serialize(Serializer writer){
			writer.PutBoolean(isAllDone);
			writer.PutArray(progress);
        }
    
        public override void Deserialize(Deserializer reader){
			isAllDone = reader.GetBoolean();
			progress = reader.GetArray(ref this.progress);
        }
    }


    public partial class Msg_GameEvent{
        public override void Serialize(Serializer writer){
			writer.PutInt16(type);
			writer.PutArray(content);
        }
    
        public override void Deserialize(Deserializer reader){
			type = reader.GetInt16();
			content = reader.GetArray(ref this.content);
        }
    }


    public partial class Msg_HashCode{
        public override void Serialize(Serializer writer){
			writer.PutInt32(startTick);
			writer.PutArray(hashCodes);
        }
    
        public override void Deserialize(Deserializer reader){
			startTick = reader.GetInt32();
			hashCodes = reader.GetArray(ref this.hashCodes);
        }
    }


    public partial class Msg_LoadingProgress{
        public override void Serialize(Serializer writer){
			writer.PutByte(progress);
        }
    
        public override void Deserialize(Deserializer reader){
			progress = reader.GetByte();
        }
    }


    public partial class Msg_PartFinished{
        public override void Serialize(Serializer writer){
			writer.PutUInt16(level);
        }
    
        public override void Deserialize(Deserializer reader){
			level = reader.GetUInt16();
        }
    }


    public partial class Msg_PlayerReady{
        public override void Serialize(Serializer writer){
			writer.PutInt32(roomId);
        }
    
        public override void Deserialize(Deserializer reader){
			roomId = reader.GetInt32();
        }
    }


    public partial class Msg_RepInit{
        public override void Serialize(Serializer writer){
			writer.PutInt64(playerId);
        }
    
        public override void Deserialize(Deserializer reader){
			playerId = reader.GetInt64();
        }
    }


    public partial class Msg_RepMissFrameAck{
        public override void Serialize(Serializer writer){
			writer.PutInt32(missFrameTick);
        }
    
        public override void Deserialize(Deserializer reader){
			missFrameTick = reader.GetInt32();
        }
    }


    public partial class Msg_ReqMissFrame{
        public override void Serialize(Serializer writer){
			writer.PutInt32(startTick);
        }
    
        public override void Deserialize(Deserializer reader){
			startTick = reader.GetInt32();
        }
    }


    public partial class Msg_RoomInitMsg{
        public override void Serialize(Serializer writer){
			writer.PutString(name);
        }
    
        public override void Deserialize(Deserializer reader){
			name = reader.GetString();
        }
    }


    public partial class Msg_StartRoomGame{
        public override void Serialize(Serializer writer){
			writer.PutByte(ActorID);
			writer.PutInt32(RoomID);
			writer.PutInt32(Seed);
			writer.PutInt32(SimulationSpeed);
			writer.PutArray(AllActors);
        }
    
        public override void Deserialize(Deserializer reader){
			ActorID = reader.GetByte();
			RoomID = reader.GetInt32();
			Seed = reader.GetInt32();
			SimulationSpeed = reader.GetInt32();
			AllActors = reader.GetArray(ref this.AllActors);
        }
    }


#endif
}
