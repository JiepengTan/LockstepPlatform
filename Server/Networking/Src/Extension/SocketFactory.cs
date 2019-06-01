namespace Lockstep.Networking {
    public class SocketFactory {
        public static IClientSocket CreateClientSocket(){
            return new ClientSocketWs();
        }

        public static IServerSocket CreateServerSocket(){
            return new ServerSocketWs();
        }

        public static IClientSocket CreateClientUdpSocket(){
            return new ClientSocketWs();
        }

        public static IServerSocket CreateServerUdpSocket(){
            return new ServerSocketWs();
        }
    }
}