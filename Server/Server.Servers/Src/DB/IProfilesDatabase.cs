using System;
using Lockstep.Serialization;

namespace Lockstep.Server.Database
{
    public class PlayerProfile:BaseFormater {
        public string Username;

        public void FromBytes(byte[] data){
            var reader = new Deserializer(data);
            Deserialize(reader);
        }

        public byte[] ToBytes(){
            var writer = new Serializer();
            Serialize(writer);
            return writer.CopyData();
        }
    }

    /// <summary>
    /// Represents generic database for profiles
    /// </summary>
    public interface IProfilesDatabase
    {
        /// <summary>
        /// Should restore all values of the given profile, 
        /// or not change them, if there's no entry in the database
        /// </summary>
        /// <returns></returns>
        void RestoreProfile(PlayerProfile profile, Action doneCallback );

        /// <summary>
        /// Should save updated profile into database
        /// </summary>
        void UpdateProfile(PlayerProfile profile, Action doneCallback);
    }
}