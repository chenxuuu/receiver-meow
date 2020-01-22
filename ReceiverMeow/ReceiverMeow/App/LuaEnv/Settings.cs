using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class Settings
    {
        private bool tcpServerEnable = false;
        private int tcpServerPort = 23333;
        private long adminQQ = 0;
        private bool mqttEnable = false;
        private string mqttServer = "mqtt.papapoi.com";
        private int mqttPort = 1883;

        /// <summary>
        /// 保存配置
        /// </summary>
        private void Save()
        {
            File.WriteAllText(Common.AppData.CQApi.AppDirectory + "settings.json", JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// 管理员qq号
        /// </summary>
        public long AdminQQ
        {
            get => adminQQ;
            set
            {
                adminQQ = value;
                Save();
            }
        }

        /// <summary>
        /// 是否自动开启tcp服务端
        /// </summary>
        public bool TcpServerEnable
        {
            get => tcpServerEnable;
            set
            {
                if (value)
                {
                    tcpServerEnable = value;
                    TcpServer.Start();
                }
                else
                {
                    tcpServerEnable = value;
                    TcpServer.Stop();
                }
                Save();
            }
        }
        /// <summary>
        /// tcp服务端端口号
        /// </summary>
        public int TcpServerPort
        {
            get => tcpServerPort;
            set
            {
                if (tcpServerPort != value)
                {
                    tcpServerPort = value;
                    if (TcpServerEnable)
                    {
                        TcpServer.Stop();
                        TcpServer.Start();
                    }
                }
                Save();
            }
        }


        /// <summary>
        /// 是否开启mqtt连接功能
        /// </summary>
        public bool MqttEnable
        {
            get => mqttEnable;
            set
            {
                mqttEnable = value;
                Save();
            }
        }

        /// <summary>
        /// mqtt服务端
        /// </summary>
        public string MqttServer
        {
            get => mqttServer;
            set
            {
                mqttServer = value;
                Save();
            }
        }

        /// <summary>
        /// mqtt端口号
        /// </summary>
        public int MqttPort { get => mqttPort;
            set
            {
                mqttPort = value;
                Save();
            }
        }

    }
}
