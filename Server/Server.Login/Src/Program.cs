using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Lockstep.Server.Common;
using Lockstep.Util;
using Server.Common;

namespace Lockstep.Server.Login {
    internal class Program {
        static IEnumerator CoroutineDetail(){
            yield return null;
            Console.WriteLine("yield return null:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            yield return new WaitForSeconds(1.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            yield return new WaitForSeconds(2.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            yield return new WaitForSeconds(1.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            yield return new WaitForSeconds(1.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            yield return new WaitForSeconds(1.0f);
            Console.WriteLine("wait 1.0 seconds:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        static void Main(string[] args){
            ServerUtil.StartServices();
            CoroutineHelper.StartCoroutine(CoroutineDetail());
            while (true) {
                ServerUtil.UpdateServices();
                Thread.Sleep(16);
            }

            //var config = ServerUtil.LoadConfig().GetServerConfig(EServerType.LoginServer);
            //ServerUtil.RunServer<LoginServer>(config);
        }
    }
}