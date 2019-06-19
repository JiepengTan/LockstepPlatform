using System;

namespace Lockstep.Game {
    public class PureUIService : PureBaseService, IUIService {
        public T GetIService<T>() where T : IService{return GetService<T>();}
        public void OpenWindow<T>(string dir, EWindowDepth dep, UICallback callback = null){ }
        public void CloseWindow(string dir){ }
        public void CloseWindow(object window = null){ }
        public void ShowDialog(string title, string body, Action<bool> resultCallback){ }
    }
}