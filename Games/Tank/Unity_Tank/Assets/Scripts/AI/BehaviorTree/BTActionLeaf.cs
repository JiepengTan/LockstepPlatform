using System;
using System.Collections.Generic;

namespace Lockstep.AI {
    public abstract class BTActionLeaf : BTAction {
        private const int ACTION_READY = 0;
        private const int ACTION_RUNNING = 1;
        private const int ACTION_FINISHED = 2;

        class BTActionLeafContext : BTActionContext {
            internal int status;
            internal bool needExit;

            private object _userData;

            public T getUserData<T>() where T : class, new(){
                if (_userData == null) {
                    _userData = new T();
                }

                return (T) _userData;
            }

            public BTActionLeafContext(){
                status = ACTION_READY;
                needExit = false;

                _userData = null;
            }
        }

        public BTActionLeaf()
            : base(0){ }

        protected sealed override int OnUpdate(BTWorkingData wData){
            int runningState = BTRunningStatus.FINISHED;
            BTActionLeafContext thisContext = GetContext<BTActionLeafContext>(wData);
            if (thisContext.status == ACTION_READY) {
                onEnter(wData);
                thisContext.needExit = true;
                thisContext.status = ACTION_RUNNING;
            }

            if (thisContext.status == ACTION_RUNNING) {
                runningState = onExecute(wData);
                if (BTRunningStatus.IsFinished(runningState)) {
                    thisContext.status = ACTION_FINISHED;
                }
            }

            if (thisContext.status == ACTION_FINISHED) {
                if (thisContext.needExit) {
                    onExit(wData, runningState);
                }

                thisContext.status = ACTION_READY;
                thisContext.needExit = false;
            }

            return runningState;
        }

        protected sealed override void OnTransition(BTWorkingData wData){
            BTActionLeafContext thisContext = GetContext<BTActionLeafContext>(wData);
            if (thisContext.needExit) {
                onExit(wData, BTRunningStatus.TRANSITION);
            }

            thisContext.status = ACTION_READY;
            thisContext.needExit = false;
        }

        protected T getUserContexData<T>(BTWorkingData wData) where T : class, new(){
            return GetContext<BTActionLeafContext>(wData).getUserData<T>();
        }

        //--------------------------------------------------------
        // inherented by children-
        protected virtual void onEnter( /*in*/ BTWorkingData wData){ }

        protected virtual int onExecute(BTWorkingData wData){
            return BTRunningStatus.FINISHED;
        }

        protected virtual void onExit(BTWorkingData wData, int runningStatus){ }
    }
}