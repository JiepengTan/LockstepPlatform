using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.Game {
    public interface IConstStateService : IService {

        bool IsVideoLoading { get; set; }
        bool IsVideoMode { get; set; }
        bool IsRunVideo { get; set; }
        
        /// 是否正在重连
        bool IsReconnecting { get; set; }

        /// 是否正在追帧
        bool IsPursueFrame { get; set; }

        int CurLevel { get; set; }
    }
}