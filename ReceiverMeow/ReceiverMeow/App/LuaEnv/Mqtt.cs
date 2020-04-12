using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class Mqtt
    {
        private static MqttFactory factory = new MqttFactory();
        private static MQTTnet.Client.IMqttClient mqttClient = factory.CreateMqttClient();

        private static IMqttClientOptions getOptions()
        {
            var op = new MqttClientOptionsBuilder()
           .WithClientId(Utils.setting.ClientID)
           .WithTcpServer(Utils.setting.MqttBroker, Utils.setting.MqttPort)
           .WithCredentials(Utils.setting.MqttUser, Utils.setting.MqttPassword)
           .WithCleanSession();
            IMqttClientOptions options;
            if (Utils.setting.MqttTLS)
                options = op.WithTls().Build();
            else
                options = op.Build();
            return options;
        }

        /// <summary>
        /// 初始化各个参数
        /// </summary>
        public static void Initial()
        {
            //连接成功
            mqttClient.Connected += (sender,e)=> 
            {
                LuaEnv.LuaStates.Run("MQTT", "MQTT", new {
                    t = "connected"
                });
            };

            //收到消息
            mqttClient.ApplicationMessageReceived += (sender, e) =>
            {
                LuaEnv.LuaStates.Run("MQTT", "MQTT", new
                {
                    t = "receive",
                    topic = e.ApplicationMessage.Topic,
                    payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload),
                    qos = (int)e.ApplicationMessage.QualityOfServiceLevel
                });
            };

            //断线重连
            mqttClient.Disconnected += async (s, e) =>
            {
                Common.AppData.CQLog.Warning("lua插件", "MQTT连接已断开");
                if (!Utils.setting.MqttEnable)
                    return;
                await Task.Delay(TimeSpan.FromSeconds(5));
                Common.AppData.CQLog.Warning("lua插件", "MQTT尝试重连");
                try
                {
                    await mqttClient.ConnectAsync(getOptions());
                }
                catch(Exception ee)
                {
                    Common.AppData.CQLog.Error("lua插件", $"MQTT重连失败");
                    Common.AppData.CQLog.Error("lua插件", $"原因： {ee.Message}");
                }
            };
        }

        /// <summary>
        /// 连接状态
        /// </summary>
        /// <returns></returns>
        public static bool Status()
        {
            return mqttClient.IsConnected;
        }

        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="qos"></param>
        /// <returns></returns>
        public static bool Subscribe(string topic,int qos)
        {
            if (mqttClient.IsConnected)
            {
                try
                {
                    mqttClient.SubscribeAsync(topic,(MqttQualityOfServiceLevel)qos);
                    return true;
                }
                catch (Exception e)
                {
                    Common.AppData.CQLog.Error("lua插件", $"fail to subscribe mqtt message:{topic}");
                    Common.AppData.CQLog.Error("lua插件", $"reason: {e.Message}");
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="payload"></param>
        public static bool Publish(string topic,string payload, long qos)
        {
            if (mqttClient.IsConnected)
            {
                try
                {
                    mqttClient.PublishAsync(topic, payload, (MqttQualityOfServiceLevel)qos);
                    return true;
                }
                catch (Exception e)
                {
                    Common.AppData.CQLog.Error("lua插件", $"fail to send mqtt message:{topic},{payload}");
                    Common.AppData.CQLog.Error("lua插件", $"reason: {e.Message}");
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// 连接mqtt服务器
        /// </summary>
        public static void Connect()
        {
            try
            {
                mqttClient.ConnectAsync(getOptions());
            }
            catch (Exception e)
            {
                Common.AppData.CQLog.Error("lua插件", $"fail to connect mqtt");
                Common.AppData.CQLog.Error("lua插件", $"reason: {e.Message}");
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public static void Disconnect()
        {
            if(mqttClient.IsConnected)
            {
                try
                {
                    mqttClient.DisconnectAsync();
                }
                catch (Exception e)
                {
                    Common.AppData.CQLog.Error("lua插件", $"fail to disconnect mqtt");
                    Common.AppData.CQLog.Error("lua插件", $"reason: {e.Message}");
                }
            }
        }
    }
}
