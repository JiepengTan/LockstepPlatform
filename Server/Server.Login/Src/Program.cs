using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Lockstep.Server.Common;
using Server.Common;

namespace Lockstep.Server.Login {
    public class MyYieldInstruction { }

    public class MyWaitForSeconds : MyYieldInstruction {
        public float seconds;

        public MyWaitForSeconds(float seconds){
            this.seconds = seconds;
        }
    }

    public class MyYieldInsInfo { }

    public class MyWaitForSecondsInfo : MyYieldInsInfo {
        public MyWaitForSecondsInfo(DateTime BeginTime){
            this.BeginTime = BeginTime;
        }

        public DateTime BeginTime { get; set; }
    }

    public class RoutineInfo {
        public void Reset(){
            objYield = null;
            objYieldInfo = null;
        }

        public MyYieldInstruction objYield { get; set; } = null;

        public MyYieldInsInfo objYieldInfo { get; set; } = null;

        public IEnumerator routine;
    }

    public class MyMonoBehaviour {
        private List<RoutineInfo> lstRoutine = new List<RoutineInfo>();

        public void StartCoroutine(IEnumerator routine){
            if (null == routine) {
                return;
            }

            routine.MoveNext();

            RoutineInfo objRoutineInfo = new RoutineInfo();
            objRoutineInfo.routine = routine;
            SetRoutineInfo(ref objRoutineInfo);

            lstRoutine.Add(objRoutineInfo);
        }

        public void SetRoutineInfo(ref RoutineInfo objRoutineInfo){
            if (objRoutineInfo.routine.Current is MyYieldInstruction) {
                objRoutineInfo.objYield = objRoutineInfo.routine.Current as MyYieldInstruction;
                objRoutineInfo.objYieldInfo = new MyWaitForSecondsInfo(DateTime.Now);
            }
        }

        public void Update(){
            List<int> lstNeedDelIndex = new List<int>();
            for (int i = 0; i < lstRoutine.Count; ++i) {
                RoutineInfo item = lstRoutine[i];
                if (null == item) {
                    continue;
                }

                bool bCallMoveNext = item.objYield == null ? true : DealWithYieldInstruction(item);
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

            foreach (var item in lstNeedDelIndex) {
                lstRoutine.RemoveAt(item);
            }
        }

        public bool DealWithYieldInstruction(RoutineInfo objRoutineInfo){
            if (objRoutineInfo.objYield is MyWaitForSeconds) {
                TimeSpan objSpan = DateTime.Now - (objRoutineInfo.objYieldInfo as MyWaitForSecondsInfo).BeginTime;

                return objSpan.TotalSeconds > (objRoutineInfo.objYield as MyWaitForSeconds).seconds;
            }

            return true;
        }

        public void CoroutineTest(){
            StartCoroutine(CoroutineDetail());
        }

        IEnumerator CoroutineDetail(){
            yield return null;
            Console.WriteLine("yield return null:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            yield return new MyWaitForSeconds(1.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            yield return new MyWaitForSeconds(2.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            yield return new MyWaitForSeconds(1.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            yield return new MyWaitForSeconds(1.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            yield return new MyWaitForSeconds(1.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }

    internal class Program {
        static void Main(string[] args){
            MyMonoBehaviour objMyMonoBehaviour = new MyMonoBehaviour();
            Console.WriteLine("Create MyMonoBehaviour" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            objMyMonoBehaviour.CoroutineTest();
            while (true) {
                objMyMonoBehaviour.Update();
                Thread.Sleep(1);
            }

            // var config = ServerUtil.LoadConfig().GetServerConfig(EServerType.LoginServer);
            // ServerUtil.RunServer<LoginServer>(config);
        }
    }
}