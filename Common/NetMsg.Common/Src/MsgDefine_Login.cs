using System;
using System.Collections.Generic;
using LiteDB;
using Lockstep.Serialization;

namespace NetMsg.Common {
    public interface IPasswordResetData {
        string Email { get; set; }
        string Code { get; set; }
    }

    /// <summary>
    ///     Represents account data
    /// </summary>
    public interface IAccountData {
        string Username { get; set; }
        string Password { get; set; }
        long UserId { get; set; }
        string Email { get; set; }
        string Token { get; set; }
        bool IsAdmin { get; set; }
        bool IsGuest { get; set; }

        bool IsEmailConfirmed { get; set; }
        //Dictionary<string, string> Properties { get; set; }

        event Action<IAccountData> OnChange;
        void MarkAsDirty();
    }

    public partial class GameProperty : BaseFormater {
        public string Name { get; set; }
        public byte Type { get; set; }
        public byte[] Data{ get; set; }
    }

    public partial class GameData : BaseFormater {
        [BsonId] public string Username { get; set; }
        public long UserId { get; set; }
        public List<GameProperty> Datas { get; set; }
    }

    public partial class AccountData : BaseFormater, IAccountData {
        [BsonId] public string Username { get; set; }
        public long UserId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsGuest { get; set; }

        public bool IsEmailConfirmed { get; set; }
        //public Dictionary<string, string> Properties { get; set; }

        public event Action<IAccountData> OnChange;
        public void MarkAsDirty(){ }
    }

    public partial class Msg_S2D_ReqGameData : BaseFormater {
        public string account;
    }
    public partial class Msg_D2S_RepGameData : BaseFormater {
        public GameData data;
    }
    public partial class Msg_S2D_SaveGameData : BaseFormater {
        public GameData data;
    }
    public partial class Msg_D2S_SaveGameData : BaseFormater {
        public byte result;
    }
    public partial class Msg_ReqAccountData : BaseFormater {
        public string account;
        public string password;
    }

    public partial class Msg_CreateAccount : BaseFormater {
        public string account;
        public string password;
    }

    public partial class Msg_RepAccountData : BaseFormater {
        public AccountData accountData;
    }

    public partial class Msg_RepCreateResult : BaseFormater {
        public byte result;
        public long userId;
    }
}