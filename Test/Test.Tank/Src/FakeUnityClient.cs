using Lockstep.Game;
using Lockstep.Networking;

namespace Test {
    public class FakeUnityClient : NetworkEntity {
        private GameConfig __Game;
        private EcsFacade __ECS;
        
        public Launcher launcher = new Launcher();
        public override void DoAwake(){
            base.DoAwake();
            launcher.DoAwake(null);
        }

        public override void DoStart(){
            base.DoStart();
            launcher.DoStart();
        }

        public override void DoDestroy(){
            base.DoDestroy();
            launcher.DoDestroy();
        }

        public override void DoUpdate(int deltaTimeMs){
            base.DoUpdate(deltaTimeMs);
            launcher.DoUpdate(deltaTimeMs);
        }

        public override void PollEvents(){
            base.PollEvents();
        }
        private void OnApplicationQuit(){
            launcher.OnApplicationQuit();
        }

    }
}