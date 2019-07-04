//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace NetMsg.Common{
#if !DONT_USE_GENERATE_CODE

    [System.Serializable]
    public partial class AccountData{
        public override void Serialize(Serializer writer){
			writer.Write(Email);
			writer.Write(IsAdmin);
			writer.Write(IsEmailConfirmed);
			writer.Write(IsGuest);
			writer.Write(Password);
			writer.Write(Token);
			writer.Write(UserId);
			writer.Write(Username);
        }
    
        public override void Deserialize(Deserializer reader){
			Email = reader.ReadString();
			IsAdmin = reader.ReadBoolean();
			IsEmailConfirmed = reader.ReadBoolean();
			IsGuest = reader.ReadBoolean();
			Password = reader.ReadString();
			Token = reader.ReadString();
			UserId = reader.ReadInt64();
			Username = reader.ReadString();
        }
    }


    [System.Serializable]
    public partial class GameData{
        public override void Serialize(Serializer writer){
			writer.Write(_UserId);
			writer.Write(_Username);
			writer.Write(_Datas);
        }
    
        public override void Deserialize(Deserializer reader){
			_UserId = reader.ReadInt64();
			_Username = reader.ReadString();
			_Datas = reader.ReadList(this._Datas);
        }
    }


    [System.Serializable]
    public partial class GamePlayerInfo{
        public override void Serialize(Serializer writer){
			writer.Write(Account);
			writer.Write(LoginHash);
			writer.Write(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			Account = reader.ReadString();
			LoginHash = reader.ReadString();
			UserId = reader.ReadInt64();
        }
    }


    [System.Serializable]
    public partial class GameProperty{
        public override void Serialize(Serializer writer){
			writer.Write(_Name);
			writer.Write(_Type);
			writer.Write(_Data);
        }
    
        public override void Deserialize(Deserializer reader){
			_Name = reader.ReadString();
			_Type = reader.ReadInt16();
			_Data = reader.ReadArray(this._Data);
        }
    }


    [System.Serializable]
    public partial class IPEndInfo{
        public override void Serialize(Serializer writer){
			writer.Write(Ip);
			writer.Write(Port);
        }
    
        public override void Deserialize(Deserializer reader){
			Ip = reader.ReadString();
			Port = reader.ReadUInt16();
        }
    }


    [System.Serializable]
    public partial class MessageHello{
        public override void Serialize(Serializer writer){
			writer.Write(GameHash);
			writer.Write(GameId);
			writer.Write(GameType);
			writer.Write(IsReconnect);
			writer.Write(UserInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			GameHash = reader.ReadString();
			GameId = reader.ReadInt32();
			GameType = reader.ReadInt32();
			IsReconnect = reader.ReadBoolean();
			UserInfo = reader.ReadRef(ref this.UserInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_C2G_GameEvent{
        public override void Serialize(Serializer writer){
			writer.Write(Data);
        }
    
        public override void Deserialize(Deserializer reader){
			Data = reader.ReadArray(this.Data);
        }
    }


    [System.Serializable]
    public partial class Msg_C2G_Hello{
        public override void Serialize(Serializer writer){
			writer.Write(Hello);
        }
    
        public override void Deserialize(Deserializer reader){
			Hello = reader.ReadRef(ref this.Hello);
        }
    }


    [System.Serializable]
    public partial class Msg_C2G_LoadingProgress{
        public override void Serialize(Serializer writer){
			writer.Write(Progress);
        }
    
        public override void Deserialize(Deserializer reader){
			Progress = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class Msg_C2G_UdpHello{
        public override void Serialize(Serializer writer){
			writer.Write(Hello);
        }
    
        public override void Deserialize(Deserializer reader){
			Hello = reader.ReadRef(ref this.Hello);
        }
    }


    [System.Serializable]
    public partial class Msg_C2I_UserLogin{
        public override void Serialize(Serializer writer){
			writer.Write(Account);
			writer.Write(EncryptHash);
			writer.Write(GameType);
			writer.Write(Password);
        }
    
        public override void Deserialize(Deserializer reader){
			Account = reader.ReadString();
			EncryptHash = reader.ReadString();
			GameType = reader.ReadInt32();
			Password = reader.ReadString();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_CreateRoom{
        public override void Serialize(Serializer writer){
			writer.Write(GameType);
			writer.Write(MapId);
			writer.Write(MaxPlayerCount);
			writer.Write(Name);
        }
    
        public override void Deserialize(Deserializer reader){
			GameType = reader.ReadInt32();
			MapId = reader.ReadInt32();
			MaxPlayerCount = reader.ReadByte();
			Name = reader.ReadString();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_JoinRoom{
        public override void Serialize(Serializer writer){
			writer.Write(RoomId);
        }
    
        public override void Deserialize(Deserializer reader){
			RoomId = reader.ReadInt32();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_LeaveRoom{
        public override void Serialize(Serializer writer){
			writer.Write(Reason);
        }
    
        public override void Deserialize(Deserializer reader){
			Reason = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_ReadyInRoom{
        public override void Serialize(Serializer writer){
			writer.Write(State);
        }
    
        public override void Deserialize(Deserializer reader){
			State = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_ReqRoomList{
        public override void Serialize(Serializer writer){
			writer.Write(StartIdx);
        }
    
        public override void Deserialize(Deserializer reader){
			StartIdx = reader.ReadInt16();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_RoomChatInfo{
        public override void Serialize(Serializer writer){
			writer.Write(ChatInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ChatInfo = reader.ReadRef(ref this.ChatInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_StartGame{
        public override void Serialize(Serializer writer){
			writer.Write(Reason);
        }
    
        public override void Deserialize(Deserializer reader){
			Reason = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class Msg_C2L_UserLogin{
        public override void Serialize(Serializer writer){
			writer.Write(LoginHash);
			writer.Write(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			LoginHash = reader.ReadString();
			UserId = reader.ReadInt64();
        }
    }


    [System.Serializable]
    public partial class Msg_CreateAccount{
        public override void Serialize(Serializer writer){
			writer.Write(account);
			writer.Write(password);
        }
    
        public override void Deserialize(Deserializer reader){
			account = reader.ReadString();
			password = reader.ReadString();
        }
    }


    [System.Serializable]
    public partial class Msg_D2S_RepGameData{
        public override void Serialize(Serializer writer){
			writer.Write(data);
        }
    
        public override void Deserialize(Deserializer reader){
			data = reader.ReadRef(ref this.data);
        }
    }


    [System.Serializable]
    public partial class Msg_D2S_SaveGameData{
        public override void Serialize(Serializer writer){
			writer.Write(result);
        }
    
        public override void Deserialize(Deserializer reader){
			result = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_AllFinishedLoaded{
        public override void Serialize(Serializer writer){
			writer.Write(Level);
        }
    
        public override void Deserialize(Deserializer reader){
			Level = reader.ReadInt16();
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_GameEvent{
        public override void Serialize(Serializer writer){
			writer.Write(Data);
        }
    
        public override void Deserialize(Deserializer reader){
			Data = reader.ReadArray(this.Data);
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_GameStartInfo{
        public override void Serialize(Serializer writer){
			writer.Write(MapId);
			writer.Write(RoomId);
			writer.Write(Seed);
			writer.Write(SimulationSpeed);
			writer.Write(UserCount);
			writer.Write(TcpEnd);
			writer.Write(UdpEnd);
			writer.Write(UserInfos);
        }
    
        public override void Deserialize(Deserializer reader){
			MapId = reader.ReadInt32();
			RoomId = reader.ReadInt32();
			Seed = reader.ReadInt32();
			SimulationSpeed = reader.ReadInt32();
			UserCount = reader.ReadByte();
			TcpEnd = reader.ReadRef(ref this.TcpEnd);
			UdpEnd = reader.ReadRef(ref this.UdpEnd);
			UserInfos = reader.ReadArray(this.UserInfos);
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_GameStatu{
        public override void Serialize(Serializer writer){
			writer.Write(Status);
        }
    
        public override void Deserialize(Deserializer reader){
			Status = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_Hello{
        public override void Serialize(Serializer writer){
			writer.Write(GameId);
			writer.Write(LocalId);
			writer.Write(MapId);
			writer.Write(Seed);
			writer.Write(UserCount);
			writer.Write(UdpEnd);
        }
    
        public override void Deserialize(Deserializer reader){
			GameId = reader.ReadInt32();
			LocalId = reader.ReadByte();
			MapId = reader.ReadInt32();
			Seed = reader.ReadInt32();
			UserCount = reader.ReadByte();
			UdpEnd = reader.ReadRef(ref this.UdpEnd);
        }
    }


    [System.Serializable]
    public partial class Msg_G2C_LoadingProgress{
        public override void Serialize(Serializer writer){
			writer.Write(Progress);
        }
    
        public override void Deserialize(Deserializer reader){
			Progress = reader.ReadArray(this.Progress);
        }
    }


    [System.Serializable]
    public partial class Msg_G2L_OnGameFinished{
        public override void Serialize(Serializer writer){
			writer.Write(GameId);
			writer.Write(RoomId);
        }
    
        public override void Deserialize(Deserializer reader){
			GameId = reader.ReadInt32();
			RoomId = reader.ReadInt32();
        }
    }


    [System.Serializable]
    public partial class Msg_HashCode{
        public override void Serialize(Serializer writer){
			writer.Write(StartTick);
			writer.Write(HashCodes);
        }
    
        public override void Deserialize(Deserializer reader){
			StartTick = reader.ReadInt32();
			HashCodes = reader.ReadArray(this.HashCodes);
        }
    }


    [System.Serializable]
    public partial class Msg_I2C_LoginResult{
        public override void Serialize(Serializer writer){
			writer.Write(LoginHash);
			writer.Write(LoginResult);
			writer.Write(UserId);
			writer.Write(LobbyEnd);
        }
    
        public override void Deserialize(Deserializer reader){
			LoginHash = reader.ReadString();
			LoginResult = reader.ReadByte();
			UserId = reader.ReadInt64();
			LobbyEnd = reader.ReadRef(ref this.LobbyEnd);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_CreateRoom{
        public override void Serialize(Serializer writer){
			writer.Write(Info);
			writer.Write(PlayerInfos);
        }
    
        public override void Deserialize(Deserializer reader){
			Info = reader.ReadRef(ref this.Info);
			PlayerInfos = reader.ReadArray(this.PlayerInfos);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_JoinRoom{
        public override void Serialize(Serializer writer){
			writer.Write(PlayerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			PlayerInfo = reader.ReadRef(ref this.PlayerInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_JoinRoomResult{
        public override void Serialize(Serializer writer){
			writer.Write(PlayerInfos);
        }
    
        public override void Deserialize(Deserializer reader){
			PlayerInfos = reader.ReadArray(this.PlayerInfos);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_LeaveRoom{
        public override void Serialize(Serializer writer){
			writer.Write(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			UserId = reader.ReadInt64();
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_ReadyInRoom{
        public override void Serialize(Serializer writer){
			writer.Write(State);
			writer.Write(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			State = reader.ReadByte();
			UserId = reader.ReadInt64();
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_RoomChatInfo{
        public override void Serialize(Serializer writer){
			writer.Write(ChatInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			ChatInfo = reader.ReadRef(ref this.ChatInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_RoomInfoUpdate{
        public override void Serialize(Serializer writer){
			writer.Write(AddInfo);
			writer.Write(ChangedInfo);
			writer.Write(DeleteInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			AddInfo = reader.ReadArray(this.AddInfo);
			ChangedInfo = reader.ReadArray(this.ChangedInfo);
			DeleteInfo = reader.ReadArray(this.DeleteInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_RoomList{
        public override void Serialize(Serializer writer){
			writer.Write(GameType);
			writer.Write(Rooms);
        }
    
        public override void Deserialize(Deserializer reader){
			GameType = reader.ReadInt32();
			Rooms = reader.ReadArray(this.Rooms);
        }
    }


    [System.Serializable]
    public partial class Msg_L2C_StartGame{
        public override void Serialize(Serializer writer){
			writer.Write(GameHash);
			writer.Write(GameId);
			writer.Write(IsReconnect);
			writer.Write(Result);
			writer.Write(RoomId);
			writer.Write(GameServerEnd);
        }
    
        public override void Deserialize(Deserializer reader){
			GameHash = reader.ReadString();
			GameId = reader.ReadInt32();
			IsReconnect = reader.ReadBoolean();
			Result = reader.ReadByte();
			RoomId = reader.ReadInt32();
			GameServerEnd = reader.ReadRef(ref this.GameServerEnd);
        }
    }


    [System.Serializable]
    public partial class Msg_L2G_CreateGame{
        public override void Serialize(Serializer writer){
			writer.Write(GameHash);
			writer.Write(GameType);
			writer.Write(MapId);
			writer.Write(RoomId);
			writer.Write(Players);
        }
    
        public override void Deserialize(Deserializer reader){
			GameHash = reader.ReadString();
			GameType = reader.ReadInt32();
			MapId = reader.ReadInt32();
			RoomId = reader.ReadInt32();
			Players = reader.ReadArray(this.Players);
        }
    }


    [System.Serializable]
    public partial class Msg_L2G_UserLeave{
        public override void Serialize(Serializer writer){
			writer.Write(GameId);
			writer.Write(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			GameId = reader.ReadInt32();
			UserId = reader.ReadInt64();
        }
    }


    [System.Serializable]
    public partial class Msg_L2G_UserReconnect{
        public override void Serialize(Serializer writer){
			writer.Write(PlayerInfo);
        }
    
        public override void Deserialize(Deserializer reader){
			PlayerInfo = reader.ReadRef(ref this.PlayerInfo);
        }
    }


    [System.Serializable]
    public partial class Msg_RepAccountData{
        public override void Serialize(Serializer writer){
			writer.Write(accountData);
        }
    
        public override void Deserialize(Deserializer reader){
			accountData = reader.ReadRef(ref this.accountData);
        }
    }


    [System.Serializable]
    public partial class Msg_RepCreateResult{
        public override void Serialize(Serializer writer){
			writer.Write(result);
			writer.Write(userId);
        }
    
        public override void Deserialize(Deserializer reader){
			result = reader.ReadByte();
			userId = reader.ReadInt64();
        }
    }


    [System.Serializable]
    public partial class Msg_RepMissFrameAck{
        public override void Serialize(Serializer writer){
			writer.Write(MissFrameTick);
        }
    
        public override void Deserialize(Deserializer reader){
			MissFrameTick = reader.ReadInt32();
        }
    }


    [System.Serializable]
    public partial class Msg_ReqAccountData{
        public override void Serialize(Serializer writer){
			writer.Write(account);
			writer.Write(password);
        }
    
        public override void Deserialize(Deserializer reader){
			account = reader.ReadString();
			password = reader.ReadString();
        }
    }


    [System.Serializable]
    public partial class Msg_ReqMissFrame{
        public override void Serialize(Serializer writer){
			writer.Write(StartTick);
        }
    
        public override void Deserialize(Deserializer reader){
			StartTick = reader.ReadInt32();
        }
    }


    [System.Serializable]
    public partial class Msg_S2C_TickPlayer{
        public override void Serialize(Serializer writer){
			writer.Write(Reason);
        }
    
        public override void Deserialize(Deserializer reader){
			Reason = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class Msg_S2D_ReqGameData{
        public override void Serialize(Serializer writer){
			writer.Write(account);
        }
    
        public override void Deserialize(Deserializer reader){
			account = reader.ReadString();
        }
    }


    [System.Serializable]
    public partial class Msg_S2D_SaveGameData{
        public override void Serialize(Serializer writer){
			writer.Write(data);
        }
    
        public override void Deserialize(Deserializer reader){
			data = reader.ReadRef(ref this.data);
        }
    }


    [System.Serializable]
    public partial class RoomChangedInfo{
        public override void Serialize(Serializer writer){
			writer.Write(CurPlayerCount);
			writer.Write(RoomId);
        }
    
        public override void Deserialize(Deserializer reader){
			CurPlayerCount = reader.ReadByte();
			RoomId = reader.ReadInt32();
        }
    }


    [System.Serializable]
    public partial class RoomChatInfo{
        public override void Serialize(Serializer writer){
			writer.Write(Channel);
			writer.Write(DstUserId);
			writer.Write(SrcUserId);
			writer.Write(Message);
        }
    
        public override void Deserialize(Deserializer reader){
			Channel = reader.ReadByte();
			DstUserId = reader.ReadInt64();
			SrcUserId = reader.ReadInt64();
			Message = reader.ReadArray(this.Message);
        }
    }


    [System.Serializable]
    public partial class RoomInfo{
        public override void Serialize(Serializer writer){
			writer.Write(CurPlayerCount);
			writer.Write(GameType);
			writer.Write(MapId);
			writer.Write(MaxPlayerCount);
			writer.Write(Name);
			writer.Write(OwnerName);
			writer.Write(RoomId);
			writer.Write(State);
        }
    
        public override void Deserialize(Deserializer reader){
			CurPlayerCount = reader.ReadByte();
			GameType = reader.ReadInt32();
			MapId = reader.ReadInt32();
			MaxPlayerCount = reader.ReadByte();
			Name = reader.ReadString();
			OwnerName = reader.ReadString();
			RoomId = reader.ReadInt32();
			State = reader.ReadByte();
        }
    }


    [System.Serializable]
    public partial class RoomPlayerInfo{
        public override void Serialize(Serializer writer){
			writer.Write(Name);
			writer.Write(Status);
			writer.Write(UserId);
        }
    
        public override void Deserialize(Deserializer reader){
			Name = reader.ReadString();
			Status = reader.ReadByte();
			UserId = reader.ReadInt64();
        }
    }


    [System.Serializable]
    public partial class UserGameInfo{
        public override void Serialize(Serializer writer){
			writer.Write(Name);
			writer.Write(Data);
        }
    
        public override void Deserialize(Deserializer reader){
			Name = reader.ReadString();
			Data = reader.ReadArray(this.Data);
        }
    }


#endif
}
