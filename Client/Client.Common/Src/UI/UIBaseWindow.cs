using Lockstep.Game.UI;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public class UIBaseWindow : MonoBehaviour {
        public IUIService _uiService;
        public string ResPath { get; set; }
        protected IReferenceHolder _referenceHolder;

        public  T GetRef<T>(string name) where T : UnityEngine.Object{
            return _referenceHolder.GetRef<T>(name);
        }

        protected virtual void Awake(){
            _referenceHolder = GetComponent<IReferenceHolder>();
            Debug.Assert(_referenceHolder != null, GetType() + " miss IReferenceHolder ");
        }

        public void Close(){
            _uiService.CloseWindow(this);
        }

        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void OnClose(){ }

        protected Button BindEvent(string name, UnityAction func){
            var btn = transform.Find(name).GetComponent<Button>();
            btn.onClick.AddListener(func);
            return btn;
        }

        protected void OpenWindow(WindowCreateInfo windowInfo){
            _uiService.OpenWindow(windowInfo.resDir, windowInfo.depth);
        }

        protected void SendMessage(EMsgSC type, object body){ }

        protected T GetService<T>() where T : IService{
            return _uiService.GetIService<T>();
        }
    }
}