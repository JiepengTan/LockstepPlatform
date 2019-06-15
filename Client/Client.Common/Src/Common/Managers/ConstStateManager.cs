using NaughtyAttributes;
using UnityEngine;

namespace Lockstep.Game {
    public partial class ConstStateManager : SingletonManager<ConstStateManager>, IConstStateService{

        [SerializeField] public bool IsVideoLoading { get; set; }
        [SerializeField] public bool IsVideoMode { get; set; }
        [SerializeField] public bool IsRunVideo { get; set; }
        [ShowNativeProperty] public bool IsReconnecting { get; set; }

        [ShowNativeProperty] public bool IsPursueFrame { get; set; }

        //game info 
        [ShowNativeProperty] public int CurLevel { get; set; }
    }
}