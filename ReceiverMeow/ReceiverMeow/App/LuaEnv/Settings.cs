using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class Settings
    {
        private bool tcpServerEnable = false;
        private int tcpServerPort = 23333;
        private long adminQQ = 0;
        private bool mqttEnable = false;
        private string mqttBroker = "broker.emqx.io";
        private int mqttPort = 1883;
        private string mqttUser = "user";
        private string mqttPassword = "password";
        private bool mqttTLS = false;
        private string clientID = Guid.NewGuid().ToString();


        private bool _mqtt_first = true;
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
                    if (_mqtt_first)
                    {
                        _mqtt_first = false;
                    }
                    else
                    {
                        Mqtt.Connect();
                    }
                }
                else
                {
                    if (_mqtt_first)
                    {
                        _mqtt_first = false;
                    }
                    else
                    {
                        Mqtt.Disconnect();
                    }
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

        private bool _tcp_first = true;
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
                    if (_tcp_first)
                    {
                        _tcp_first = false;
                    }
                    else
                    {
                        TcpServer.Start();
                    }
                }
                else
                {
                    tcpServerEnable = value;
                    if (_tcp_first)
                    {
                        _tcp_first = false;
                    }
                    else
                    {
                        TcpServer.Stop();
                    }
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
    }
}
