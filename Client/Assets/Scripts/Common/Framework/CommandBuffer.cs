#define DEBUG_SIMPLE_CHECK
using System;
using System.Collections.Generic;
using Entitas;
using Lockstep.Logging;

namespace Lockstep.Game {
    public interface ICommand<T> {
        void Do(T param);
        void Undo(T param);
    }

    public class BaseCommand<T> : ICommand<T> {
        public virtual void Do(T param){ }
        public virtual void Undo(T param){ }
    }

    public class CommandBuffer<T> {
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
        private T _param;
        private Action<CommandNode, CommandNode, T> _funcUndoCommand;

        public CommandBuffer(T param, Action<CommandNode, CommandNode, T> funcUndoCommand){
            _param = param;
            _funcUndoCommand = funcUndoCommand ?? UndoCommands;
        }

        public List<CommandNode> allCmds = new List<CommandNode>();

        ///RevertTo tick , so all cmd between [tick,~)(Include tick) should undo
        public void RevertTo(uint tick){
            if(allCmds.Count == 0) return;
#if DEBUG_SIMPLE_CHECK
            allCmds[(int) tick].cmd.Undo(_param);
#else
            if (_tail == null || _tail.Tick < tick) {
                return;
            }

            var newTail = _tail;
            while (newTail.pre != null && newTail.pre.Tick >= tick) {
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

            _funcUndoCommand(from, to, _param);
#endif
        }

        ///Discard all cmd between [0,maxVerifiedTick] (Include maxVerifiedTick)
        public void Clean(uint maxVerifiedTick){
#if DEBUG_SIMPLE_CHECK
#else
            if (_head == null || _head.Tick > maxVerifiedTick) {
                return;
            }

            var newHead = _head;
            while (newHead.next != null && newHead.next.Tick <= maxVerifiedTick) {
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
#endif
        }

        public void Execute(uint tick, ICommand<T> cmd){
#if DEBUG_SIMPLE_CHECK
            var iTick = (int) tick;
            for (int i = allCmds.Count; i <= iTick; i++) {
                allCmds.Add(null);
            }

            cmd.Do(_param);
            var node = new CommandNode(tick, cmd, _tail, null);
            allCmds[iTick] = node;
#else
            if (cmd == null) return;
            cmd.Do(_param);
            var node = new CommandNode(tick, cmd, _tail, null);
            if (_head == null) {
                _head = node;
                _tail = node;
                return;
            }

            _tail.next = node;
            _tail = node;
#endif
        }

        /// 只需执行undo 不需要顾虑指针的维护  //如果有性能需要可以考虑合并Cmd 
        protected void UndoCommands(CommandNode from, CommandNode to, T param){
            if (to == null) return;
            while (to != from) {
                to.cmd.Undo(_param);
                to = to.pre;
            }

            to.cmd.Undo(_param);
        }
    }
}