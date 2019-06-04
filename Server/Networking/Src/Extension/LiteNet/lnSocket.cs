using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using Lockstep.Logging;

namespace Lockstep.Networking
{
    public class LnSocket
    {
        private Uri mUrl;
    
        public LnSocket(Uri url)
        {
            mUrl = url;

            string protocol = mUrl.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
                throw new ArgumentException("Unsupported protocol: " + protocol);
        }


#if !UNITY_EDITOR && (UNITY_WEBGL || !UNITY_WEBPLAYER)
        private bool SuportsThreads { get { return false; } }

#else
        private bool SuportsThreads { get { return true; } }
#endif

        public void SendString(string str)
        {
            Send(Encoding.UTF8.GetBytes (str));
        }

        public string RecvString()
        {
            byte[] retval = Recv();
            if (retval == null)
                return null;
            return Encoding.UTF8.GetString (retval);
        }

        public bool IsConnecting { get; private set; }

        WebSocketSharp.WebSocket m_Socket;
        Queue<byte[]> m_Messages = new Queue<byte[]>();
        bool m_IsConnected = false;
        string m_Error = null;

        public bool IsConnected { get { return m_IsConnected; } }

        public IEnumerator Connect()
        {
            m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString());
            m_Socket.OnMessage += (sender, e) =>
            {
                m_Messages.Enqueue(e.RawData);
            };
            m_Socket.OnOpen += (sender, e) =>
            {
                m_IsConnected = true;
            };
            m_Socket.OnError += (sender, e) =>
            {
                m_Error = e.Message;
                Debug.LogError(e.Message);
            };
            m_Socket.OnClose += (sender, args) => m_IsConnected = false;

            if (SuportsThreads)
            {
                ThreadPool.QueueUserWorkItem((status) =>
                {
                    m_Socket.Connect();
                });
            }
            else
            {
                m_Socket.Connect();
            }

            IsConnecting = true;
            while (!m_IsConnected && m_Error == null)
            {
                yield return null;
            }
            IsConnecting = false;
        }

        public void Send(byte[] buffer)
        {
            m_Socket.Send(buffer);
        }

        public byte[] Recv()
        {
            if (m_Messages.Count == 0)
                return null;

            return m_Messages.Dequeue();
        }

        public void Close()
        {
            m_Socket.Close();
        }

        public string error
        {
            get {
                return m_Error;
            }
        }
    }
}