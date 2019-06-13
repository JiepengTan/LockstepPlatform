using NetMsg.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lockstep.Game {

    public class UIBaseWindow : MonoBehaviour {
        public IUIService uiService;
        public string ResPath { get; set; }

        public void Close(){
            uiService.CloseWindow(this);
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
            uiService.OpenWindow(windowInfo);
        }

        protected void SendMessage(EMsgSC type, object body){ }
    }
}