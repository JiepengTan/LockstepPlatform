using Lockstep.Server.Common;

namespace Lockstep.Server.Game {
    public interface IGameServer  {
       // Player GetPlayer(long playerId);

       void OnGameEmpty(IGame game);

        //players
       void TickOut(Player player, int reason);

        //Net
       // void OnClientConnected(object peer);
       // void OnCilentDisconnected(object peer);
    }
}