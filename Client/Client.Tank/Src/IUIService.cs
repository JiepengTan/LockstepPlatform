
using System;

namespace Lockstep.Game {
    public enum EWindowDepth {
        Normal,
        Notice,
        Forward,
    }   
    public struct WindowCreateInfo {
        public string resDir;
        public EWindowDepth depth;

        public WindowCreateInfo(string dir, EWindowDepth dep){
            this.resDir = dir;
            this.depth = dep;
        }
    }

    public delegate void UICallback(UIBaseWindow windowObj);
    public interface IUIService : IService {
        void OpenWindow(WindowCreateInfo info, UICallback callback = null);
        void CloseWindow(UIBaseWindow window);
        void ShowDialog(string title, string body, Action<bool> resultCallback);
    }

}