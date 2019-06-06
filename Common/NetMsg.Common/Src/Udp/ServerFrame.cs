using Lockstep.Serialization;

namespace NetMsg.Common {
    [System.Serializable]
    [SelfImplement]
    [Udp]
    public partial class ServerFrame : BaseFormater {
        public byte[] InputDatas; //包含玩家的输入& 游戏输入
        public int Tick;
        public Msg_PlayerInput[] _inputs;

        public Msg_PlayerInput[] Inputs {
            get { return _inputs; }
            set {
                _inputs = value;
                InputDatas = null;
            }
        }

        private byte[] _serverInputs;
        
        public byte[] ServerInputs {//服务器输入 如掉落等
            get { return _serverInputs; }
            set {
                _serverInputs = value;
                InputDatas = null;
            }
        }

        public void BeforeSerialize(){
            if (InputDatas != null) return;
            var writer = new Serializer();
            var inputLen = (byte) (Inputs?.Length ?? 0);
            writer.PutByte(inputLen);
            for (byte i = 0; i < inputLen; i++) {
                var cmds = Inputs[i].Commands;
                var len = (byte) (cmds?.Length ?? 0);
                writer.PutByte(len);
                for (int cmdIdx = 0; cmdIdx < len; cmdIdx++) {
                    cmds[cmdIdx].Serialize(writer);
                }
            }

            writer.PutBytes_255(_serverInputs);
            InputDatas = writer.CopyData();
        }

        public void AfterDeserialize(){
            var reader = new Deserializer(InputDatas);
            var inputLen = reader.GetByte();
            _inputs = new Msg_PlayerInput[inputLen];
            for (byte i = 0; i < inputLen; i++) {
                var input = new Msg_PlayerInput();
                input.Tick = Tick;
                input.ActorId = i;
                _inputs[i] = input;
                var len = reader.GetByte();
                if (len == 0) {
                    input.Commands = null;
                    continue;
                }

                input.Commands = new InputCmd[len];
                for (int cmdIdx = 0; cmdIdx < len; cmdIdx++) {
                    var cmd = new InputCmd();
                    cmd.Deserialize(reader);
                    input.Commands[cmdIdx] = cmd;
                }
            }

            _serverInputs = reader.GetBytes_255();
        }

        public override void Serialize(Serializer writer){
            BeforeSerialize();
            writer.PutInt32(Tick);
            writer.PutBytes(InputDatas);
        }

        public override void Deserialize(Deserializer reader){
            Tick = reader.GetInt32();
            InputDatas = reader.GetBytes();
            AfterDeserialize();
        }

        public override string ToString(){
            var count = (InputDatas == null) ? 0 : InputDatas.Length;
            return
                $"t:{Tick} " +
                $"inputNum:{count}";
        }

        public override bool Equals(object obj){
            if (obj == null) return false;
            var frame = obj as ServerFrame;
            return Equals(frame);
        }

        public override int GetHashCode(){
            return Tick;
        }

        public bool Equals(ServerFrame frame){
            if (frame == null) return false;
            if (Tick != frame.Tick) return false;
            BeforeSerialize();
            frame.BeforeSerialize();
            return InputDatas.EqualsEx(frame.InputDatas);
        }
    }
}