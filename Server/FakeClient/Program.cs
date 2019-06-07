using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Lockstep.Util;

namespace Lockstep.FakeClient {

    internal class Program {
        public static void Main(string[] args){
            ClientUtil.RunClient();
        }
    }
}