using SimpleTCPStandar;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverMeow.LuaEnv
{
    class TcpServer
    {
        private static string moudle = "TCP";

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
                    server.Start(Utils.Setting.TcpServerPort);
                    Log.Info(moudle, $"服务器于{Utils.Setting.TcpServerPort}端口启动");
                    server.DataReceived += (sender, msg) => {
                        LuaEnv.LuaStates.Run("tcp", "TcpServer", msg.MessageString);
                    };
                    return true;
                }
                catch (Exception e)
                {
                    Log.Warn(moudle, $"无法监听{Utils.Setting.TcpServerPort}端口");
                    Log.Warn(moudle, $"原因：{e.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 结束
        /// </summary>
        /// <returns></returns>
        public static bool Stop()
        {
            lock (serverLock)
            {
                if (!server.IsStarted)
                    return true;
                try
                {
                    server.Stop();
                    Log.Info(moudle, $"服务端已停机");
                    return true;
                }
                catch(Exception e)
                {
                    Log.Warn(moudle, $"服务器关闭失败");
                    Log.Warn(moudle, $"原因：{e.Message}");
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
            timer.Interval = 1000;//1s
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
                    Log.Warn(moudle, $"tcp队列报错：{ee.Message}");
                }
            });
            timer.Start();
        }
    }
}
