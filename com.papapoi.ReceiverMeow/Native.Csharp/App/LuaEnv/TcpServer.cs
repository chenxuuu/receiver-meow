using SimpleTCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class TcpServer
    {
        //需要发送的包列表
        private static ArrayList toSend = new ArrayList();
        //每个包发送间隔时间（可以自己改）
        private static int packTime = 1000;

        private static SimpleTcpServer server = new SimpleTcpServer();
        public static void Start()
        {
            try
            {
                server.StringEncoder = Encoding.UTF8;
                server.Start(23333);
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server",
                    "tcp server started!");
                server.DataReceived += (sender, msg) => {
                    LuaEnv.RunLua(
                        $"message=[[{msg.MessageString.Replace("]", "] ")}]] ",
                        "envent/ReceiveTcp.lua");
                };
            }
            catch(Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "tcp server", 
                    "tcp server start failed!\r\n"+e.ToString());
            }

            //消息发送队列
            Task.Run(() =>
            {
                while(true)
                {
                    while (toSend.Count > 0)
                    {
                        string temp = toSend[0].ToString();//取出第一个数据
                        toSend.RemoveAt(0);
                        server.Broadcast(temp);
                        Task.Delay(packTime).Wait();
                    }
                    Task.Delay(200).Wait();//等等，防止卡死
                }
            });
        }

        public static void Send(string msg)
        {
            try
            {
                //server.Broadcast(msg);
                toSend.Add(msg);
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "tcp server",
                    "tcp server send failed!\r\n" + e.ToString());
            }
        }
    }
}
