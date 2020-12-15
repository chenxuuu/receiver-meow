using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReceiverMeow.LuaEnv
{
    class Mqtt
    {
        private static MqttFactory factory = new MqttFactory();
        private static IMqttClient mqttClient = factory.CreateMqttClient();
        private static string moudle = "MQTT";

        private static IMqttClientOptions getOptions()
        {
            var op = new MqttClientOptionsBuilder()
           .WithClientId(Utils.Setting.ClientID)
           .WithTcpServer(Utils.Setting.MqttBroker, Utils.Setting.MqttPort)
           .WithCredentials(Utils.Setting.MqttUser, Utils.Setting.MqttPassword)
           .WithKeepAlivePeriod(new TimeSpan(0,0,Utils.Setting.KeepAlive))
           .WithCleanSession();
            IMqttClientOptions options;
            if (Utils.Setting.MqttTLS)
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
                Log.Info(moudle, "已连接");
                LuaEnv.LuaStates.Run("MQTT", "MQTT", new {
                    t = "connected"
                });
            };

            //收到消息
            mqttClient.ApplicationMessageReceived += (sender, e) =>
            {
                Log.Debug(moudle, 
                    $"收到消息：{e.ApplicationMessage.Topic} {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
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
                Log.Warn(moudle, "MQTT连接已断开");
                if (!Utils.Setting.MqttEnable)
                    return;
                await Task.Delay(TimeSpan.FromSeconds(5));
                Log.Warn(moudle, "MQTT尝试重连");
                try
                {
                    await mqttClient.ConnectAsync(getOptions());
                }
                catch(Exception ee)
                {
                    Log.Warn(moudle, $"MQTT重连失败");
                    Log.Warn(moudle, $"原因： {ee.Message}");
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
                    Log.Debug(moudle, $"订阅主题{topic}");
                    return true;
                }
                catch (Exception e)
                {
                    Log.Warn(moudle, $"订阅主题失败：{topic}");
                    Log.Warn(moudle, $"原因：{e.Message}");
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
                    Log.Warn(moudle, $"推送消息失败：{topic},{payload}");
                    Log.Warn(moudle, $"原因：{e.Message}");
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
            if (mqttClient.IsConnected)
            {
                Log.Info(moudle, "已连接，无需再次连接");
                return;
            }
            try
            {
                mqttClient.ConnectAsync(getOptions());
                Log.Info(moudle, $"已连接");
            }
            catch (Exception e)
            {
                Log.Warn(moudle, $"服务器连接失败：{Utils.Setting.MqttBroker}:{Utils.Setting.MqttPort}");
                Log.Warn(moudle, $"原因：{e.Message}");
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public static void Disconnect()
        {
            if (!mqttClient.IsConnected)
            {
                Log.Info(moudle, "没有连接服务器，无需再次断开");
                return;
            }
            if (mqttClient.IsConnected)
            {
                try
                {
                    mqttClient.DisconnectAsync();
                    Log.Info(moudle, $"已断开");
                }
                catch (Exception e)
                {
                    Log.Warn(moudle, $"断开连接失败");
                    Log.Warn(moudle, $"原因：{e.Message}");
                }
            }
        }
    }
}
