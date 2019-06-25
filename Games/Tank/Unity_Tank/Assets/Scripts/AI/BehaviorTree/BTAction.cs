using System;

namespace Lockstep.AI {
    public class BTActionContext { }

    public abstract class BTAction : BTTreeNode {
        private static int sUNIQUEKEY = 0;

        private static int genUniqueKey(){
            if (sUNIQUEKEY >= int.MaxValue) {
                sUNIQUEKEY = 0;
            }
            else {
                sUNIQUEKEY = sUNIQUEKEY + 1;
            }

            return sUNIQUEKEY;
        }

        //-------------------------------------------------------------
        protected int _uniqueKey;
        protected BTPrecondition _precondition;
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
            _uniqueKey = BTAction.genUniqueKey();
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

        protected T GetContext<T>(BTWorkingData wData) where T : BTActionContext, new(){
            int uniqueKey = GetHashCode();
            T thisContext;
            if (wData.context.ContainsKey(uniqueKey) == false) {
                thisContext = new T();
                wData.context.Add(uniqueKey, thisContext);
            }
            else {
                thisContext = (T) wData.context[uniqueKey];
            }

            return thisContext;
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