using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverMeow
{
    class Settings
    {
        private bool _colorful = true;
        private bool mqttEnable = false;
        private string mqttBroker = "broker.emqx.io";
        private int mqttPort = 1883;
        private string mqttUser = "user";
        private string mqttPassword = "password";
        private bool mqttTLS = false;
        private string clientID = Guid.NewGuid().ToString();
        private int keepAlive = 60;
        private bool tcpServerEnable = false;
        private int tcpServerPort = 23333;
        private long adminQQ = 0;


        /// <summary>
        /// 保存配置
        /// </summary>
        private void Save()
        {
            File.WriteAllText(Utils.Path + "settings.json", JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// 终端是否开启彩色界面
        /// </summary>
        public bool Colorful
        {
            get => _colorful;
            set
            {
                _colorful = value;
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
                if (value)
                {
                    LuaEnv.Mqtt.Connect();
                }
                else
                {
                    LuaEnv.Mqtt.Disconnect();
                }
                Save();
            }
        }

        /// <summary>
        /// 启用tls
        /// </summary>
        public bool MqttTLS
        {
            get => mqttTLS;
            set
            {
                mqttTLS = value;
                Save();
            }
        }

        /// <summary>
        /// mqtt服务端
        /// </summary>
        public string MqttBroker
        {
            get => mqttBroker;
            set
            {
                mqttBroker = value;
                Save();
            }
        }

        /// <summary>
        /// mqtt端口号
        /// </summary>
        public int MqttPort
        {
            get => mqttPort;
            set
            {
                mqttPort = value;
                Save();
            }
        }

        /// <summary>
        /// mqtt服务器用户名
        /// </summary>
        public string MqttUser
        {
            get => mqttUser;
            set
            {
                mqttUser = value;
                Save();
            }
        }

        /// <summary>
        /// mqtt服务器密码
        /// </summary>
        public string MqttPassword
        {
            get => mqttPassword;
            set
            {
                mqttPassword = value;
                Save();
            }
        }

        /// <summary>
        /// GUID唯一识别码
        /// </summary>
        public string ClientID
        {
            get => clientID;
            set
            {
                clientID = value;
                Save();
            }
        }

        /// <summary>
        /// mqtt保活心跳周期
        /// </summary>
        public int KeepAlive
        {
            get => keepAlive;
            set
            {
                keepAlive = value;
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
                tcpServerEnable = value;
                if (value)
                {
                    LuaEnv.TcpServer.Start();
                }
                else
                {
                    LuaEnv.TcpServer.Stop();
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
                tcpServerPort = value;
                Save();
            }
        }
        /// <summary>
        /// tcp服务端端口号
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
    }
}
