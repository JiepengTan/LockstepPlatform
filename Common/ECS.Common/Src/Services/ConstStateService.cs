using Entitas;

namespace Lockstep.Game {
    public partial class ConstStateService : BaseService, IConstStateService {
        public static ConstStateService Instance { get; private set; }

        public ConstStateService(){
            Instance = this;
        }

        public bool IsVideoLoading { get; set; }
        public bool IsVideoMode { get; set; }
        public bool IsRunVideo { get; set; }
        public bool IsReconnecting { get; set; }

        public bool IsPursueFrame { get; set; }


        public int CurLevel { get; set; }
        public IContexts Contexts { get; set; }
        public int SnapshotFrameInterval { get; set; }
    }
}