using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lockstep.Game.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lockstep.Game {
    [System.Serializable]
    public class UnityUIService : UnityBaseService, IUIService {
        public RenderTexture rt;
        private const string _prefabDir = "UI/";
        [SerializeField] private Transform _uiRoot;
        private Dictionary<string, UIBaseWindow> _windowPool = new Dictionary<string, UIBaseWindow>();
        private UIRoot uiRoot;
        private Transform normalParent;
        private Transform forwardParent;
        private Transform importParent;

        public T GetIService<T>() where T : IService{
            return GetService<T>();
        }

        public override void DoStart(){
            var canvas = GameObject.Find("Canvas");
            var prefab = Resources.Load<GameObject>(_prefabDir + UIDefine.UIRoot.resDir);
            if (prefab == null) {
                Debug.LogError("Can not load UIRoot !" + UIDefine.UIRoot.resDir);
            }

            var uiGo = GameObject.Instantiate(prefab, canvas.transform);
            uiRoot = uiGo.GetOrAddComponent<UIRoot>();
            normalParent = uiRoot.TransNormal;
            forwardParent = uiRoot.TransForward;
            importParent = uiRoot.TransNotice;
            if (_constStateService.IsVideoMode) {
                OpenWindow<UILoading>(UIDefine.UILoading);
            }
            else {
                OpenWindow<UILogin>(UIDefine.UILogin);
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

                OpenWindow<UILogin>(UIDefine.UILogin);
            });
        }

        protected void OnEvent_OnLoginFailed(object param){
            ShowDialog("Message", "Login failed " + param.ToString());
        }

        #region interfaces

        public void ShowDialog(string title, string body, Action<bool> resultCallback){
            OpenWindow<UIDialogBox>(UIDefine.UIDialogBox,
                (window) => { (window as UIDialogBox)?.Init(title, body, resultCallback); });
        }

        public void ShowDialog(string title, string body, Action resultCallback = null){
            OpenWindow<UIDialogBox>(UIDefine.UIDialogBox,
                (window) => { (window as UIDialogBox)?.Init(title, body, resultCallback); });
        }
         public void OpenWindow<T>(WindowCreateInfo info, UICallback callback = null)where T:UIBaseWindow{
            OpenWindow<T>(info.resDir, info.depth, callback);
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

        public void OpenWindow<T>(string resPath, EWindowDepth depth, UICallback callback = null){
            OpenWindow(typeof(T),resPath, GetParentFromDepth(depth), callback);
        }

        private HashSet<UIBaseWindow> openedWindows = new HashSet<UIBaseWindow>();

        private void OpenWindow(Type type, string resPath, Transform parent, UICallback callback){
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
                window = uiGo.GetOrAddComponent<UIBaseWindow>(type);
                window.ResPath = resPath;
            }

            openedWindows.Add(window);
            window.uiService = this;
            window.DoAwake();
            window.DoStart();
            _eventRegisterService.RegisterEvent(window);
            RegisterUiEvent(window);
            callback?.Invoke(window);
        }

        public void RegisterUiEvent(object obj){
            RegisterUiEvent<UnityAction>("OnClick_", "OnClick_".Length, RegisterEventButton, obj);
            RegisterUiEvent<UnityAction<bool>>("OnToggle_", "OnToggle_".Length, RegisterEventToggle, obj);
            RegisterUiEvent<UnityAction<int>>("OnSelect_", "OnSelect_".Length, RegisterEventDropdown, obj);
        }

        public void RegisterUiEvent<TDelegate>(string prefix, int ignorePrefixLen,
            Action<object, string, TDelegate> callBack, object obj)
            where TDelegate : Delegate{
            if (callBack == null) return;
            var methods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                   BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                var methodName = method.Name;
                if (methodName.StartsWith(prefix)) {
                    var eventTypeStr = methodName.Substring(ignorePrefixLen);
                    try {
                        var handler = Delegate.CreateDelegate(typeof(TDelegate), obj, method) as TDelegate;
                        callBack(obj, eventTypeStr, handler);
                    }
                    catch (Exception e) {
                        Debug.LogError("methodName " + methodName);
                        throw;
                    }
                }
            }
        }
        void RegisterEventDropdown(object objWin, string compName, UnityAction<int> action){
            var window = (UIBaseWindow) objWin;
            var comp = window.GetRef<Dropdown>(compName);
            if (comp != null) {
                comp.onValueChanged.RemoveListener(action);
                comp.onValueChanged.AddListener(action);
            }
            else {
                Logging.Debug.Log(window.GetType() + " miss ref " + compName);
            }
        }
        void RegisterEventToggle(object objWin, string compName, UnityAction<bool> action){
            var window = (UIBaseWindow) objWin;
            var comp = window.GetRef<Toggle>(compName);
            if (comp != null) {
                comp.onValueChanged.RemoveListener(action);
                comp.onValueChanged.AddListener(action);
            }
            else {
                Logging.Debug.Log(window.GetType() + " miss ref " + compName);
            }
        }

        void RegisterEventButton(object objWin, string compName, UnityAction action){
            var window = (UIBaseWindow) objWin;
            var comp = window.GetRef<Button>(compName);
            if (comp != null) {
                comp.onClick.RemoveListener(action);
                comp.onClick.AddListener(action);
            }
            else {
                Logging.Debug.Log(window.GetType() + " miss ref " + compName);
            }
        }
    }
}