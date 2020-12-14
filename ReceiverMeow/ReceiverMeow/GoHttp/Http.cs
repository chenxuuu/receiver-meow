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
        private static string host;
        private static int port;
        private static string module = "HTTP";


        public static void Set(string h, int p)
        {
            host = h;
            port = p;
            Log.Info(module, $"已设置HTTP接口 http://{h}:{p}");
        }

        public static string Send(string path, string json)
        {
            Log.Debug(module, $"发送{json}到{path}");
            var client = new RestClient();
            client.BaseUrl = new Uri($"http://127.0.0.1:5700/{path}");
            var request = new RestRequest(RestSharp.Method.POST);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = client.Execute(request);
            
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
                Log.Warn(module, $"HTTP返回状态码：{(int)response.StatusCode}");
            return Encoding.Default.GetString(response.RawBytes);
        }
    }
}
