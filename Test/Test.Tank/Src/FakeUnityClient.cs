using Lockstep;
using Lockstep.Game;
using Lockstep.Networking;

namespace Test {
    public class FakeUnityClient : NetworkEntity {
        private GameConfig __Game;//Client.Tank
        private EcsFacade __ECS;//ECS.Tank
        private BaseService _service;//ECS.Common
        private BaseGameService _baseGameService;//Common.Tank
        public Launcher launcher = new Launcher();//Client.Common
        public override void DoAwake(){
            base.DoAwake();
            launcher.RunMode = EPureModeType.Pure;
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