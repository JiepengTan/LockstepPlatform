using Lockstep.Core;
using NetMsg.Common;

using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI    
{
    /// <summary>
    ///     Represents a basic view for login form
    /// </summary>
    public class UILogin : UIBaseWindow
    {
        public Text ErrorText;
        public Button LoginButton;
        public InputField Password;
        public Toggle Remember;
        public InputField Username;

        protected string RememberPrefKey = "lp.auth.remember";
        protected string UsernamePrefKey = "lp.auth.username";

        protected virtual void Awake()
        {
            ErrorText = ErrorText ?? transform.Find("Error").GetComponent<Text>();
            LoginButton = LoginButton ?? transform.Find("Button").GetComponent<Button>();
            Password = Password ?? transform.Find("Password").GetComponent<InputField>();
            Remember = Remember ?? transform.Find("Remember").GetComponent<Toggle>();
            Username = Username ?? transform.Find("Username").GetComponent<InputField>();
            LoginButton.onClick.AddListener(OnLoginClick);
            ErrorText.gameObject.SetActive(false);
        }

        // Use this for initialization
        private void Start()
        {
            RestoreRememberedValues();
        }

        private void OnEnable()
        {
            gameObject.transform.localPosition = Vector3.zero;
        }

        protected void OnLoggedIn()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        ///     Tries to restore previously held values
        /// </summary>
        protected virtual void RestoreRememberedValues()
        {
            Username.text = PlayerPrefs.GetString(UsernamePrefKey, Username.text);
            Remember.isOn = PlayerPrefs.GetInt(RememberPrefKey, -1) > 0;
        }

        /// <summary>
        ///     Checks if inputs are valid
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateInput()
        {
            var error = "";

            if (Username.text.Length < 3)
                error += "Username is too short \n";

            if (Password.text.Length < 3)
                error += "Password is too short \n";

            if (error.Length > 0)
            {
                // We've got an error
                error = error.Remove(error.Length - 1);
                ShowError(error);
                return false;
            }

            return true;
        }

        protected void ShowError(string message)
        {
            ErrorText.gameObject.SetActive(true);
            ErrorText.text = message;
        }

        /// <summary>
        ///     Called after clicking login button
        /// </summary>
        protected virtual void HandleRemembering()
        {
            if (!Remember.isOn)
            {
                // Remember functionality is off. Delete all values
                PlayerPrefs.DeleteKey(UsernamePrefKey);
                PlayerPrefs.DeleteKey(RememberPrefKey);
                return;
            }

            // Remember is on
            PlayerPrefs.SetString(UsernamePrefKey, Username.text);
            PlayerPrefs.SetInt(RememberPrefKey, 1);
        }

        public virtual void OnLoginClick()
        {
            //SendMessage(EMsgSC.C2L_ReqLogin,new Msg_RoomInitMsg() {name = Username.text});
            HandleRemembering();
            EventHelper.Trigger(EEvent.TryLogin,new LoginParam() {
                account = Username.text,
                password =  Password.text
            });

        }

        public void OnEvent_OnLoginResult(object param){
            OpenWindow(UIDefine.UILobby);
            Close();
        }

        public virtual void OnPasswordForgotClick()
        {
        }
    }
}