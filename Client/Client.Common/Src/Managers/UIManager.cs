using System;
using System.Collections.Generic;
using System.Linq;
using Lockstep.Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game {
    [System.Serializable]
    public class UIManager : BaseManager, IUIService {
        private const string _prefabDir = "UI/";
        [SerializeField] private Transform _uiRoot;
        private Dictionary<string, UIBaseWindow> _windowPool = new Dictionary<string, UIBaseWindow>();
        private UIRoot uiRoot;
        private Transform normalParent;
        private Transform forwardParent;
        private Transform importParent;

        public  T GetService<T>() where T : IService{
            return _serviceContainer.GetService<T>();
        }

        public override void DoStart(){
            var canvas = GameObject.Find("Canvas");
            var prefab = Resources.Load<GameObject>(_prefabDir + UIDefine.UIRoot.resDir);
            if (prefab == null) {
                Debug.LogError("Can not load UIRoot !" + UIDefine.UIRoot.resDir);
            }

            var uiGo = GameObject.Instantiate(prefab, canvas.transform);
            uiRoot = uiGo.GetComponent<UIRoot>();
            normalParent = uiRoot.normalParent;
            forwardParent = uiRoot.forwardParent;
            importParent = uiRoot.noticeParent;
            if (_constStateService.IsVideoMode) {
                OpenWindow(UIDefine.UILoading);
            }
            else {
                OpenWindow(UIDefine.UILogin);
            }
        }

        private Transform GetParentFromDepth(EWindowDepth depth){
            switch (depth) {
                case EWindowDepth.Normal: return normalParent;
                case EWindowDepth.Notice: return forwardParent;
                case EWindowDepth.Forward: return importParent;
            }

            return uiRoot.transform;
        }

        protected void OnEvent_OnTickPlayer(object param){
            ShowDialog("Message", "OnTickPlayer reason" + param, () => {
                foreach (var window in openedWindows.ToArray()) {
                    window.Close();
                }

                OpenWindow(UIDefine.UILogin);
            });
        }

        protected void OnEvent_OnLoginFailed(object param){
            ShowDialog("Message", "Login failed " + param.ToString());
        }

        #region interfaces

        public void ShowDialog(string title, string body, Action<bool> resultCallback){
            OpenWindow(UIDefine.UIDialogBox,
                (window) => { (window as UIDialogBox)?.Init(title, body, resultCallback); });
        }

        public void ShowDialog(string title, string body, Action resultCallback = null){
            OpenWindow(UIDefine.UIDialogBox,
                (window) => { (window as UIDialogBox)?.Init(title, body, resultCallback); });
        }

        public void OpenWindow(WindowCreateInfo info, UICallback callback = null){
            OpenWindow(info.resDir, info.depth, callback);
        }

        public void CloseWindow(UIBaseWindow window){
            if (window != null) {
                //unbind Msgs
                window.OnClose();
                openedWindows.Remove(window);
                _eventRegisterService.UnRegisterEvent(window);
                if (_windowPool.ContainsKey(window.ResPath)) {
                    GameObject.Destroy(window.gameObject);
                }
                else {
                    window.gameObject.SetActive(false);
                    _windowPool[window.ResPath] = window;
                }
            }
        }

        //TODO
        public void CloseWindow(string dir){ }

        //TODO
        public void CloseWindow(object window = null){ }

        #endregion

        public void OpenWindow(string resPath, EWindowDepth depth, UICallback callback = null){
            OpenWindow(resPath, GetParentFromDepth(depth), callback);
        }

        private HashSet<UIBaseWindow> openedWindows = new HashSet<UIBaseWindow>();

        private void OpenWindow(string resPath, Transform parent, UICallback callback){
            UIBaseWindow window = null;
            if (_windowPool.TryGetValue(resPath, out var win)) {
                win.gameObject.SetActive(true);
                window = win;
                _windowPool.Remove(resPath);
            }
            else {
                var prefab = Resources.Load<GameObject>(_prefabDir + resPath);
                if (prefab == null) {
                    Logging.Debug.LogError("OpenWindow failed: can not find prefab" + resPath);
                    callback?.Invoke(null);
                    return;
                }

                var uiGo = GameObject.Instantiate(prefab, parent);
                window = uiGo.GetOrAddComponent<UIBaseWindow>();
                window.ResPath = resPath;
            }

            openedWindows.Add(window);
            window.DoAwake();
            window.DoStart();
            window.uiService = this;
            _eventRegisterService.RegisterEvent(window);
            callback?.Invoke(window);
        }
    }
}