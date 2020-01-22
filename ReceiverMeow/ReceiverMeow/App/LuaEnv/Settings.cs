using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class Settings
    {
        private bool tcpServerEnable = false;

        /// <summary>
        /// 管理员qq号
        /// </summary>
        public long AdminQQ { get; set; } = 0;

        /// <summary>
        /// 是否自动开启tcp服务端
        /// </summary>
        public bool TcpServerEnable { get => tcpServerEnable;
            set
            {
                if(value)
                {
                    tcpServerEnable = value;
                    TcpServer.Start();
                }
                else
                {
                    tcpServerEnable = value;
                    TcpServer.Stop();
                }
            }
        }
        /// <summary>
        /// tcp服务端端口号
        /// </summary>
        public int TcpServerPort { get; set; } = 23333;

        /// <summary>
        /// 是否开启mqtt连接功能
        /// </summary>
        public bool MqttEnable { get; set; } = false;
        /// <summary>
        /// mqtt服务端
        /// </summary>
        public string MqttServer { get; set; } = "mqtt.papapoi.com";
        /// <summary>
        /// mqtt端口号
        /// </summary>
        public int MqttPort { get; set; } = 1883;


    }
}
