using System;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    public class UIDialogBox : UIBaseWindow {
        public Text TextTitle;
        public Text TextContent;
        public Button BtnYes;
        public Button BtnNo;

        public Action<bool> callbackYesNo;

        public Action callbackYes;

        public void Start(){
            BtnYes.onClick.AddListener(OnBtn_Yes);
            BtnNo.onClick.AddListener(OnBtn_No);
        }

        public void Init(string title, string content, Action callback){
            TextContent.text = content;
            TextTitle.text = title;
            BtnNo.gameObject.SetActive(false);
            BtnYes.gameObject.SetActive(true);
            this.callbackYes = callback;
        }

        public void Init(string title, string content, Action<bool> onBtnClick){
            TextContent.text = content;
            TextTitle.text = title;
            this.callbackYesNo = onBtnClick;
            BtnNo.gameObject.SetActive(true);
            BtnYes.gameObject.SetActive(true);
        }

        public void OnBtn_Yes(){
            CallBack(false);
        }

        public void OnBtn_No(){
            CallBack(false);
        }

        void CallBack(bool val){
            callbackYesNo?.Invoke(val);
            callbackYesNo = null;
            callbackYes?.Invoke();
            callbackYes = null;
            Close();
        }
    }
}