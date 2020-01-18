using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class LuaStates
    {
        //虚拟机池子
        private static ConcurrentDictionary<string, LuaTask.LuaEnv> states =
            new ConcurrentDictionary<string, LuaTask.LuaEnv>();
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
            lock (stateLock)
            {
                if (!states.ContainsKey(name))//没有的话就初始化池子
                {
                    states[name] = new LuaTask.LuaEnv();
                    states[name].ErrorEvent += (e,text) =>
                    {
                        Common.AppData.CQLog.Error(
                            "Lua插件报错",
                            $"虚拟机名称：{name},错误信息：{text}"
                        );
                    };
                    states[name].DoFile("head.lua");
                }
                states[name].addTigger(type, data);//运行
            }
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
                    states.TryRemove(k, out _);//移除
                }
            }
        }
    }
}