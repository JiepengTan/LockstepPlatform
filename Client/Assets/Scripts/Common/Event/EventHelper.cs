#define DEBUG_EVENT_TRIGGER
#if UNITY_EDITOR || DEBUG_EVENT_TRIGGER
#define _DEBUG_EVENT_TRIGGER
#endif

using System.Collections.Generic;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Core {

    public enum EEvent {
        LoadMapDone,
        OnRoomGameStart,
        OnAllPlayerFinishedLoad,
        OnServerFrame
    }

    public class EventHelper {

        public delegate void OnEventHandler(object param);
        public static Dictionary<int,List<OnEventHandler>> allListeners = new Dictionary<int, List<OnEventHandler>>();
        private static bool IsTriggingEvent;
        
        public static void RemoveAllListener(EEvent type){
            allListeners.Remove((int) type);
        }

        public static void AddListener(EEvent type, OnEventHandler listener){
#if _DEBUG_EVENT_TRIGGER 
            if (IsTriggingEvent) { Debug.LogError("Error!!! can not modify allListeners when was Trigger Event");}
#endif

            var itype  = (int)type;
            if(allListeners.TryGetValue(itype,out var tmplst)) {
                tmplst.Add(listener);
            }
            else {
                var lst = new List<OnEventHandler>();
                lst.Add(listener);
                allListeners.Add(itype,lst);
            }
        }
        public static void RemoveListener(EEvent type, OnEventHandler listener){
#if _DEBUG_EVENT_TRIGGER
            if (IsTriggingEvent) { Debug.LogError("Error!!! can not modify allListeners when was Trigger Event");}
#endif
            var itype  = (int)type;
            if(allListeners.TryGetValue(itype,out var tmplst)) {
                if (tmplst.Remove(listener)) {
                    if (tmplst.Count == 0) {
                        allListeners.Remove(itype);
                    }
                    return;
                }
            }
            Debug.LogError("Try remove a not exist listner " + type);
        }

        public static void Trigger(EEvent type, object param){
            var itype  = (int)type;
            if (allListeners.TryGetValue(itype, out var tmplst)) {
                IsTriggingEvent = true;
                foreach (var listener in tmplst) {
                    listener?.Invoke(param);
                }
            }
            IsTriggingEvent = false;
        }
    }
}