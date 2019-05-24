using System;
using Lockstep.Serialization;

namespace NetMsg.Game {
    public partial class Msg_RepMissFrame : BaseFormater {
        public ServerFrame[] frames;

        public override void Serialize(Serializer writer){
            writer.Put(frames.Length);
            for (int i = 0; i < frames.Length; i++) {
                try { 
                    frames[i].Serialize(writer);}
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }

        public override void Deserialize(Deserializer reader){
            var len = reader.GetInt();
            frames = new ServerFrame[len];
            for (int i = 0; i < len; i++) {
                frames[i] = new ServerFrame();
                frames[i].Deserialize(reader);
            }
        }
    }
}