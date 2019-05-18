namespace Lockstep.Game {
    public partial class ManagerReferenceHolder {
        public AudioManager audioMgr;
        public InputManager inputMgr;
        public LevelManager levelMgr;
        public GameManager gameMgr;
        public UIManager uiMgr;
        public SimulationManager simulationMgr;
        public NetworkManager networkMgr;

        public void AssignReference(IManagerContainer mgrContainer){
            audioMgr = mgrContainer.GetManager<AudioManager>();
            inputMgr = mgrContainer.GetManager<InputManager>();
            levelMgr = mgrContainer.GetManager<LevelManager>();
            gameMgr = mgrContainer.GetManager<GameManager>();
            uiMgr = mgrContainer.GetManager<UIManager>();
            simulationMgr = mgrContainer.GetManager<SimulationManager>();
            networkMgr = mgrContainer.GetManager<NetworkManager>();
        }
    }
}