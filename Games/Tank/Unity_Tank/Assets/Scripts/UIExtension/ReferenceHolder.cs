using System.Collections.Generic;
using UnityEngine;
using Lockstep.Game.UI;
using Lockstep.Math;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Lockstep.Game.UI {
    [System.Serializable]
    public enum EComponentType {
        ScrollRect,
        Button,
        Image,
        Text,
        Tab,
        Toggle,
        Dropdown,
        InputField,
        LayoutGroup,
        RawImage,
        Camera,
        CanvasGroup,
        Slider,
        RectTransform,
        Transform,
        GameObject,
        Number,
        Color,
        EnumCount,
    }


#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ReferenceHolder : MonoBehaviour, IReferenceHolder {
        [SerializeField] private Dictionary<string, Object> _name2Objs;
        [SerializeField] private Dictionary<string, object> _name2Vals;
        public List<RefData> Datas = new List<RefData>();

        public T GetRef<T>(string name) where T : Object{
            if (_name2Objs != null && _name2Objs.TryGetValue(name, out Object val)) {
                return val as T;
            }

            Debug.Log("Miss Ref " + name);
            return null;
        }

        public LFloat GetVal(string name){
            if (_name2Vals.TryGetValue(name, out object val)) {
                return (LFloat) val;
            }

            return LFloat.zero;
        }

        public Color GetColor(string name){
            if (_name2Vals.TryGetValue(name, out object val)) {
                return (Color) val;
            }

            return Color.white;
        }

        void Awake(){
            foreach (var item in Datas) {
                if (item.bindObj != null) {
                    if (_name2Objs == null) _name2Objs = new Dictionary<string, Object>();
                    _name2Objs.Add(item.name, item.bindObj);
                }
                else {
                    if (_name2Vals == null) _name2Vals = new Dictionary<string, object>();
                    _name2Vals.Add(item.name, item.bindVal);
                }
            }
        }

        public void OnDestroy(){
            if (Application.isPlaying) {
                Datas = null;
            }

            _name2Objs = null;
            _name2Vals = null;
        }
    }
}