using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class MinecraftSolve
    {
        //命令文件位置
        private static string codeFile = @"D:\server1.12\plugins\CommandCode\Codes.yml";

        /// <summary>
        /// 新增命令
        /// </summary>
        /// <param name="player"></param>
        private static string AddNewCode(string player)
        {
            string result = Tools.GetRandomString(40, true, false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            //在将文本写入文件前，处理文本行
            //StreamWriter一个参数默认覆盖
            //StreamWriter第二个参数为false覆盖现有文件，为true则把文本追加到文件末尾
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(codeFile, true))
            {
                file.WriteLine(result + ": /manuadd " + player + " builder world");// 直接追加文件末尾，换行   
            }
            return result;
        }

        /// <summary>
        /// 删除白名单命令
        /// </summary>
        /// <param name="player"></param>
        public static string DelNewCode(string player)
        {
            string result = Tools.GetRandomString(40, true, false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            //在将文本写入文件前，处理文本行
            //StreamWriter一个参数默认覆盖
            //StreamWriter第二个参数为false覆盖现有文件，为true则把文本追加到文件末尾
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(codeFile, true))
            {
                file.WriteLine(result + ": /manudel " + player);// 直接追加文件末尾，换行   
            }
            return result;
        }



        /// <summary>
        /// 处理玩家群消息
        /// </summary>
        /// <param name="qq"></param>
        /// <param name="msg"></param>
        /// <param name="_mahuaApi"></param>
        /// <returns></returns>
        public static string SolvePlayer(string qq,string msg, IMahuaApi _mahuaApi)
        {
            bool wait = true;
            string player = XmlSolve.xml_get("bind_qq_wait", qq);
            if(player == "")
            {
                player = XmlSolve.xml_get("bind_qq", qq);
                wait = false;
            }
            if(player == "")
            {
                if(msg.IndexOf("绑定") != 0)
                {
                    return Tools.At(qq) + "检测到你还没有绑定游戏账号，请发送“绑定”加自己的游戏id来绑定账号";
                }
                else
                {
                    if (!CheckID(msg.Replace("绑定", "")))
                    {
                        return Tools.At(qq) + "你绑定的id为“" + msg.Replace("绑定", "") + "”，不符合规范，请换一个id绑定";
                    }
                    else
                    {
                        XmlSolve.insert("bind_qq_wait", qq, msg.Replace("绑定", ""));
                        _mahuaApi.SendGroupMessage("567145439", "接待喵糖拌管理：\r\n玩家id：" + msg.Replace("绑定", "") + "\r\n已成功绑定QQ：" + qq +
                                                        "\r\n请及时检查该玩家是否已经提交白名单申请https://wj.qq.com/mine.html" +
                                                        "\r\n如果符合要求，请回复“通过”+qq来给予白名单" +
                                                        "\r\n如果不符合要求，请回复“不通过”+qq+空格+原因来给打回去重填");
                        return Tools.At(qq) + "绑定id:" + msg.Replace("绑定", "") + "成功！" +
                                                      "\r\n请耐心等待管理员审核白名单申请哟~" +
                                                      "\r\n如未申请请打开此链接：https://wj.qq.com/s/1308067/143c" +
                                                      "\r\n如果过去24小时仍未被审核，请回复“催促审核”来进行催促";
                    }
                }
            }
            else  //绑定过的情况下
            {
                if(msg == "激活")
                {
                    if(wait)
                    {
                        return Tools.At(qq) + "你还没通过审核呢，想什么呢";
                    }
                    else
                    {
                        return Tools.At(qq) + "请在服务器输入命令/code " + AddNewCode(player) + "来激活本账号\r\n" +
                            "命令只能使用一次，请尽快使用，失效可重新获取";
                    }
                }
                else if(msg=="催促审核")
                {
                    if (wait)
                    {
                        return Tools.At(qq) + "你还不如直接at管理员呢，自己去问吧";
                    }
                    else
                    {
                        return Tools.At(qq) + "你已经有权限了，发送“激活”来获取权限吧";
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 处理管理群消息
        /// </summary>
        /// <param name="qq"></param>
        /// <param name="msg"></param>
        /// <param name="_mahuaApi"></param>
        /// <returns></returns>
        public static string SolveAdmin(string qq, string msg, IMahuaApi _mahuaApi)
        {
            if (msg.IndexOf("删除") == 0)
            {
                if (msg.Replace("删除", "") != "")
                {
                    XmlSolve.del("bind_qq_wait", msg.Replace("删除", ""));
                    XmlSolve.del("bind_qq", msg.Replace("删除", ""));
                    return "已删除QQ：" + msg.Replace("删除", "") + "所绑定的id。";
                }
            }
            else if (msg.IndexOf("通过") == 0)
            {
                string player = XmlSolve.xml_get("bind_qq_wait", msg.Replace("通过", ""));
                if (player != "")
                {
                    XmlSolve.del("bind_qq_wait", msg.Replace("通过", ""));
                    XmlSolve.insert("bind_qq", msg.Replace("通过", ""), player);
                    _mahuaApi.SendGroupMessage("241464054", Tools.At(msg.Replace("通过", "")) + "你的白名单申请已经通过了哟~" +
                                                                            "\r\n在群里发送“激活”即可获取激活账号的方法哦~" +
                                                                            "\r\n你的id：" + player);
                    return "已通过QQ：" + msg.Replace("通过", "") + "，id：" + player + "的白名单申请";
                }
                else
                {
                    return "参数不对或该玩家不在待审核玩家数据中";
                }

            }
            else if (msg.IndexOf("不通过") == 0)
            {
                if (msg.IndexOf("不通过 ") == 0)
                {
                    return "命令关键字后不要加空格，直接加玩家id";
                }
                string player = "";
                string reason = "";
                string qq_get = "";
                string[] str2;
                int count_temp = 0;
                str2 = msg.Replace("不通过", "").Split(' ');
                foreach (string i in str2)
                {
                    if (count_temp == 0)
                    {
                        player = XmlSolve.xml_get("bind_qq_wait", i);
                        qq_get = i;
                        count_temp++;
                    }
                    else if (count_temp == 1)
                    {
                        reason += i + " ";
                    }
                }

                if (player != "")
                {
                    _mahuaApi.SendGroupMessage("241464054", Tools.At(qq_get) + "你的白名单申请并没有通过。" +
                                                                            "\r\n原因：" + reason +
                                                                            "\r\n请按照原因重新填写白名单：https://wj.qq.com/s/1308067/143c" +
                                                                            "\r\n你的id：" + player);
                    return "已不通过QQ：" + qq_get + "，id：" + player + "的白名单申请，原因：" + reason;
                }
                else
                {
                    return "参数不对或该玩家不在待审核玩家数据中";
                }
            }
            else if (msg == "空间")
            {
                return
                    "服务器盘剩余空间：" + ((float)GetHardDiskFreeSpace("D") / 1024).ToString(".00") + "GB\r\n" +
                    "备份盘剩余空间：" + ((float)GetHardDiskFreeSpace("E") / 1024).ToString(".00") + "GB";
            }
            else if (msg == "即时备份" && qq == "961726194")
            {
                System.Diagnostics.Process.Start(@"D:\backup.bat");
                return "备份任务已开始";  //567145439
            }
            return "";
        }

        /// <summary>
        /// 检查id合法性
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool CheckID(string t)
        {
            t = t.ToUpper();
            if (t == "" || t == "ID")
                return false;
            t = t.Replace("A", "");
            t = t.Replace("B", "");
            t = t.Replace("C", "");
            t = t.Replace("D", "");
            t = t.Replace("E", "");
            t = t.Replace("F", "");
            t = t.Replace("G", "");
            t = t.Replace("H", "");
            t = t.Replace("I", "");
            t = t.Replace("J", "");
            t = t.Replace("K", "");
            t = t.Replace("L", "");
            t = t.Replace("M", "");
            t = t.Replace("N", "");
            t = t.Replace("O", "");
            t = t.Replace("P", "");
            t = t.Replace("Q", "");
            t = t.Replace("R", "");
            t = t.Replace("S", "");
            t = t.Replace("T", "");
            t = t.Replace("U", "");
            t = t.Replace("V", "");
            t = t.Replace("W", "");
            t = t.Replace("X", "");
            t = t.Replace("Y", "");
            t = t.Replace("Z", "");
            t = t.Replace("_", "");
            t = t.Replace("1", "");
            t = t.Replace("2", "");
            t = t.Replace("3", "");
            t = t.Replace("4", "");
            t = t.Replace("5", "");
            t = t.Replace("6", "");
            t = t.Replace("7", "");
            t = t.Replace("8", "");
            t = t.Replace("9", "");
            t = t.Replace("0", "");
            if (t == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        ///  <summary> 
        /// 获取指定驱动器的剩余空间总大小(单位为MB) 
        ///  </summary> 
        ///  <param name="str_HardDiskName">只需输入代表驱动器的字母即可 </param> 
        ///  <returns> </returns> 
        public static long GetHardDiskFreeSpace(string str_HardDiskName)
        {
            long freeSpace = new long();
            str_HardDiskName = str_HardDiskName + ":\\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    freeSpace = drive.TotalFreeSpace / (1024 * 1024);
                }
            }
            return freeSpace;
        }
    }
}
