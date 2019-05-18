using UnityEngine;

namespace Lockstep.Game {
    public partial class ManagerReferenceHolder {
       [HideInInspector] public AudioManager audioMgr;
       [HideInInspector] public InputManager inputMgr;
       [HideInInspector] public LevelManager levelMgr;
       [HideInInspector] public GameManager gameMgr;
       [HideInInspector] public UIManager uiMgr;
       [HideInInspector] public SimulationManager simulationMgr;
       [HideInInspector] public NetworkManager networkMgr;

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