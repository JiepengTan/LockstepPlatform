using System;
using System.Collections.Generic;

namespace Lockstep.AI
{
    public class BTTreeNode
    {
        //-------------------------------------------------------------------
        private const int defaultChildCount = -1; //TJQ： unlimited count
        //-------------------------------------------------------------------
        private List<BTTreeNode> _children;
        private int _maxChildCount;
        //private TBTTreeNode _parent;
        //-------------------------------------------------------------------
        public BTTreeNode(int maxChildCount = -1)
        {
            _children = new List<BTTreeNode>();
            if (maxChildCount >= 0) {
                _children.Capacity = maxChildCount;
            }
            _maxChildCount = maxChildCount;
        }
        public BTTreeNode()
            : this(defaultChildCount)
        {}
        ~BTTreeNode()
        {
            _children = null;
            //_parent = null;
        }
        //-------------------------------------------------------------------
        public BTTreeNode AddChild(BTTreeNode node)
        {
            if (_maxChildCount >= 0 && _children.Count >= _maxChildCount) {
                TLogger.WARNING("**BT** exceeding child count");
                return this;
            }
            _children.Add(node);
            //node._parent = this;
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
        public T GetChild<T>(int index) where T : BTTreeNode 
        {
            if (index < 0 || index >= _children.Count) {
                return null;
            }
            return (T)_children[index];
        }
    }
}
