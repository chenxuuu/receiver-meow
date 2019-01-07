using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
            string result = Tools.GetRandomString(32, true, false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            //在将文本写入文件前，处理文本行
            //StreamWriter一个参数默认覆盖
            //StreamWriter第二个参数为false覆盖现有文件，为true则把文本追加到文件末尾
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(codeFile, true))
            {
                file.WriteLine((result + ": /manuadd " + player + " builder world").Replace("\r", "").Replace("\n", ""));// 直接追加文件末尾，换行   
            }
            return result;
        }

        /// <summary>
        /// 删除白名单命令
        /// </summary>
        /// <param name="player"></param>
        public static string DelNewCode(string player)
        {
            string result = Tools.GetRandomString(32, true, false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            //在将文本写入文件前，处理文本行
            //StreamWriter一个参数默认覆盖
            //StreamWriter第二个参数为false覆盖现有文件，为true则把文本追加到文件末尾
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(codeFile, true))
            {
                file.WriteLine((result + ": /manudel " + player + " default world").Replace("\r", "").Replace("\n", ""));// 直接追加文件末尾，换行   
            }
            return result;
        }

        /// <summary>
        /// 获取任意命令
        /// </summary>
        /// <param name="player"></param>
        public static string GetCode(string cmd)
        {
            cmd = HttpUtility.HtmlDecode(cmd);
            string result = Tools.GetRandomString(32, true, false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            //在将文本写入文件前，处理文本行
            //StreamWriter一个参数默认覆盖
            //StreamWriter第二个参数为false覆盖现有文件，为true则把文本追加到文件末尾
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(codeFile, true))
            {
                file.WriteLine((result + ": /" + cmd).Replace("\r", "").Replace("\n", ""));// 直接追加文件末尾，换行   
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
                        return Tools.At(qq) + "你绑定的id为“" + msg.Replace("绑定", "").Replace(" ","空格") + "”，不符合规范，请换一个id绑定";
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
                if (msg == "激活")
                {
                    if (wait)
                    {
                        return Tools.At(qq) + "你还没通过审核呢，想什么呢";
                    }
                    else
                    {
                        return Tools.At(qq) + "请在服务器输入命令/code " + AddNewCode(player) + "来激活本账号\r\n" +
                            "命令只能使用一次，请尽快使用，失效可重新获取";
                    }
                }
                else if (msg == "催促审核")
                {
                    if (wait)
                    {
                        _mahuaApi.SendGroupMessage("567145439", "接待喵糖拌管理：\r\n玩家id：" + msg.Replace("绑定", "") + "\r\n进行了催促审核" + qq +
                                "\r\n请及时检查该玩家是否已经提交白名单申请https://wj.qq.com/mine.html" +
                                "\r\n如果符合要求，请回复“通过”+qq来给予白名单" +
                                "\r\n如果不符合要求，请回复“不通过”+qq+空格+原因来给打回去重填");
                        return Tools.At(qq) + "催促成功，还没回你就去找管理员";
                    }
                    else
                    {
                        return Tools.At(qq) + "你已经有权限了，发送“激活”来获取权限吧";
                    }
                }
                else if (msg == "签到" || msg.IndexOf("[CQ:sign,") != -1)
                {
                    string last_time = XmlSolve.xml_get("daily_mc_sign_in_time", qq);

                    if (last_time == System.DateTime.Today.ToString())
                        return Tools.At(qq) + "你今天已经签过到啦！";

                    string qdTimesStr = XmlSolve.xml_get("daily_sign_in_count", qq);
                    string CoinStr = XmlSolve.xml_get("money", qq);

                    int CoinsTemp, qdTimesTemp;

                    if (CoinStr != "")
                        CoinsTemp = int.Parse(CoinStr);
                    else
                        CoinsTemp = 0;

                    if (qdTimesStr != "")
                        qdTimesTemp = int.Parse(qdTimesStr);
                    else
                        qdTimesTemp = 1;

                    if (last_time == System.DateTime.Today.AddDays(-1).ToString())
                        qdTimesTemp++;
                    else
                        qdTimesTemp = 1;

                    Random ran = new Random(System.DateTime.Now.Millisecond);
                    int RandKey = ran.Next(100, 501);
                    CoinsTemp += RandKey + qdTimesTemp * 5;

                    XmlSolve.del("money", qq);
                    XmlSolve.del("daily_mc_sign_in_time", qq);
                    XmlSolve.del("daily_sign_in_count", qq);
                    XmlSolve.insert("money", qq, CoinsTemp.ToString());
                    XmlSolve.insert("daily_sign_in_count", qq, qdTimesTemp.ToString());
                    XmlSolve.insert("daily_mc_sign_in_time", qq, System.DateTime.Today.ToString());
                    Tools.AddXmlNumber("daily_sign_in_count_all", System.DateTime.Today.ToString(), 1);

                    return Tools.At(qq) + "\r\n签到成功！已连续签到" + qdTimesTemp.ToString() + "天\r\n获得游戏币" + RandKey + "+" + qdTimesTemp.ToString() + "*5枚！\r\n银行内游戏币" + CoinsTemp + "枚\r\n取钱请回复“取钱”加金额\r\n发送“查询”可查询余额";
                }
                else if (msg == "查询")
                {
                    string CoinStr = XmlSolve.xml_get("money", qq);
                    if (CoinStr != "")
                    {
                        return Tools.At(qq) + "你的群内余额为" + CoinStr;
                    }
                    else
                    {
                        return Tools.At(qq) + "你的群内余额为0";
                    }
                }
                else if (msg.IndexOf("取钱") == 0)
                {
                    int CoinsTemp = 0;
                    try
                    {
                        CoinsTemp = int.Parse(Tools.GetNumber(msg));
                    }
                    catch
                    {
                        return Tools.At(qq) + "格式错误！";
                    }
                    if (CoinsTemp == 0)
                        return Tools.At(qq) + "请输入大于0的数字";

                    string CoinStr = XmlSolve.xml_get("money", qq);
                    if (CoinStr != "")
                    {
                        int CoinsHave = int.Parse(CoinStr);
                        if (CoinsHave < CoinsTemp)
                            return Tools.At(qq) + "你的钱不够";

                        CoinsHave -= CoinsTemp;
                        XmlSolve.del("money", qq);
                        XmlSolve.insert("money", qq, CoinsHave.ToString());
                        _mahuaApi.SendPrivateMessage(qq, "请在服务器中使用/code " +
                            GetCode("eco give " + player + " " + CoinsTemp.ToString()) +
                            "来领取取出的" + CoinsTemp + "金币");
                        return Tools.At(qq) + "已取出" + CoinsTemp + "金币";
                    }
                    else
                    {
                        return Tools.At(qq) + "你的钱不够";
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
                int len = msg.Replace("不通过", "").IndexOf("：");
                if(len >= 0)
                {
                    player = XmlSolve.xml_get("bind_qq_wait", msg.Replace("不通过", "").Substring(0, len));
                    reason = msg.Replace("不通过", "").Substring(len + 1);
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
            else if (msg.IndexOf("命令") == 0)
            {
                return "命令/" + msg.Replace("命令", "") + "的执行码为：/code " + GetCode(msg.Replace("命令", ""));
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
