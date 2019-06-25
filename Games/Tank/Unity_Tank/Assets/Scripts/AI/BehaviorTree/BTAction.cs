using System;
using System.Collections.Generic;

namespace Lockstep.AI {
    public class BTActionContext { }

    public abstract unsafe class BTAction : BTNode {
#if DEBUG
        protected string _name;
        public string name {
            get { return _name; }
            set { _name = value; }
        }
#endif
        
        //-------------------------------------------------------------
        public BTAction(int maxChildCount)
            : base(maxChildCount){
        }

        ~BTAction(){
            _precondition = null;
        }

        //-------------------------------------------------------------
        public bool Evaluate( /*in*/ BTWorkingData wData){
            return (_precondition == null || _precondition.IsTrue(wData)) && OnEvaluate(wData);
        }

        public int Update(BTWorkingData wData){
            return OnUpdate(wData);
        }

        public void Transition(BTWorkingData wData){
            OnTransition(wData);
        }

        public BTAction SetPrecondition(BTPrecondition precondition){
            _precondition = precondition;
            return this;
        }

        public override int GetHashCode(){
            return _uniqueKey;
        }

        //--------------------------------------------------------
        // inherented by children
        protected virtual bool OnEvaluate( /*in*/ BTWorkingData wData){
            return true;
        }

        protected virtual int OnUpdate(BTWorkingData wData){
            return BTRunningStatus.FINISHED;
        }

        protected virtual void OnTransition(BTWorkingData wData){ }
    }
}