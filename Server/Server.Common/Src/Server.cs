using System.Collections.Generic;
using Lockstep.Serialization;

namespace Lockstep.Server.Common {
    public class Server : BaseServer {
        private List<IServerProxy> slaveServers = new List<IServerProxy>();
        private Queue<byte[]> pendingSyncMsgs = new Queue<byte[]>();
        private IServerProxy masterServer;
        private IServerProxy candidateMasterServer;
        private IDaemonServer daemonServer;

        public void RequireMoreSlave(){
            daemonServer.ReqStartServer(serverType);
        }

        public void BorderToSlaves(EServerMsg type, BaseFormater data){
            var writer = new Serializer();
            writer.PutByte((byte) EServerMsg.MasterToSlave);
            writer.PutByte((byte) type);
            data.Serialize(writer);
            var bytes = writer.CopyData();
            candidateMasterServer.SendMsg(bytes);
        }

        public void SendToCandidate(EServerMsg type, BaseFormater data){
            var writer = new Serializer();
            writer.PutByte((byte) EServerMsg.MasterToCandidate);
            writer.PutByte((byte) type);
            data.Serialize(writer);
            var bytes = writer.CopyData();
            candidateMasterServer.SendMsg(bytes);
        }

        public void OnRecvServerMsg(byte[] data){
            var reader = new Deserializer(data);
            var masterType = (EServerMsg) reader.GetByte();
            var type = (EServerMsg) reader.GetByte();
            if (masterType == EServerMsg.SlaveToMaster) {
                if (IsMaster) {
                    if (candidateMasterServer != null) {
                        while (pendingSyncMsgs.Count > 0) {
                            var msg = pendingSyncMsgs.Dequeue();
                            candidateMasterServer.SendMsg(data);
                        }

                        candidateMasterServer.SendMsg(data); //同步给备份服务器 
                    }
                    else {
                        pendingSyncMsgs.Enqueue(data);
                    }
                }
            }

            DispatcherMsg(type, reader);
        }

        public virtual void DispatcherMsg(EServerMsg type, Deserializer reader){ }
    }
}