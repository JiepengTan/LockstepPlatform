using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Logging;
using Server.Common;


namespace Lockstep.Networking {
    public class CoroutineUtil {
        public static void StartCoroutine(IEnumerator enumerator){ }
    }

    public class BTimer {
        private BTimer(){ }
        public static long CurrentTick { get; protected set; }

        public delegate void DoneHandler(bool isSuccessful);

        private static BTimer _instance;
        private List<Action> _mainThreadActions;

        /// <summary>
        /// Event, which is invoked every second
        /// </summary>
        public event Action<long> OnTick;

        public event Action ApplicationQuit;

        private readonly object _mainThreadLock = new object();

        public static BTimer Instance {
            get {
                if (_instance == null) {
                    _instance = new BTimer();
                }

                return _instance;
            }
        }
        public void DoAwake(){
            _mainThreadActions = new List<Action>();
            _instance = this;
            CoroutineUtil.StartCoroutine(StartTicker());
        }

        public void DoUpdate(){
            if (_mainThreadActions.Count > 0) {
                lock (_mainThreadLock) {
                    foreach (var actions in _mainThreadActions) {
                        actions.Invoke();
                    }
                    _mainThreadActions.Clear();
                }
            }
        }

        /// <summary>
        ///     Waits while condition is false
        ///     If timed out, callback will be invoked with false
        /// </summary>
        public static void WaitUntil(Func<bool> condiction, DoneHandler doneCallback, float timeoutSeconds){
            CoroutineUtil.StartCoroutine(WaitWhileTrueCoroutine(condiction, doneCallback, timeoutSeconds, true));
        }

        /// <summary>
        ///     Waits while condition is true
        ///     If timed out, callback will be invoked with false
        /// </summary>
        public static void WaitWhile(Func<bool> condiction, DoneHandler doneCallback, float timeoutSeconds){
            CoroutineUtil.StartCoroutine(WaitWhileTrueCoroutine(condiction, doneCallback, timeoutSeconds));
        }

        private static IEnumerator WaitWhileTrueCoroutine(Func<bool> condition, DoneHandler callback,
            float timeoutSeconds, bool reverseCondition = false){
            while ((timeoutSeconds > 0) && (condition.Invoke() == !reverseCondition)) {
                timeoutSeconds -= Time.deltaTime;
                yield return null;
            }

            callback.Invoke(timeoutSeconds > 0);
        }

        public static void AfterSeconds(float time, Action callback){
            CoroutineUtil.StartCoroutine(Instance.StartWaitingSeconds(time, callback));
        }

        public static void ExecuteOnMainThread(Action action){
            Instance.OnMainThread(action);
        }

        public void OnMainThread(Action action){
            lock (_mainThreadLock) {
                _mainThreadActions.Add(action);
            }
        }

        private IEnumerator StartWaitingSeconds(float time, Action callback){
            yield return new WaitForSeconds(time);
            callback.Invoke();
        }

        private IEnumerator StartTicker(){
            CurrentTick = 0;
            while (true) {
                yield return new WaitForSeconds(1);
                CurrentTick++;
                try {
                    if (OnTick != null)
                        OnTick.Invoke(CurrentTick);
                }
                catch (Exception e) {
                    Debug.LogError(e);
                }
            }
        }

        void OnDestroy(){ }

        void OnApplicationQuit(){
            if (ApplicationQuit != null)
                ApplicationQuit.Invoke();
        }
    }
}