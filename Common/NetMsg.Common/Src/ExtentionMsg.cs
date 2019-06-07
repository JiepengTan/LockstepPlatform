//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace NetMsg.Common{
#if !DONT_USE_GENERATE_CODE

    public partial class AccountData{
        public override void Serialize(Serializer writer){
			writer.PutString(Email);
			writer.PutBoolean(IsAdmin);
			writer.PutBoolean(IsEmailConfirmed);
			writer.PutBoolean(IsGuest);
			writer.PutString(Password);
			writer.PutString(Token);
			writer.PutInt64(UserId);
			writer.PutString(Username);
        }
    
        public override void Deserialize(Deserializer reader){
			Email = reader.GetString();
			IsAdmin = reader.GetBoolean();
			IsEmailConfirmed = reader.GetBoolean();
			IsGuest = reader.GetBoolean();
			Password = reader.GetString();
			Token = reader.GetString();
			UserId = reader.GetInt64();
			Username = reader.GetString();
        }
    }


    public partial class GamePlayerInfo{
        public override void Serialize(Serializer writer){
			writer.PutString(Account);
			writer.PutString(LoginHash);
			writer.PutInt64(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			Account = reader.GetString();
			LoginHash = reader.GetString();
			UserId = reader.GetInt64();
        }
    }


    public partial class IPEndInfo{
        public override void Serialize(Serializer writer){
			writer.PutString(Ip);
			writer.PutUInt16(Port);
        }
    
        public override void Deserialize(Deserializer reader){
			Ip = reader.GetString();
			Port = reader.GetUInt16();
        }
    }


    public partial class Msg_C2G_GameEvent{
        public override void Serialize(Serializer writer){
			writer.PutArray(Data);
        }
    
        public override void Deserialize(Deserializer reader){
			Data = reader.GetArray(ref this.Data);
        }
    }


    public partial class Msg_C2G_Hello{
        public override void Serialize(Serializer writer){
			writer.PutString(GameHash);
			writer.PutInt32(GameType);
			writer.PutInt32(RoomId);
			writer.Put(UserInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			GameHash = reader.GetString();
			GameType = reader.GetInt32();
			RoomId = reader.GetInt32();
			UserInfo = reader.Get(ref this.UserInfo);
        }
    }


    public partial class Msg_C2G_LoadingProgress{
        public override void Serialize(Serializer writer){
			writer.PutByte(Progress);
        }
    
        public override void Deserialize(Deserializer reader){
			Progress = reader.GetByte();
        }
    }


    public partial class Msg_C2G_UdpHello{
        public override void Serialize(Serializer writer){
			writer.Put(UserInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			UserInfo = reader.Get(ref this.UserInfo);
        }
    }


    public partial class Msg_C2I_UserLogin{
        public override void Serialize(Serializer writer){
			writer.PutString(Account);
			writer.PutString(EncryptHash);
			writer.PutInt32(GameType);
			writer.PutString(Password);
        }
    
        public override void Deserialize(Deserializer reader){
			Account = reader.GetString();
			EncryptHash = reader.GetString();
			GameType = reader.GetInt32();
			Password = reader.GetString();
        }
    }


    public partial class Msg_C2L_CreateRoom{
        public override void Serialize(Serializer writer){
			writer.PutInt32(GameType);
			writer.PutInt32(MapId);
			writer.PutByte(MaxPlayerCount);
			writer.PutString(Name);
        }
    
        public override void Deserialize(Deserializer reader){
			GameType = reader.GetInt32();
			MapId = reader.GetInt32();
			MaxPlayerCount = reader.GetByte();
			Name = reader.GetString();
        }
    }


    public partial class Msg_C2L_JoinRoom{
        public override void Serialize(Serializer writer){
			writer.PutInt32(RoomId);
        }
    
        public override void Deserialize(Deserializer reader){
			RoomId = reader.GetInt32();
        }
    }


    public partial class Msg_C2L_LeaveRoom{
        public override void Serialize(Serializer writer){
			writer.PutByte(Reason);
        }
    
        public override void Deserialize(Deserializer reader){
			Reason = reader.GetByte();
        }
    }


    public partial class Msg_C2L_StartGame{
        public override void Serialize(Serializer writer){
			writer.PutByte(Reason);
        }
    
        public override void Deserialize(Deserializer reader){
			Reason = reader.GetByte();
        }
    }


    public partial class Msg_C2L_UserLogin{
        public override void Serialize(Serializer writer){
			writer.PutString(LoginHash);
			writer.PutInt64(userId);
        }
    
        public override void Deserialize(Deserializer reader){
			LoginHash = reader.GetString();
			userId = reader.GetInt64();
        }
    }


    public partial class Msg_CreateAccount{
        public override void Serialize(Serializer writer){
			writer.PutString(account);
			writer.PutString(password);
        }
    
        public override void Deserialize(Deserializer reader){
			account = reader.GetString();
			password = reader.GetString();
        }
    }


    public partial class Msg_G2C_AllFinishedLoaded{
        public override void Serialize(Serializer writer){
			writer.PutByte(Padding);
        }
    
        public override void Deserialize(Deserializer reader){
			Padding = reader.GetByte();
        }
    }


    public partial class Msg_G2C_GameEvent{
        public override void Serialize(Serializer writer){
			writer.PutArray(Data);
        }
    
        public override void Deserialize(Deserializer reader){
			Data = reader.GetArray(ref this.Data);
        }
    }


    public partial class Msg_G2C_GameInfo{
        public override void Serialize(Serializer writer){
			writer.PutInt32(MapId);
			writer.PutInt32(Seed);
			writer.PutInt32(SimulationSpeed);
			writer.PutInt32(UserCount);
			writer.Put(UdpEnd);
			writer.PutArray(UserInfos);
        }
    
        public override void Deserialize(Deserializer reader){
			MapId = reader.GetInt32();
			Seed = reader.GetInt32();
			SimulationSpeed = reader.GetInt32();
			UserCount = reader.GetInt32();
			UdpEnd = reader.Get(ref this.UdpEnd);
			UserInfos = reader.GetArray(ref this.UserInfos);
        }
    }


    public partial class Msg_G2C_GameStatu{
        public override void Serialize(Serializer writer){
			writer.PutByte(Status);
        }
    
        public override void Deserialize(Deserializer reader){
			Status = reader.GetByte();
        }
    }


    public partial class Msg_G2C_Hello{
        public override void Serialize(Serializer writer){
			writer.PutByte(LocalId);
			writer.PutInt32(MapId);
        }
    
        public override void Deserialize(Deserializer reader){
			LocalId = reader.GetByte();
			MapId = reader.GetInt32();
        }
    }


    public partial class Msg_G2C_LoadingProgress{
        public override void Serialize(Serializer writer){
			writer.PutArray(Progress);
        }
    
        public override void Deserialize(Deserializer reader){
			Progress = reader.GetArray(ref this.Progress);
        }
    }


    public partial class Msg_HashCode{
        public override void Serialize(Serializer writer){
			writer.PutInt32(StartTick);
			writer.PutArray(HashCodes);
        }
    
        public override void Deserialize(Deserializer reader){
			StartTick = reader.GetInt32();
			HashCodes = reader.GetArray(ref this.HashCodes);
        }
    }


    public partial class Msg_I2C_LoginResult{
        public override void Serialize(Serializer writer){
			writer.PutString(LoginHash);
			writer.PutByte(LoginResult);
			writer.PutInt64(UserId);
			writer.Put(LobbyEnd);
        }
    
        public override void Deserialize(Deserializer reader){
			LoginHash = reader.GetString();
			LoginResult = reader.GetByte();
			UserId = reader.GetInt64();
			LobbyEnd = reader.Get(ref this.LobbyEnd);
        }
    }


    public partial class Msg_L2C_CreateRoom{
        public override void Serialize(Serializer writer){
			writer.Put(Info);
        }
    
        public override void Deserialize(Deserializer reader){
			Info = reader.Get(ref this.Info);
        }
    }


    public partial class Msg_L2C_RoomChangedInfo{
        public override void Serialize(Serializer writer){
			writer.PutArray(AddInfo);
			writer.PutArray(ChangedInfo);
			writer.PutArray(DeleteInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			AddInfo = reader.GetArray(ref this.AddInfo);
			ChangedInfo = reader.GetArray(ref this.ChangedInfo);
			DeleteInfo = reader.GetArray(ref this.DeleteInfo);
        }
    }


    public partial class Msg_L2C_RoomList{
        public override void Serialize(Serializer writer){
			writer.PutInt32(GameType);
			writer.PutArray(Rooms);
        }
    
        public override void Deserialize(Deserializer reader){
			GameType = reader.GetInt32();
			Rooms = reader.GetArray(ref this.Rooms);
        }
    }


    public partial class Msg_L2C_StartGame{
        public override void Serialize(Serializer writer){
			writer.PutString(GameHash);
			writer.PutByte(Result);
			writer.PutInt32(RoomId);
			writer.Put(GameServerEnd);
        }
    
        public override void Deserialize(Deserializer reader){
			GameHash = reader.GetString();
			Result = reader.GetByte();
			RoomId = reader.GetInt32();
			GameServerEnd = reader.Get(ref this.GameServerEnd);
        }
    }


    public partial class Msg_L2G_StartGame{
        public override void Serialize(Serializer writer){
			writer.PutString(GameHash);
			writer.PutInt32(GameType);
			writer.PutInt32(MapId);
			writer.PutArray(Players);
        }
    
        public override void Deserialize(Deserializer reader){
			GameHash = reader.GetString();
			GameType = reader.GetInt32();
			MapId = reader.GetInt32();
			Players = reader.GetArray(ref this.Players);
        }
    }


    public partial class Msg_RepAccountData{
        public override void Serialize(Serializer writer){
			writer.Put(accountData);
        }
    
        public override void Deserialize(Deserializer reader){
			accountData = reader.Get(ref this.accountData);
        }
    }


    public partial class Msg_RepCreateResult{
        public override void Serialize(Serializer writer){
			writer.PutByte(result);
			writer.PutInt64(userId);
        }
    
        public override void Deserialize(Deserializer reader){
			result = reader.GetByte();
			userId = reader.GetInt64();
        }
    }


    public partial class Msg_RepMissFrameAck{
        public override void Serialize(Serializer writer){
			writer.PutInt32(MissFrameTick);
        }
    
        public override void Deserialize(Deserializer reader){
			MissFrameTick = reader.GetInt32();
        }
    }


    public partial class Msg_ReqAccountData{
        public override void Serialize(Serializer writer){
			writer.PutString(account);
			writer.PutString(password);
        }
    
        public override void Deserialize(Deserializer reader){
			account = reader.GetString();
			password = reader.GetString();
        }
    }


    public partial class Msg_ReqMissFrame{
        public override void Serialize(Serializer writer){
			writer.PutInt32(StartTick);
        }
    
        public override void Deserialize(Deserializer reader){
			StartTick = reader.GetInt32();
        }
    }


    public partial class RoomChangedInfo{
        public override void Serialize(Serializer writer){
			writer.PutInt32(CurPlayerCount);
			writer.PutInt32(RoomId);
        }
    
        public override void Deserialize(Deserializer reader){
			CurPlayerCount = reader.GetInt32();
			RoomId = reader.GetInt32();
        }
    }


    public partial class RoomInfo{
        public override void Serialize(Serializer writer){
			writer.PutByte(CurPlayerCount);
			writer.PutInt32(GameType);
			writer.PutInt32(MapId);
			writer.PutByte(MaxPlayerCount);
			writer.PutString(Name);
			writer.PutString(OwnerName);
			writer.PutInt32(RoomId);
			writer.PutByte(State);
        }
    
        public override void Deserialize(Deserializer reader){
			CurPlayerCount = reader.GetByte();
			GameType = reader.GetInt32();
			MapId = reader.GetInt32();
			MaxPlayerCount = reader.GetByte();
			Name = reader.GetString();
			OwnerName = reader.GetString();
			RoomId = reader.GetInt32();
			State = reader.GetByte();
        }
    }


    public partial class UserGameInfo{
        public override void Serialize(Serializer writer){
			writer.PutString(Name);
			writer.PutArray(Data);
        }
    
        public override void Deserialize(Deserializer reader){
			Name = reader.GetString();
			Data = reader.GetArray(ref this.Data);
        }
    }


#endif
}
