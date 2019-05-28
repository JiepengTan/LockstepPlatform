using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep.Math;
using UnityEngine;

namespace BinarySerializer  {
    [ToBinaryAttribute(true, false)]
    public class TestSerializeClassBase {
        public class TestInnerCls {
            public int iival;
            public string isval;
        }
        public class TestInnerCls2 {
            public int iival;
            public string isval;
            public Dictionary<int, TestInnerCls> idictVal;
            public List<TestInnerCls> ilstVal;
            public TestInnerCls[] iiarr;
        }
        public int ival;
        public float fval;
        public string sval;
        [NoToBinary]
        public string nsval;

        public Dictionary<int, TestInnerCls> dictVal;
        public List<TestInnerCls> lstVal;
        public TestInnerCls[] iarr;

        public Dictionary<int, TestInnerCls2> dictVal2;
        public List<TestInnerCls2> lstVal2;
        public TestInnerCls2[] iarr2;
    }

    public class TestSerializeClassChild : TestSerializeClassBase {
        public List<Vector3> cvecVals;
        [NoToBinary]
        public string csname;
        public int cival;
        public LVector2 lvec2;
    }
    public class TestSerializeClassChildChild : TestSerializeClassChild {
        public TestSerializeClassChild ccbaseVal;
        public Vector2 ccvecVals;
        [NoToBinary]
        public string ccsname;
        public int ccival;
        public LVector3 lvec3;
    }

    public class TestSerializeClassAlone {
        public List<Vector3> vecVals;
        [NoToBinary]
        public string sname;
        public int ival;
        public LFloat lfloat;
    }

}
