using SimpleTCP;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class TcpServer
    {
        //需要发送的包列表
        private static ConcurrentBag<string> toSend = new ConcurrentBag<string>();

        private static SimpleTcpServer server = new SimpleTcpServer();

        //服务器操作锁
        private static readonly object serverLock = new object();

        /// <summary>
        /// 启动
        /// </summary>
        public static bool Start()
        {
            lock (serverLock)
            {
                if (server.IsStarted)
                    return true;
                try
                {
                    server.StringEncoder = Encoding.UTF8;
                    server.Start(Utils.setting.TcpServerPort);
                    Common.AppData.CQLog.Info("lua插件", $"tcp server started at port {Utils.setting.TcpServerPort}");
                    server.DataReceived += (sender, msg) => {
                        LuaEnv.LuaStates.Run("tcp", "TcpServer", msg.MessageString);
                    };
                    return true;
                }
                catch (Exception e)
                {
                    Common.AppData.CQLog.Error("lua插件", $"tcp server failed to start at port {Utils.setting.TcpServerPort}");
                    Common.AppData.CQLog.Error("lua插件", $"reason: {e.Message}");
                    return false;
                }
            }
        }

        public static bool Stop()
        {
            lock (serverLock)
            {
                if (!server.IsStarted)
                    return true;
                try
                {
                    server.Stop();
                    return true;
                }
                catch(Exception e)
                {
                    Common.AppData.CQLog.Error("lua插件", $"tcp server failed to stop at port {Utils.setting.TcpServerPort}");
                    Common.AppData.CQLog.Error("lua插件", $"reason: {e.Message}");
                    return false;
                }
            }
        }

        public static bool Send(string msg)
        {
            if(server.IsStarted)
                toSend.Add(msg);
            return server.IsStarted;
        }

        public static void SendList()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;//60s
            timer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) =>
            {
                try
                {
                    if(toSend.Count > 0)
                    {
                        string temp;
                        toSend.TryTake(out temp);
                        server.Broadcast(temp);
                    }
                }
                catch (Exception ee)
                {
                    Common.AppData.CQLog.Error("lua插件", $"tcp队列报错：{ee.Message}");
                }
            });
            timer.Start();
        }
    }
}
