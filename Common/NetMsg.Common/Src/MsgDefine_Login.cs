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
        string Email { get; set; }
        string Token { get; set; }
        bool IsAdmin { get; set; }
        bool IsGuest { get; set; }

        bool IsEmailConfirmed { get; set; }
        //Dictionary<string, string> Properties { get; set; }

        event Action<IAccountData> OnChange;
        void MarkAsDirty();
    }

    public partial class AccountData : BaseFormater, IAccountData {
        [BsonId]
        public string Username { get; set; }
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
    }
}