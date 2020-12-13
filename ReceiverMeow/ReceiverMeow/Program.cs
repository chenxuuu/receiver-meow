using System;
using WebSocketSharp;

namespace ReceiverMeow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lua = new NLua.Lua();
            lua.DoString("print(1213122)");

            using (var ws = new WebSocket("ws://tcplab.openluat.com:12421/"))
            {
                ws.OnMessage += (sender, e) =>
                    Console.WriteLine("Laputa says: " + e.Data);

                ws.Connect();
                ws.Send("{\"function\":0}");
                Console.ReadKey(true);
            }
        }
    }
}
