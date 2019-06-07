using System;
using System.Collections.Generic;
using LitJson;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Common;

namespace Lockstep.FakeClient {
    public class Client {
        public class Debug {
            public static string prefix = "";

            public static void Log(string ss){
                Console.WriteLine(prefix + ss);
            }
        }

    }
}