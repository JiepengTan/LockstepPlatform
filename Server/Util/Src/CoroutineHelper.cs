using System;
using System.Collections;
using System.Collections.Generic;

namespace Lockstep.Util {
    public class YieldInstruction { }

    public sealed class WaitForSeconds : YieldInstruction {
        public WaitForSeconds(float seconds){
            this.Seconds = seconds;
        }

        public float Seconds { get; }
    }

    public abstract class CustomYieldInstruction : IEnumerator {
        public abstract bool keepWaiting { get; }

        public object Current {
            get { return (object) null; }
        }

        public bool MoveNext(){
            return this.keepWaiting;
        }

        public void Reset(){ }
    }

    public class WaitForSecondsRealtime : CustomYieldInstruction {
        private float m_WaitUntilTime = -1f;

        public WaitForSecondsRealtime(float time){
            this.waitTime = time;
        }

        public float waitTime { get; set; }

        public override bool keepWaiting {
            get {
                if ((double) this.m_WaitUntilTime < 0.0)
                    this.m_WaitUntilTime = Time.realtimeSinceStartup + this.waitTime;
                bool flag = (double) Time.realtimeSinceStartup < (double) this.m_WaitUntilTime;
                if (!flag)
                    this.m_WaitUntilTime = -1f;
                return flag;
            }
        }
    }


    public class YieldInsInfo { }

    public class WaitForSecondsInfo : YieldInsInfo {
        public WaitForSecondsInfo(float beginTime){
            BeginTime = beginTime;
        }

        public float BeginTime { get; }
    }

    public class RoutineInfo {
        public void Reset(){
            objYield = null;
            objYieldInfo = null;
        }

        public YieldInstruction objYield { get; set; } = null;
        public YieldInsInfo objYieldInfo { get; set; } = null;

        public IEnumerator routine;
    }

    public class CoroutineRunner {
        private List<RoutineInfo> lstRoutine = new List<RoutineInfo>();

        public void StartCoroutine(IEnumerator routine){
            if (null == routine) {
                return;
            }

            routine.MoveNext();
            var objRoutineInfo = new RoutineInfo {routine = routine};
            SetRoutineInfo(ref objRoutineInfo);
            lstRoutine.Add(objRoutineInfo);
        }

        public void SetRoutineInfo(ref RoutineInfo objRoutineInfo){
            if (objRoutineInfo.routine.Current is YieldInstruction) {
                objRoutineInfo.objYield = objRoutineInfo.routine.Current as YieldInstruction;
                objRoutineInfo.objYieldInfo = new WaitForSecondsInfo(Time.timeSinceLevelLoad);
            }
        }

        public void Update(){
            List<int> lstNeedDelIndex = new List<int>();
            for (int i = 0; i < lstRoutine.Count; ++i) {
                RoutineInfo item = lstRoutine[i];
                if (null == item) {
                    continue;
                }

                bool bCallMoveNext = item.objYield == null || DealWithYieldInstruction(item);
                if (!bCallMoveNext) {
                    continue;
                }

                if (item.routine.MoveNext()) {
                    SetRoutineInfo(ref item);
                }
                else {
                    lstNeedDelIndex.Add(i);
                }
            }
            
            for (int i = lstNeedDelIndex.Count-1; i >=0; i--) {
                lstRoutine.RemoveAt(lstNeedDelIndex[i]);
            }
        }

        public bool DealWithYieldInstruction(RoutineInfo objRoutineInfo){
            if (objRoutineInfo.objYield is WaitForSeconds waitForSec) {
                float objSpan = Time.timeSinceLevelLoad - ((WaitForSecondsInfo) objRoutineInfo.objYieldInfo).BeginTime;
                return objSpan > waitForSec.Seconds;
            }
            return true;
        }
    }


    public static class CoroutineHelper {
        static CoroutineRunner _runner = new CoroutineRunner();
        public static void DoStart(){ }

        public static void DoUpdate(){
            _runner.Update();
        }
        public static void StartCoroutine(IEnumerator enumerator){
            _runner.StartCoroutine(enumerator);
        }
    }
}