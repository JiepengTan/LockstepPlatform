//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace NetMsg.Common{
#if !DONT_USE_GENERATE_CODE

    [System.Serializable]
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


    [System.Serializable]
    public partial class GameData{
        public override void Serialize(Serializer writer){
			writer.PutInt64(_UserId);
			writer.PutString(_Username);
			writer.PutList(_Datas);
        }
    
        public override void Deserialize(Deserializer reader){
			_UserId = reader.GetInt64();
			_Username = reader.GetString();
			_Datas = reader.GetList(this._Datas);
        }
    }


    [System.Serializable]
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


    [System.Serializable]
    public partial class GameProperty{
        public override void Serialize(Serializer writer){
			writer.PutString(_Name);
			writer.PutInt16(_Type);
			writer.PutArray(_Data);
        }
    
        public override void Deserialize(Deserializer reader){
			_Name = reader.GetString();
			_Type = reader.GetInt16();
			_Data = reader.GetArray(this._Data);
        }
    }


    [System.Serializable]
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


    [System.Serializable]
    public partial class MessageHello{
        public override void Serialize(Serializer writer){
			writer.PutString(GameHash);
			writer.PutInt32(GameId);
			writer.PutInt32(GameType);
			writer.PutBoolean(IsReconnect);
			writer.Put(UserInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			GameHash = reader.GetString();
			GameId = reader.GetInt32();
			GameType = reader.GetInt32();
			IsReconnect = reader.GetBoolean();
			UserInfo = reader.Get(ref this.UserInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_C2G_GameEvent{
        public override void Serialize(Serializer writer){
			writer.PutArray(Data);
        }
    
        public override void Deserialize(Deserializer reader){
			Data = reader.GetArray(this.Data);
        }
    }


    [System.Serializable]
    public partial class Msg_C2G_Hello{
        public override void Serialize(Serializer writer){
			writer.Put(Hello);
        }
    
        public override void Deserialize(Deserializer reader){
			Hello = reader.Get(ref this.Hello);
        }
    }


    [System.Serializable]
    public partial class Msg_C2G_LoadingProgress{
        public override void Serialize(Serializer writer){
			writer.PutByte(Progress);
        }
    
        public override void Deserialize(Deserializer reader){
			Progress = reader.GetByte();
        }
    }


    [System.Serializable]
    public partial class Msg_C2G_UdpHello{
        public override void Serialize(Serializer writer){
			writer.Put(Hello);
        }
    
        public override void Deserialize(Deserializer reader){
			Hello = reader.Get(ref this.Hello);
        }
    }


    [System.Serializable]
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


    [System.Serializable]
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


    [System.Serializable]
    public partial class Msg_C2L_JoinRoom{
        public override void Serialize(Serializer writer){
			writer.PutInt32(RoomId);
        }
    
        public override void Deserialize(Deserializer reader){
			RoomId = reader.GetInt32();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_LeaveRoom{
        public override void Serialize(Serializer writer){
			writer.PutByte(Reason);
        }
    
        public override void Deserialize(Deserializer reader){
			Reason = reader.GetByte();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_ReadyInRoom{
        public override void Serialize(Serializer writer){
			writer.PutByte(State);
        }
    
        public override void Deserialize(Deserializer reader){
			State = reader.GetByte();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_ReqRoomList{
        public override void Serialize(Serializer writer){
			writer.PutInt16(StartIdx);
        }
    
        public override void Deserialize(Deserializer reader){
			StartIdx = reader.GetInt16();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_RoomChatInfo{
        public override void Serialize(Serializer writer){
			writer.Put(ChatInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ChatInfo = reader.Get(ref this.ChatInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_StartGame{
        public override void Serialize(Serializer writer){
			writer.PutByte(Reason);
        }
    
        public override void Deserialize(Deserializer reader){
			Reason = reader.GetByte();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_UserLogin{
        public override void Serialize(Serializer writer){
			writer.PutString(LoginHash);
			writer.PutInt64(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			LoginHash = reader.GetString();
			UserId = reader.GetInt64();
        }
    }


    [System.Serializable]
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


    [System.Serializable]
    public partial class Msg_D2S_RepGameData{
        public override void Serialize(Serializer writer){
			writer.Put(data);
        }
    
        public override void Deserialize(Deserializer reader){
			data = reader.Get(ref this.data);
        }
    }


    [System.Serializable]
    public partial class Msg_D2S_SaveGameData{
        public override void Serialize(Serializer writer){
			writer.PutByte(result);
        }
    
        public override void Deserialize(Deserializer reader){
			result = reader.GetByte();
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_AllFinishedLoaded{
        public override void Serialize(Serializer writer){
			writer.PutInt16(Level);
        }
    
        public override void Deserialize(Deserializer reader){
			Level = reader.GetInt16();
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_GameEvent{
        public override void Serialize(Serializer writer){
			writer.PutArray(Data);
        }
    
        public override void Deserialize(Deserializer reader){
			Data = reader.GetArray(this.Data);
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_GameStartInfo{
        public override void Serialize(Serializer writer){
			writer.PutInt32(MapId);
			writer.PutInt32(RoomId);
			writer.PutInt32(Seed);
			writer.PutInt32(SimulationSpeed);
			writer.PutByte(UserCount);
			writer.Put(TcpEnd);
			writer.Put(UdpEnd);
			writer.PutArray(UserInfos);
        }
    
        public override void Deserialize(Deserializer reader){
			MapId = reader.GetInt32();
			RoomId = reader.GetInt32();
			Seed = reader.GetInt32();
			SimulationSpeed = reader.GetInt32();
			UserCount = reader.GetByte();
			TcpEnd = reader.Get(ref this.TcpEnd);
			UdpEnd = reader.Get(ref this.UdpEnd);
			UserInfos = reader.GetArray(this.UserInfos);
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_GameStatu{
        public override void Serialize(Serializer writer){
			writer.PutByte(Status);
        }
    
        public override void Deserialize(Deserializer reader){
			Status = reader.GetByte();
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_Hello{
        public override void Serialize(Serializer writer){
			writer.PutInt32(GameId);
			writer.PutByte(LocalId);
			writer.PutInt32(MapId);
			writer.PutInt32(Seed);
			writer.PutByte(UserCount);
			writer.Put(UdpEnd);
        }
    
        public override void Deserialize(Deserializer reader){
			GameId = reader.GetInt32();
			LocalId = reader.GetByte();
			MapId = reader.GetInt32();
			Seed = reader.GetInt32();
			UserCount = reader.GetByte();
			UdpEnd = reader.Get(ref this.UdpEnd);
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_LoadingProgress{
        public override void Serialize(Serializer writer){
			writer.PutArray(Progress);
        }
    
        public override void Deserialize(Deserializer reader){
			Progress = reader.GetArray(this.Progress);
        }
    }


    [System.Serializable]
    public partial class Msg_G2L_OnGameFinished{
        public override void Serialize(Serializer writer){
			writer.PutInt32(GameId);
			writer.PutInt32(RoomId);
        }
    
        public override void Deserialize(Deserializer reader){
			GameId = reader.GetInt32();
			RoomId = reader.GetInt32();
        }
    }


    [System.Serializable]
    public partial class Msg_HashCode{
        public override void Serialize(Serializer writer){
			writer.PutInt32(StartTick);
			writer.PutArray(HashCodes);
        }
    
        public override void Deserialize(Deserializer reader){
			StartTick = reader.GetInt32();
			HashCodes = reader.GetArray(this.HashCodes);
        }
    }


    [System.Serializable]
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


    [System.Serializable]
    public partial class Msg_L2C_CreateRoom{
        public override void Serialize(Serializer writer){
			writer.Put(Info);
			writer.PutArray(PlayerInfos);
        }
    
        public override void Deserialize(Deserializer reader){
			Info = reader.Get(ref this.Info);
			PlayerInfos = reader.GetArray(this.PlayerInfos);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_JoinRoom{
        public override void Serialize(Serializer writer){
			writer.Put(PlayerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			PlayerInfo = reader.Get(ref this.PlayerInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_JoinRoomResult{
        public override void Serialize(Serializer writer){
			writer.PutArray(PlayerInfos);
        }
    
        public override void Deserialize(Deserializer reader){
			PlayerInfos = reader.GetArray(this.PlayerInfos);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_LeaveRoom{
        public override void Serialize(Serializer writer){
			writer.PutInt64(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			UserId = reader.GetInt64();
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_ReadyInRoom{
        public override void Serialize(Serializer writer){
			writer.PutByte(State);
			writer.PutInt64(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			State = reader.GetByte();
			UserId = reader.GetInt64();
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_RoomChatInfo{
        public override void Serialize(Serializer writer){
			writer.Put(ChatInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ChatInfo = reader.Get(ref this.ChatInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_RoomInfoUpdate{
        public override void Serialize(Serializer writer){
			writer.PutArray(AddInfo);
			writer.PutArray(ChangedInfo);
			writer.PutArray(DeleteInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			AddInfo = reader.GetArray(this.AddInfo);
			ChangedInfo = reader.GetArray(this.ChangedInfo);
			DeleteInfo = reader.GetArray(this.DeleteInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_RoomList{
        public override void Serialize(Serializer writer){
			writer.PutInt32(GameType);
			writer.PutArray(Rooms);
        }
    
        public override void Deserialize(Deserializer reader){
			GameType = reader.GetInt32();
			Rooms = reader.GetArray(this.Rooms);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_StartGame{
        public override void Serialize(Serializer writer){
			writer.PutString(GameHash);
			writer.PutInt32(GameId);
			writer.PutBoolean(IsReconnect);
			writer.PutByte(Result);
			writer.PutInt32(RoomId);
			writer.Put(GameServerEnd);
        }
    
        public override void Deserialize(Deserializer reader){
			GameHash = reader.GetString();
			GameId = reader.GetInt32();
			IsReconnect = reader.GetBoolean();
			Result = reader.GetByte();
			RoomId = reader.GetInt32();
			GameServerEnd = reader.Get(ref this.GameServerEnd);
        }
    }


    [System.Serializable]
    public partial class Msg_L2G_CreateGame{
        public override void Serialize(Serializer writer){
			writer.PutString(GameHash);
			writer.PutInt32(GameType);
			writer.PutInt32(MapId);
			writer.PutInt32(RoomId);
			writer.PutArray(Players);
        }
    
        public override void Deserialize(Deserializer reader){
			GameHash = reader.GetString();
			GameType = reader.GetInt32();
			MapId = reader.GetInt32();
			RoomId = reader.GetInt32();
			Players = reader.GetArray(this.Players);
        }
    }


    [System.Serializable]
    public partial class Msg_L2G_UserLeave{
        public override void Serialize(Serializer writer){
			writer.PutInt32(GameId);
			writer.PutInt64(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			GameId = reader.GetInt32();
			UserId = reader.GetInt64();
        }
    }


    [System.Serializable]
    public partial class Msg_L2G_UserReconnect{
        public override void Serialize(Serializer writer){
			writer.Put(PlayerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			PlayerInfo = reader.Get(ref this.PlayerInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_RepAccountData{
        public override void Serialize(Serializer writer){
			writer.Put(accountData);
        }
    
        public override void Deserialize(Deserializer reader){
			accountData = reader.Get(ref this.accountData);
        }
    }


    [System.Serializable]
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


    [System.Serializable]
    public partial class Msg_RepMissFrameAck{
        public override void Serialize(Serializer writer){
			writer.PutInt32(MissFrameTick);
        }
    
        public override void Deserialize(Deserializer reader){
			MissFrameTick = reader.GetInt32();
        }
    }


    [System.Serializable]
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


    [System.Serializable]
    public partial class Msg_ReqMissFrame{
        public override void Serialize(Serializer writer){
			writer.PutInt32(StartTick);
        }
    
        public override void Deserialize(Deserializer reader){
			StartTick = reader.GetInt32();
        }
    }


    [System.Serializable]
    public partial class Msg_S2C_TickPlayer{
        public override void Serialize(Serializer writer){
			writer.PutByte(Reason);
        }
    
        public override void Deserialize(Deserializer reader){
			Reason = reader.GetByte();
        }
    }


    [System.Serializable]
    public partial class Msg_S2D_ReqGameData{
        public override void Serialize(Serializer writer){
			writer.PutString(account);
        }
    
        public override void Deserialize(Deserializer reader){
			account = reader.GetString();
        }
    }


    [System.Serializable]
    public partial class Msg_S2D_SaveGameData{
        public override void Serialize(Serializer writer){
			writer.Put(data);
        }
    
        public override void Deserialize(Deserializer reader){
			data = reader.Get(ref this.data);
        }
    }


    [System.Serializable]
    public partial class RoomChangedInfo{
        public override void Serialize(Serializer writer){
			writer.PutByte(CurPlayerCount);
			writer.PutInt32(RoomId);
        }
    
        public override void Deserialize(Deserializer reader){
			CurPlayerCount = reader.GetByte();
			RoomId = reader.GetInt32();
        }
    }


    [System.Serializable]
    public partial class RoomChatInfo{
        public override void Serialize(Serializer writer){
			writer.PutByte(Channel);
			writer.PutInt64(DstUserId);
			writer.PutInt64(SrcUserId);
			writer.PutArray(Message);
        }
    
        public override void Deserialize(Deserializer reader){
			Channel = reader.GetByte();
			DstUserId = reader.GetInt64();
			SrcUserId = reader.GetInt64();
			Message = reader.GetArray(this.Message);
        }
    }


    [System.Serializable]
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


    [System.Serializable]
    public partial class RoomPlayerInfo{
        public override void Serialize(Serializer writer){
			writer.PutString(Name);
			writer.PutByte(Status);
			writer.PutInt64(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			Name = reader.GetString();
			Status = reader.GetByte();
			UserId = reader.GetInt64();
        }
    }


    [System.Serializable]
    public partial class UserGameInfo{
        public override void Serialize(Serializer writer){
			writer.PutString(Name);
			writer.PutArray(Data);
        }
    
        public override void Deserialize(Deserializer reader){
			Name = reader.GetString();
			Data = reader.GetArray(this.Data);
        }
    }


#endif
}
