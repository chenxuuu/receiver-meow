using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace ReceiverMeow.GoHttp
{
    class Ws
    {
        private static WebSocket ws = null;
        private static string module = "WS";
        public static void Connect(string url, int port)
        {
            if(ws!=null)
                Log.Error(module, $"禁止多次初始化 ws！");
            Log.Info(module, $"开始连接 ws://{url}:{port}");
            try
            {
                ws = new WebSocket($"ws://{url}:{port}/");
                ws.OnClose += Ws_OnClose;
                ws.OnOpen += Ws_OnOpen;
                ws.OnMessage += Ws_OnMessage;
                ws.Connect();
            }
            catch(Exception e)
            {
                Log.Error(module, $"连接错误：\n{e.Message}");
            }
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Log.Debug(module, $"收到消息：{Encoding.Default.GetString(e.RawData)}");
        }

        private static void Ws_OnOpen(object sender, EventArgs e)
        {
            Log.Info(module, $"已连接！");
        }

        private static void Ws_OnClose(object sender, CloseEventArgs e)
        {
            while(true)
            {
                try
                {
                    Log.Warn(module, $"连接断开！尝试两秒后重连...");
                    Thread.Sleep(2);
                    ws.Connect();
                    break;
                }
                catch{}
            }
        }


        /// <summary>
        /// 发送ws数据
        /// </summary>
        /// <param name="d">数据</param>
        /// <returns>是否成功</returns>
        public static bool Send(byte[] d)
        {
            try
            {
                if (ws != null)
                {
                    ws.Send(d);
                    Log.Debug(module, $"发送数据：{Encoding.Default.GetString(d)}");
                }
            }
            catch 
            {
                return false;
            }
            return true;
        }
    }
}
