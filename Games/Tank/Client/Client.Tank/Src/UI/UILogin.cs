using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    /// <summary>
    ///     Represents a basic view for login form
    /// </summary>
    public class UILogin : UIBaseWindow {
        private Text TextError => GetRef<Text>("TextError");
        private Button BtnLogin => GetRef<Button>("BtnLogin");
        private Toggle TglRemember => GetRef<Toggle>("TglRemember");
        private InputField InputPassword => GetRef<InputField>("InputPassword");
        private InputField InputUsername => GetRef<InputField>("InputUsername");

        protected string RememberPrefKey = "lp.auth.remember";
        protected string UsernamePrefKey = "lp.auth.username";

        protected override void Awake(){
            base.Awake();
            TextError.gameObject.SetActive(false);
        }

        // Use this for initialization
        private void Start(){
            RestoreRememberedValues();
        }

        public void OnEvent_OnConnLogin(object param){
            RestoreRememberedValues();
#if UNITY_EDITOR
            if (_uiService.IsDebugMode) {
                OnClick_BtnLogin();
            }
#endif
        }
        
        private void OnEnable(){
            gameObject.transform.localPosition = Vector3.zero;
        }

        protected void OnLoggedIn(){
            gameObject.SetActive(false);
        }

        /// <summary>
        ///     Tries to restore previously held values
        /// </summary>
        protected virtual void RestoreRememberedValues(){
            InputUsername.text = PlayerPrefs.GetString(UsernamePrefKey, InputUsername.text);
            TglRemember.isOn = PlayerPrefs.GetInt(RememberPrefKey, -1) > 0;
        }

        /// <summary>
        ///     Checks if inputs are valid
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateInput(){
            var error = "";

            if (InputUsername.text.Length < 3)
                error += "Username is too short \n";

            if (InputPassword.text.Length < 3)
                error += "InputPassword is too short \n";

            if (error.Length > 0) {
                // We've got an error
                error = error.Remove(error.Length - 1);
                ShowError(error);
                return false;
            }

            return true;
        }

        protected void ShowError(string message){
            TextError.gameObject.SetActive(true);
            TextError.text = message;
        }

        /// <summary>
        ///     Called after clicking login button
        /// </summary>
        protected virtual void HandleRemembering(){
            if (!TglRemember.isOn) {
                // Remember functionality is off. Delete all values
                PlayerPrefs.DeleteKey(UsernamePrefKey);
                PlayerPrefs.DeleteKey(RememberPrefKey);
                return;
            }

            // Remember is on
            PlayerPrefs.SetString(UsernamePrefKey, InputUsername.text);
            PlayerPrefs.SetInt(RememberPrefKey, 1);
        }

        public virtual void OnClick_BtnLogin(){
            //SendMessage(EMsgSC.C2L_ReqLogin,new Msg_RoomInitMsg() {name = Username.text});
            HandleRemembering();
            EventHelper.Trigger(EEvent.TryLogin, new LoginParam() {
                account = InputUsername.text,
                password = InputPassword.text
            });
        }

        public void OnEvent_OnLoginResult(object param){
            OpenWindow(UIDefine.UIRoomList);
            Close();
        }

        public void OnEvent_OnConnectToGameServer(object param){
            OpenWindow(UIDefine.UILoading);
            Close();
        }

        public virtual void OnInputPasswordForgotClick(){ }
    }
}