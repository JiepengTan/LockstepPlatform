using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lockstep.Logging;

namespace Lockstep.AI
{
    public unsafe partial class BTNode {
        protected virtual int MemSize => 0;
        public int UniqueKey {
            set => _uniqueKey = value;
        }
        protected int _uniqueKey;
        
        private const int defaultChildCount = -1; 
        protected BTPrecondition _precondition;
        private List<BTNode> _children;
        private int _maxChildCount;
        
        public BTNode(int maxChildCount = -1)
        {
            _children = new List<BTNode>();
            if (maxChildCount >= 0) {
                _children.Capacity = maxChildCount;
            }
            _maxChildCount = maxChildCount;
        }
        public BTNode()
            : this(defaultChildCount)
        {}
        ~BTNode()
        {
            _children = null;
        }
        
        public BTNode AddChild(BTNode node)
        {
            if (_maxChildCount >= 0 && _children.Count >= _maxChildCount) {
                Logging.Debug.LogError("**BT** exceeding child count");
                return this;
            }
            _children.Add(node);
            return this;
        }
        public int GetChildCount()
        {
            return _children.Count;
        }
        public bool IsIndexValid(int index)
        {
            return index >= 0 && index < _children.Count;
        }
        public T GetChild<T>(int index) where T : BTNode 
        {
            if (index < 0 || index >= _children.Count) {
                return null;
            }
            return (T)_children[index];
        }

        public int GetTotalNodeCount(){
            int sum = 0;
            foreach (var child in _children) {
                sum += child.GetTotalNodeCount();
            }
            return sum + 1;
        }
        public int GetTotalMemSize(){
            int sum = 0;
            foreach (var child in _children) {
                sum += child.GetTotalMemSize();
            }
            return sum + MemSize;
        }
        public int[] GetTotalOffsets(){
            var nodes = new List<BTNode>();
            Flatten(nodes);
            var offsets = new int[nodes.Count];
           
            for (int i = 0; i < nodes.Count; i++) {
                Debug.Assert(nodes[i]._uniqueKey == i,"Error: Idx not match");
            }
            var offset = 0;
            for (int i = 0; i < nodes.Count; i++) {
                offsets[i] = offset;
                offset += nodes[i].MemSize;
            }
            return offsets;
        }

        protected virtual void Flatten(List<BTNode> nodes){
            nodes.Add(this);
            _precondition?.Flatten(nodes);
            foreach (var child in _children) {
                child.Flatten(nodes);
            }
        }
    }
}
