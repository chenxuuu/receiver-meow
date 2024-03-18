using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram2MQTT;

internal class Program
{

    static MqttFactory factory = new MqttFactory();
    static IMqttClient mqttClient = factory.CreateMqttClient();

    static string botToken = "tg机器人的token";
    static string mqttAddress = "mqtt服务器地址";
    static int mqttPort = 1883;
    static string mqttUser = "用户名";
    static string mqttPassword = "密码";
    static string mqttClientId = "mqtt的客户端名";
    static string tg2meowTopic = "/tg/receive";
    static string meow2tgTopic = "/tg/send";

    static void Main(string[] args)
    {
        var botClient = new TelegramBotClient(botToken);

        //mqtt配置
        var mqttOptions = new MqttClientOptionsBuilder()
            .WithClientId(mqttClientId)
            .WithCredentials(mqttUser, mqttPassword)
            .WithKeepAlivePeriod(new TimeSpan(0, 0, 60))
            .WithCleanSession()
            .WithTcpServer(mqttAddress, mqttPort)
            .Build();

        //连上后订阅主题
        mqttClient.ConnectedAsync += async (m) =>
        {
            Console.WriteLine("Connected to MQTT Broker");
            MqttClientSubscribeOptions subscribeOptions = new();
            subscribeOptions.TopicFilters.Add(new MQTTnet.Packets.MqttTopicFilter()
            {
                Topic = meow2tgTopic
            });
            await mqttClient.SubscribeAsync(subscribeOptions);
        };

        //断线重连
        mqttClient.DisconnectedAsync += async (m) =>
        {
            Console.WriteLine("Disconnected from MQTT Broker");
            await Task.Delay(TimeSpan.FromSeconds(2));
            await mqttClient.ConnectAsync(mqttOptions);
        };

        //mqtt收到消息
        mqttClient.ApplicationMessageReceivedAsync += async (m) =>
        {
            var payload = m.ApplicationMessage.Payload;
            var message = System.Text.Encoding.UTF8.GetString(payload);
            var topic = m.ApplicationMessage.Topic;
            Console.WriteLine($"Received message '{message}' on topic '{topic}'");
            var msg = JsonConvert.DeserializeObject<dynamic>(message);
            long chatId = msg!.to;
            string text = msg.msg;
            await botClient.SendTextMessageAsync(chatId, text);
        };

        using CancellationTokenSource cts = new();
        Task.Run(async () =>
        {
            //连mqtt
            await mqttClient.ConnectAsync(mqttOptions, cts.Token);

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }).Wait();
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        await mqttClient.PublishStringAsync(
            tg2meowTopic,
            JsonConvert.SerializeObject(new
            {
                msg = messageText,
                from = chatId,
            }), 
            MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, 
            false, 
            cancellationToken);
        // Echo received message text
        //Message sentMessage = await botClient.SendTextMessageAsync(
        //    chatId: chatId,
        //    text: "我收到啦",
        //    cancellationToken: cancellationToken);
    }

    static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
