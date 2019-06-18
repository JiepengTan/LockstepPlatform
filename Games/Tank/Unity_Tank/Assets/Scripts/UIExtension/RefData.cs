using System;
using UnityEngine;

namespace Lockstep.Game.UI {
    [System.Serializable]
    public class RefData {
        public RefData(string name, EComponentType eComponent,object bindVal = null){
            this.name = name;
            this.bindObj = null;
            this.bindVal = bindVal;
            this.TypeName = eComponent.ToString();
        }

        public RefData(string name, EComponentType eComponent,UnityEngine.Object bindObj){
            this.name = name;
            this.bindObj = bindObj;
            this.bindVal = null;
            this.TypeName = eComponent.ToString();
        }

        public string name;
        public UnityEngine.Object bindObj;
        public object bindVal;
        public string typeName = "";
        public bool hasVal = false;

        public string TypeName {
            get => typeName;
            set {
                typeName = value;
                SetParams(value);
            }
        }

        public UnityEngine.Object GetBindObj(){
            return bindObj;
        }

        private void SetParams(string typeName){
            EComponentType type = (EComponentType) Enum.Parse(typeof(EComponentType), typeName);
            if (type < EComponentType.GameObject) {
                bindObj = GetGameObject().GetComponent(typeName);
            }
        }

        public GameObject GetGameObject(){
            GameObject gameObj = null;
            if (bindObj is Component com) {
                gameObj = com.gameObject;
            }
            else if (bindObj is GameObject pGo) {
                gameObj = pGo;
            }

            return gameObj;
        }
    }
}