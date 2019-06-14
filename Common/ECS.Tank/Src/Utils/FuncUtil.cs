using System;
using System.Collections.Generic;

namespace Lockstep.Game {
    public static class FuncUtil {
        private static int _startIdx;
        private static readonly List<Action> actions = new List<Action>();
        private static int _actionId =0;
        
        public static int RegisterFunc( Action action){
            var id = _actionId++;
            var idx = id - _startIdx;
            if (idx >= actions.Count) {
                actions.Add(action);
            }
            else {
                actions[idx] = action;
            }
            return id;
        }

        public static void Call(int id){
            var func =  actions[id - _startIdx];
            func.Invoke();
        }

        /// <summary>
        /// 移除掉 id <= leId 的所有 action
        /// </summary>
        /// <param name="leId"></param>
        public static void RemoveFunc(int leId){
            var len = leId - _startIdx +1;
            if(len <=0) return;
            actions.RemoveRange(0,len);
            _startIdx += len;
        }
    }
}