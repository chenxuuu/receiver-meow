using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class TcpServer
    {
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
                    //msg.Reply("Content-Type: text/plain\n\nHello from my web server!");
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
        }

        public static void Send(string msg)
        {
            try
            {
                server.Broadcast(msg);
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "tcp server",
                    "tcp server send failed!\r\n" + e.ToString());
            }
        }
    }
}
