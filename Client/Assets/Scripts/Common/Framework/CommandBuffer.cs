using Entitas;

namespace Lockstep.Game {
    public interface ICommand<T> {
        void Do<T>(T param);
        void Undo<T>(T param);
    }

    public class BaseCommand<T> : ICommand<T> {
        public void Do<T>(T param){ }
        public void Undo<T>(T param){ }
    }

    public class CommandBuffer<T> : IRollbackable {
        public class CommandNode {
            public CommandNode pre;
            public CommandNode next;
            public uint Tick;
            public ICommand<T> cmd;
            public CommandNode(uint tick, ICommand<T> cmd, CommandNode pre = null, CommandNode next = null){
                this.Tick = tick;
                this.cmd = cmd;
                this.pre = null;
                this.next = null;
            }
        }

        private CommandNode _head;
        private CommandNode _tail;
        public T param;
        ///RevertTo tick , so all cmd between [tick,~)(Include tick) should undo
        public void RevertTo(uint tick){ 
            if (_tail == null || _tail.Tick < tick) {
                return;
            }

            var newTail = _tail;
            while (newTail.pre != null && newTail.pre.Tick >= tick ) {
                newTail = newTail.pre;
            }

            var from = newTail;
            var to = _tail;
            
            if (newTail.pre == null) {
                _head = null;
                _tail = null;
            }
            else {
                _tail = newTail.pre;
                //断开链接
                _tail.next = null;
                newTail.pre = null;
            }

            UndoCommands(from, to);
        }

        public void BackUp(uint tick){}

        ///Discard all cmd between [0,maxVerifiedTick] (Include maxVerifiedTick)
        public void Clean(uint maxVerifiedTick){
            if (_head == null || _head.Tick > maxVerifiedTick) { return;}
            
            var newHead = _head;
            while (newHead.next != null && newHead.next.Tick <= maxVerifiedTick ) {
                newHead = newHead.next;
            }

            if (newHead.next == null) {
                _tail = null;
                _head = null;
            }
            else {
                _head = newHead.next;
                //断开链接
                _head.pre = null;
                newHead.next = null;
            }
        }

        public void Execute(uint tick, ICommand<T> cmd){
            if (cmd == null) return;
            cmd.Do(param);
            var node = new CommandNode(tick, cmd, _tail, null);
            if (_head == null) {
                _head = node;
                _tail = node;
                return;
            }
            _tail.next = node;
            _tail = node;
        }

        /// 只需执行undo 不需要顾虑指针的维护  //如果有性能需要可以考虑合并Cmd 
        protected virtual void UndoCommands(CommandNode from, CommandNode to){
            if(to == null) return;
            while (to != from) {
                to.cmd.Undo(param);
                to = to.pre;
            }
            to.cmd.Undo(param);
        }
    }
}