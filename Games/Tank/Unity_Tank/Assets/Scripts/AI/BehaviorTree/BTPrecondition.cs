using System;

namespace Lockstep.AI
{
    //---------------------------------------------------------------
    public abstract class BTPrecondition : BTTreeNode
    {
        public BTPrecondition(int maxChildCount)
            : base(maxChildCount)
        {}
        public abstract bool IsTrue( /*in*/ BTWorkingData wData);
    }
    public abstract class BtPreconditionLeaf : BTPrecondition
    {
        public BtPreconditionLeaf()
            : base(0)
        {}
    }
    public abstract class BtPreconditionUnary : BTPrecondition
    {
        public BtPreconditionUnary(BTPrecondition lhs)
            : base(1)
        {
            AddChild(lhs);
        }
    }
    public abstract class BtPreconditionBinary : BTPrecondition
    {
        public BtPreconditionBinary(BTPrecondition lhs, BTPrecondition rhs)
            : base(2)
        {
            AddChild(lhs).AddChild(rhs);
        }
    }
    //--------------------------------------------------------------
    //basic precondition
    public class BtPreconditionTrue : BtPreconditionLeaf
    {
        public override bool IsTrue( /*in*/ BTWorkingData wData)
        {
            return true;
        }
    }
    public class BtPreconditionFalse : BtPreconditionLeaf
    {
        public override bool IsTrue( /*in*/ BTWorkingData wData)
        {
            return false;
        }
    }
    //---------------------------------------------------------------
    //unary precondition
    public class BtPreconditionNot : BtPreconditionUnary
    {
        public BtPreconditionNot(BTPrecondition lhs)
            : base(lhs)
        {}
        public override bool IsTrue( /*in*/ BTWorkingData wData)
        {
            return !GetChild<BTPrecondition>(0).IsTrue(wData);
        }
    }
    //---------------------------------------------------------------
    //binary precondition
    public class BtPreconditionAnd : BtPreconditionBinary
    {
        public BtPreconditionAnd(BTPrecondition lhs, BTPrecondition rhs)
            : base(lhs, rhs)
        { }
        public override bool IsTrue( /*in*/ BTWorkingData wData)
        {
            return GetChild<BTPrecondition>(0).IsTrue(wData) &&
                   GetChild<BTPrecondition>(1).IsTrue(wData);
        }
    }
    public class BtPreconditionOr : BtPreconditionBinary
    {
        public BtPreconditionOr(BTPrecondition lhs, BTPrecondition rhs)
            : base(lhs, rhs)
        { }
        public override bool IsTrue( /*in*/ BTWorkingData wData)
        {
            return GetChild<BTPrecondition>(0).IsTrue(wData) ||
                   GetChild<BTPrecondition>(1).IsTrue(wData);
        }
    }
    public class BtPreconditionXor : BtPreconditionBinary
    {
        public BtPreconditionXor(BTPrecondition lhs, BTPrecondition rhs)
            : base(lhs, rhs)
        { }
        public override bool IsTrue( /*in*/ BTWorkingData wData)
        {
            return GetChild<BTPrecondition>(0).IsTrue(wData) ^
                   GetChild<BTPrecondition>(1).IsTrue(wData);
        }
    }
}
