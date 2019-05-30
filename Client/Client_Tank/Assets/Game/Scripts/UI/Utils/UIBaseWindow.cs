﻿using NetMsg.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game {
    public struct WindowCreateInfo {
        public string resDir;
        public EWindowDepth depth;

        public WindowCreateInfo(string dir, EWindowDepth dep){
            this.resDir = dir;
            this.depth = dep;
        }
    }
    public class UIDefine {

        public static WindowCreateInfo UIRoot = new WindowCreateInfo(  "UIRoot",EWindowDepth.Normal);
        public static WindowCreateInfo UILogin = new WindowCreateInfo( "UILogin",EWindowDepth.Normal);
        public static WindowCreateInfo UILobby = new WindowCreateInfo( "UILobby",EWindowDepth.Normal);
        public static WindowCreateInfo UICreateRoom = new WindowCreateInfo( "UICreateTeam",EWindowDepth.Normal);
        public static WindowCreateInfo UIRoomList = new WindowCreateInfo( "UIGameList",EWindowDepth.Normal);
        public static WindowCreateInfo UINetStatus = new WindowCreateInfo( "UINetStatus",EWindowDepth.Normal);

        //common
        public static WindowCreateInfo UIDialogBox = new WindowCreateInfo( "UIDialogBox",EWindowDepth.Forward);
        public static WindowCreateInfo UILoading = new WindowCreateInfo( "UILoading",EWindowDepth.Notice);
    }

    public static class UIExtension {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component{
            var comp = obj.GetComponent<T>();
            if (comp != null) return comp;
            return obj.AddComponent<T>();
        }
    }

    public class UIBaseWindow : MonoBehaviour {
        public IUIService uiService;
        public string ResPath { get; set; }
        public void Close(){uiService.CloseWindow(this); }
        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void OnClose(){ }


        protected void OpenWindow(WindowCreateInfo windowInfo){
            uiService.OpenWindow(windowInfo);
        }

        protected void SendMessage(EMsgCL type, object body){ }
    }
}