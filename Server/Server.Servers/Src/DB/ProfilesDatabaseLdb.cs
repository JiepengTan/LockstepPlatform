#if (!UNITY_WEBGL && !UNITY_IOS) || UNITY_EDITOR
using System;
using LiteDB;
using Lockstep.Serialization;

namespace Lockstep.Server.Database {
    /// <summary>
    /// LiteDB profiles database implementation
    /// </summary>
    public class ProfilesDatabaseLdb : IProfilesDatabase {
        private readonly LiteCollection<ProfileDataLdb> _profiles;
        private readonly LiteDatabase _db;

        public ProfilesDatabaseLdb(LiteDatabase database){
            _db = database;

            _profiles = _db.GetCollection<ProfileDataLdb>("profiles");
            _profiles.EnsureIndex(a => a.Username, new IndexOptions() {Unique = true});
        }

        /// <summary>
        /// Should restore all values of the given profile, 
        /// or not change them, if there's no entry in the database
        /// </summary>
        /// <returns></returns>
        public void RestoreProfile(PlayerProfile profile, Action doneCallback){
            var data = FindOrCreateData(profile);
            var reader = new Deserializer(data.Data);
            profile.FromBytes(data.Data);
            doneCallback.Invoke();
        }

        private ProfileDataLdb FindOrCreateData(PlayerProfile profile){
            var data = _profiles.FindOne(a => a.Username == profile.Username);

            if (data == null) {
                data = new ProfileDataLdb() {
                    Username = profile.Username,
                    Data = profile.ToBytes()
                };

                // Why did I do this?
                _profiles.Insert(data);
            }

            return data;
        }

        /// <summary>
        /// Should save updated profile into database
        /// </summary>
        public void UpdateProfile(PlayerProfile profile, Action doneCallback){
            var data = FindOrCreateData(profile);
            data.Data = profile.ToBytes();
            _profiles.Update(data);

            doneCallback.Invoke();
        }

        /// <summary>
        /// LiteDB profile data implementation
        /// </summary>
        private class ProfileDataLdb {
            [BsonId] public string Username { get; set; }

            public byte[] Data { get; set; }
        }
    }
}

#endif