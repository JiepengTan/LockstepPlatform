using System;
using System.Collections.Generic;
using Lockstep.Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game {
    public enum EWindowDepth {
        Normal,
        Notice,
        Forward,
    }

    public interface IUIService : IService {
        void OpenWindow(WindowCreateInfo info, UICallback callback = null);
        void CloseWindow(UIBaseWindow window);
    }

    public delegate void UICallback(UIBaseWindow windowObj);

    [System.Serializable]
    public class UIManager : SingletonManager<UIManager>, IUIService {
        private const string _prefabDir = "UI/";
        [SerializeField] private Transform _uiRoot;
        private Dictionary<string, UIBaseWindow> _windowPool = new Dictionary<string, UIBaseWindow>();
        private UIRoot uiRoot;
        private Transform normalParent;
        private Transform forwardParent;
        private Transform importParent;

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
            OpenWindow(UIDefine.UILogin);
        }

        private Transform GetParentFromDepth(EWindowDepth depth){
            switch (depth) {
                case EWindowDepth.Normal: return normalParent;
                case EWindowDepth.Notice: return forwardParent;
                case EWindowDepth.Forward: return importParent;
            }

            return uiRoot.transform;
        }


        #region interfaces

        public void OpenWindow(WindowCreateInfo info, UICallback callback = null){
            OpenWindow(info.resDir, info.depth, callback);
        }

        public void CloseWindow(UIBaseWindow window){
            if (window != null) {
                //unbind Msgs
                window.OnClose();
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

        #endregion

        private void OpenWindow(string resPath, EWindowDepth depth, UICallback callback = null){
            OpenWindow(resPath, GetParentFromDepth(depth), callback);
        }

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

            window.DoAwake();
            window.DoStart();
            window.uiService = this;
            _eventRegisterService.RegisterEvent(window);
            callback?.Invoke(window);
        }
    }
}