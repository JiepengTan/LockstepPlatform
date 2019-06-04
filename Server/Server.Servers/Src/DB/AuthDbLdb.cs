#if (!UNITY_WEBGL && !UNITY_IOS) || UNITY_EDITOR
using System;
using System.Collections.Generic;
using LiteDB;
using Lockstep.Serialization;
using NetMsg.Common;

namespace Lockstep.Server.Database
{   

    public interface IAuthDatabase
    {
        /// <summary>
        ///     Should create an empty object with account data.
        /// </summary>
        /// <returns></returns>
        IAccountData CreateAccountObject();

        void GetAccount(string username, Action<IAccountData> callback);
        void GetAccountByToken(string token, Action<IAccountData> callback);
        void GetAccountByEmail(string email, Action<IAccountData> callback);

        void SavePasswordResetCode(IAccountData account, string code, Action doneCallback );
        void GetPasswordResetData(string email, Action<IPasswordResetData> callback);

        void SaveEmailConfirmationCode(string email, string code, Action doneCallback);
        void GetEmailConfirmationCode(string email, Action<string> callback);

        void UpdateAccount(IAccountData account, Action doneCallback);
        void InsertNewAccount(IAccountData account, Action doneCallback);
        void InsertToken(IAccountData account, string token, Action doneCallback);
    }
    public class AuthDbLdb : IAuthDatabase
    {
        private readonly LiteCollection<AccountData> _accounts;
        private readonly LiteCollection<PasswordResetData> _resetCodes;
        private readonly LiteCollection<EmailConfirmationData> _emailConfirmations;

        private readonly LiteDatabase _db;

        public AuthDbLdb(LiteDatabase database)
        {
            _db = database;

            _accounts = _db.GetCollection<AccountData>("accounts");
            _accounts.EnsureIndex(a => a.Username, new IndexOptions() { Unique = true, IgnoreCase = true, TrimWhitespace = true});
            _accounts.EnsureIndex(a => a.Email, new IndexOptions() { Unique = true, IgnoreCase = true, TrimWhitespace = true });

            _resetCodes = _db.GetCollection<PasswordResetData>("resets");
            _resetCodes.EnsureIndex(a => a.Email, new IndexOptions() {Unique = true, IgnoreCase = true, TrimWhitespace = true});

            _emailConfirmations = _db.GetCollection<EmailConfirmationData>("emailConf");
            _emailConfirmations.EnsureIndex(a => a.Email, new IndexOptions() { Unique = true, IgnoreCase = true, TrimWhitespace = true });
        }

        public IAccountData CreateAccountObject()
        {
            var account = new AccountData();
            return account;
        }

        public void GetAccount(string username, Action<IAccountData> callback)
        {
            var account = _accounts.FindOne(a => a.Username == username);

            callback.Invoke(account);
        }

        public void GetAccountByToken(string token, Action<IAccountData> callback)
        {
            var account = _accounts.FindOne(a => a.Token == token);

            callback.Invoke(account);
        }

        public void GetAccountByEmail(string email, Action<IAccountData> callback)
        {
            var emailLower = email.ToLower();
            var account = _accounts.FindOne(Query.EQ("Email", emailLower));

            callback.Invoke(account);
        }

        public void SavePasswordResetCode(IAccountData account, string code, Action doneCallback )
        {
            // Delete older codes
            _resetCodes.Delete(Query.EQ("Email", account.Email.ToLower()));

            _resetCodes.Insert(new PasswordResetData()
            {
                Email = account.Email,
                Code = code
            });

            doneCallback.Invoke();
        }

        public void GetPasswordResetData(string email, Action<IPasswordResetData> callback )
        {
            var code = _resetCodes.FindOne(Query.EQ("Email", email.ToLower()));
            callback.Invoke(code);
        }

        public void SaveEmailConfirmationCode(string email, string code, Action doneCallback )
        {
            _emailConfirmations.Delete(Query.EQ("Email", email));
            _emailConfirmations.Insert(new EmailConfirmationData()
            {
                Code = code,
                Email = email
            });

            doneCallback.Invoke();
        }

        public void GetEmailConfirmationCode(string email, Action<string> callback)
        {
            var entry = _emailConfirmations.FindOne(Query.EQ("Email", email));

            callback.Invoke(entry != null ? entry.Code : null);
        }

        public void UpdateAccount(IAccountData account, Action doneCallback)
        {
            _accounts.Update(account as AccountData);

            doneCallback.Invoke();
        }

        public void InsertNewAccount(IAccountData account, Action doneCallback)
        {
            _accounts.Insert(account as AccountData);
            doneCallback.Invoke();
        }

        public void InsertToken(IAccountData account, string token, Action doneCallback)
        {
            account.Token = token;
            _accounts.Update(account as AccountData);

            doneCallback.Invoke();
        }

        private class PasswordResetData : IPasswordResetData
        {
            public string Email { get; set; }
            public string Code { get; set; }
        }

        private class EmailConfirmationData
        {
            public string Email { get; set; }
            public string Code { get; set; }
        }
    }
}

#endif