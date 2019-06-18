using System;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    public class UIDialogBox : UIBaseWindow {
        private Text TextTitle=> GetRef<Text>("TextTitle");
        private Text TextContent=> GetRef<Text>("TextContent");
        private Button BtnYes=> GetRef<Button>("BtnYes");
        private Button BtnNo=> GetRef<Button>("BtnNo");

        public Action<bool> callbackYesNo;
        public Action callbackYes;
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

        public void OnClick_BtnYes(){
            CallBack(false);
        }

        public void OnClick_BtnNo(){
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