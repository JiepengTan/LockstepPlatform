//#define DEBUG_SIMPLE_CHECK  暴力检测 不同步

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
            public int Tick;
            public ICommand<T> cmd;

            public CommandNode(int tick, ICommand<T> cmd, CommandNode pre = null, CommandNode next = null){
                this.Tick = tick;
                this.cmd = cmd;
                this.pre = pre;
                this.next = next;
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
#if DEBUG_SIMPLE_CHECK
        public List<CommandNode> allCmds = new List<CommandNode>();
#endif
        ///RevertTo tick , so all cmd between [tick,~)(Include tick) should undo
        public void RevertTo(int tick){
#if DEBUG_SIMPLE_CHECK
            if(allCmds.Count == 0) return;
            allCmds[(int) tick].cmd.Undo(_param);
#else
            if (_tail == null || _tail.Tick < tick) {
                return;
            }

            var newTail = _tail;
            while (newTail.pre != null && newTail.pre.Tick >= tick) {
                newTail = newTail.pre;
            }

            try {
                Debug.Assert(newTail.Tick >= tick,
                    $"newTail must be the first cmd executed after that tick : tick:{tick}  newTail.Tick:{newTail.Tick}");
                Debug.Assert(newTail.pre == null
                             || newTail.pre.Tick < tick,
                    $"newTail must be the first cmd executed in that tick : tick:{tick}  " +
                    $"newTail.pre.Tick:{newTail.pre?.Tick ?? tick}");
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }

            var minTickNode = newTail;
            var maxTickNode = _tail;

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

            _funcUndoCommand(minTickNode, maxTickNode, _param);
#endif
        }

        ///Discard all cmd between [0,maxVerifiedTick] (Include maxVerifiedTick)
        public void Clean(int maxVerifiedTick){
#if DEBUG_SIMPLE_CHECK
#else
            return;
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

        public void Execute(int tick, ICommand<T> cmd){
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
        protected void UndoCommands(CommandNode minTickNode, CommandNode maxTickNode, T param){
            if (maxTickNode == null) return;
            while (maxTickNode != minTickNode) {
                maxTickNode.cmd.Undo(_param);
                maxTickNode = maxTickNode.pre;
            }

            maxTickNode.cmd.Undo(_param);
        }
    }
}