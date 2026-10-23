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
        private static IMqttClient mqttClient = null;
        private static string moudle = "MQTT";

        /// <summary>
        /// 初始化各个参数
        /// </summary>
        public static void Initial()
        {
            if (mqttClient != null)
                return;
            mqttClient = factory.CreateMqttClient();
            //连接成功
            mqttClient.ConnectedAsync += e =>
            {
                Log.Info(moudle, "已连接");
                LuaEnv.LuaStates.Run("MQTT", "MQTT", new
                {
                    t = "connected"
                });
                return Task.CompletedTask;
            };

            //收到消息
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray());
                Log.Debug(moudle, $"收到消息：{e.ApplicationMessage.Topic} {payload}");
                LuaEnv.LuaStates.Run("MQTT", "MQTT", new
                {
                    t = "receive",
                    topic = e.ApplicationMessage.Topic,
                    payload = payload,
                    qos = (int)e.ApplicationMessage.QualityOfServiceLevel
                });
                return Task.CompletedTask;
            };

            //断线重连
            mqttClient.DisconnectedAsync += async e =>
            {
                Log.Warn(moudle, "MQTT连接已断开");
                if (!Utils.Setting.MqttEnable)
                    return;
                await Task.Delay(TimeSpan.FromSeconds(5));
                Log.Warn(moudle, "MQTT尝试重连");
                try
                {
                    await mqttClient.ConnectAsync(BuildOptions(), CancellationToken.None);
                }
                catch (Exception ee)
                {
                    Log.Warn(moudle, $"MQTT重连失败");
                    Log.Warn(moudle, $"原因： {ee.Message}");
                }
            };
        }

        /// <summary>
        /// 构造连接参数
        /// </summary>
        private static MqttClientOptions BuildOptions()
        {
            var op = new MqttClientOptionsBuilder()
                .WithClientId(Utils.Setting.ClientID)
                .WithTcpServer(Utils.Setting.MqttBroker, Utils.Setting.MqttPort)
                .WithCredentials(Utils.Setting.MqttUser, Utils.Setting.MqttPassword)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(Utils.Setting.KeepAlive));
            if (Utils.Setting.MqttTLS)
                op = op.WithTlsOptions(o => o.UseTls());
            return op.WithCleanSession().Build();
        }

        /// <summary>
        /// 连接状态
        /// </summary>
        /// <returns></returns>
        public static bool Status()
        {
            return mqttClient?.IsConnected ?? false;
        }

        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="qos"></param>
        /// <returns></returns>
        public static bool Subscribe(string topic, int qos)
        {
            if (mqttClient != null && mqttClient.IsConnected)
            {
                try
                {
                    var options = new MqttClientSubscribeOptionsBuilder()
                        .WithTopicFilter(f => f
                            .WithTopic(topic)
                            .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos))
                        .Build();
                    mqttClient.SubscribeAsync(options);
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
        public static bool Publish(string topic, string payload, long qos)
        {
            if (mqttClient != null && mqttClient.IsConnected)
            {
                try
                {
                    mqttClient.PublishStringAsync(topic, payload, (MqttQualityOfServiceLevel)qos);
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
            if (mqttClient == null)
                return;
            if (mqttClient.IsConnected)
            {
                Log.Info(moudle, "已连接，无需再次连接");
                return;
            }
            Task.Run(async () =>
            {
                try
                {
                    await mqttClient.ConnectAsync(BuildOptions(), CancellationToken.None);
                }
                catch (Exception e)
                {
                    Log.Warn(moudle, $"服务器连接失败：{Utils.Setting.MqttBroker}:{Utils.Setting.MqttPort}" +
                        $",{Utils.Setting.MqttUser},{Utils.Setting.MqttPassword},{Utils.Setting.MqttTLS}");
                    Log.Warn(moudle, $"原因：{e.Message}");
                }
            });
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public static void Disconnect()
        {
            if (mqttClient == null)
                return;
            if (!mqttClient.IsConnected)
            {
                Log.Info(moudle, "没有连接服务器，无需再次断开");
                return;
            }
            if (mqttClient.IsConnected)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await mqttClient.DisconnectAsync();
                    }
                    catch (Exception e)
                    {
                        Log.Warn(moudle, $"断开连接失败");
                        Log.Warn(moudle, $"原因：{e.Message}");
                    }
                });
            }
        }
    }
}
