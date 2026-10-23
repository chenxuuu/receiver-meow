using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverMeow.GoHttp
{
    class Http
    {
        private static string host = "127.0.0.1";
        private static int port = 5700;
        private static string module = "HTTP";

        /// <summary>
        /// 复用的http客户端，避免高频发消息时反复建连接
        /// </summary>
        private static readonly RestClient client = new RestClient(new RestClientOptions
        {
            Timeout = TimeSpan.FromSeconds(100)
        });


        public static void Set(string h, int p)
        {
            host = h;
            port = p;
            Log.Info(module, $"已设置HTTP接口 http://{h}:{p}");
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string Send(string path, string json)
        {
            Log.Debug(module, $"发送{json}到{path}");
            var request = new RestRequest($"http://{host}:{port}/{path}", Method.Post);
            request.AddStringBody(json, "application/json");
            var response = client.Execute(request);

            if(response.StatusCode != System.Net.HttpStatusCode.OK)
                Log.Warn(module, $"HTTP返回状态码：{(int)response.StatusCode}");
            return Encoding.UTF8.GetString(response.RawBytes ?? Array.Empty<byte>());
        }

        /// <summary>
        /// 获取json
        /// </summary>
        /// <param name="path"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JObject fetch(string path, string json)
        {
            return JsonConvert.DeserializeObject<JObject>(Send(path, json));
        }
    }
}
