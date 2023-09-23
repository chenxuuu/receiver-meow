using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverMeow.LuaEnv
{
    class LuaStates
    {
        //虚拟机池子
        private static ConcurrentDictionary<string, LuaTask> states =
            new ConcurrentDictionary<string, LuaTask>();
        //池子操作锁
        private static object stateLock = new object();

        /// <summary>
        /// 添加一个触发事件
        /// 如果虚拟机不存在，则自动新建
        /// </summary>
        /// <param name="name">虚拟机名称</param>
        /// <param name="type">触发类型名</param>
        /// <param name="data">回调数据</param>
        public static void Run(string name, string type, object data)
        {
            Log.Info("NewLuaEnv", name);
            //检查文件是否存在
            if (!File.Exists(Utils.Path + "lua/main.lua"))
            {
                Log.Error("Lua",$"报错错虚拟机名称：{name}。没有找到入口脚本文件。文件路径应在{Utils.Path}lua/main.lua");
                return;
            }
            lock (stateLock)
            {
                if (!states.ContainsKey(name))//没有的话就初始化池子
                {
                    states[name] = new LuaTask();
                    states[name].ErrorEvent += (e, text) =>
                    {
                        Log.Warn(
                            "Lua插件报错",
                            $"虚拟机运行时错误。名称：{name},错误信息：{text}"
                        );
                    };
                    try
                    {
                        states[name].lua.LoadCLRPackage();
                        states[name].lua["LuaEnvName"] = name;
                        states[name].lua.DoFile(Utils.Path + "lua/main.lua");
                    }
                    catch(Exception e)
                    {
                        states[name].Dispose();
                        states.TryRemove(name, out _);
                        Log.Warn(
                            "Lua插件报错",
                            $"虚拟机启动时错误。名称：{name},错误信息：{e.Message}"
                        );
                        return;
                    }
                }
                Log.Debug("lua插件", $"触发事件{type}");
                states[name].addTigger(type, data);//运行
            }
        }
        public static void Run(long name, string type, object data)
        {
            Run(name.ToString(), type, data);
        }

        /// <summary>
        /// 清空池子
        /// </summary>
        public static void Clear()
        {
            lock (stateLock)
            {
                foreach(string k in states.Keys)
                {
                    Log.Info("Lua插件", "已释放虚拟机" + k);
                    LuaTask l;
                    states.TryRemove(k, out l);//取出
                    l.Dispose();//释放
                }
                Log.Info("Lua插件", "所有虚拟机均已释放");
            }
        }

        public static string[] GetList()
        {
            lock (stateLock)
            {
                return states.Keys.ToArray();
            }
        }
    }
}
