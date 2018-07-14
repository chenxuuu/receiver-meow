using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace qqpet
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    //if(DateTime.Now.Minute%5 == 0 && DateTime.Now.Second == 0)
                    //{
                    Console.WriteLine("开始时间：" + DateTime.Now);
                    XElement uin = XElement.Load(path + "11.xml");
                    foreach (XElement mm in uin.Elements("msginfo"))
                    {
                        if (mm.Element("msg").Value == "初始问题")
                            continue;
                        Console.WriteLine("-------------------\r\n当前账号：" + mm.Element("msg").Value);
                        AutoFeed(mm.Element("ans").Value, replay_get(12, mm.Element("msg").Value), mm.Element("msg").Value);
                    }
                    Console.WriteLine("定时程序执行结束：" + DateTime.Now + "\r\n===========================================");
                    //}
                    //Console.WriteLine("not run,"+ DateTime.Now.Minute + "," + DateTime.Now.Second);
                    System.Threading.Thread.Sleep(10000);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }

        }

        static string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public static string replay_get(long group, string msg)
        {
            XElement root = XElement.Load(path + group + ".xml");
            string[] replay_str = new string[50];
            int count = 0;
            Random ran = new Random(System.DateTime.Now.Millisecond);
            int RandKey;
            string ansall = "";
            foreach (XElement mm in root.Elements("msginfo"))
            {
                if (msg.IndexOf(mm.Element("msg").Value) != -1 && count < 50)
                {
                    replay_str[count] = mm.Element("ans").Value;
                    count++;
                }
            }
            if (count != 0)
            {
                RandKey = ran.Next(0, count);
                ansall = replay_str[RandKey];
            }

            return ansall;
        }
        public static void del(long group, string msg)
        {
            dircheck(group);
            string gg = group.ToString();
            XElement root = XElement.Load(path + group + ".xml");

            var element = from ee in root.Elements()
                          where (string)ee.Element("msg") == msg
                          select ee;
            if (element.Count() > 0)
            {
                //element.First().Remove();
                element.Remove();
            }
            root.Save(path + group + ".xml");
        }
        public static void insert(long group, string msg, string ans)
        {
            if (msg.IndexOf("\r\n") < 0 & msg != "")
            {
                dircheck(group);
                XElement root = XElement.Load(path + group + ".xml");

                XElement read = root.Element("msginfo");

                read.AddBeforeSelf(new XElement("msginfo",
                       //new XElement("group", group),
                       new XElement("msg", msg),
                       new XElement("ans", ans)
                       ));

                root.Save(path + group + ".xml");
            }
        }
        public static void createxml(long group)
        {
            XElement root = new XElement("Categories",
                new XElement("msginfo",
                    //new XElement("group", 123),
                    new XElement("msg", "初始问题"),
                    new XElement("ans", "初始回答")
                    )
               );
            root.Save(path + group + ".xml");
        }
        public static void dircheck(long group)
        {
            if (File.Exists(path + group + ".xml"))
            {
                //MessageBox.Show("存在文件");
                //File.Delete(dddd);//删除该文件
            }
            else
            {
                //MessageBox.Show("不存在文件");
                createxml(group);//创建该文件，如果路径文件夹不存在，则报错。
            }
        }

        public static string pleaseLogin = "请设置登陆信息！";
        public static string petMore = "更多宠物命令请回复“宠物助手”";

        public static void AutoFeed(string uin, string skey, string qq)
        {
            string state = GetPetState(uin, skey);
            int growNow = 0, growMax = 0, cleanNow = 0, cleanMax = 0, level = 0;
            try
            {
                growNow = int.Parse(Reg_get(state, "饥饿值：(?<w>\\d+)/", "w"));
                growMax = int.Parse(Reg_get(state, "饥饿值：\\d+/(?<w>\\d+)", "w"));
                cleanNow = int.Parse(Reg_get(state, "清洁值：(?<w>\\d+)/", "w"));
                cleanMax = int.Parse(Reg_get(state, "清洁值：\\d+/(?<w>\\d+)", "w"));
                level = int.Parse(Reg_get(state, "等级：(?<w>\\d+)", "w"));
            }
            catch { }
            if (level == 0)
            {
                int failCount = 0;
                try
                {
                    failCount = int.Parse(replay_get(14, qq.ToString()));
                }
                catch { }
                failCount++;
                del(14, qq.ToString());
                insert(14, qq.ToString(), failCount.ToString());
                Console.WriteLine($"连续失败{failCount}次");
                return;
            }
            else
            {
                del(14, qq.ToString());
                insert(14, qq.ToString(), "0");
            }
                
            Console.WriteLine($"饥饿值:{growNow}/{growMax},清洁值:{cleanNow}/{cleanMax}");
            if (growMax - growNow > 1500)
            {
                string grow = FeedPetSelect(uin, skey, "1");
                string goodid = Reg_get(grow, "物品id：(?<w>\\d+)", "w");
                if (goodid == "")
                {
                    HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&feed=5&goodid=5629937620877313", uin, skey);
                    goodid = "5629937620877313";
                    Console.WriteLine($"购买了物品{goodid}，进行饥饿值补充");
                }
                else
                {
                    Console.WriteLine($"选取了物品{goodid}，进行饥饿值补充");
                }
                UsePet(uin, skey, goodid);
            }
            if (cleanMax - cleanNow > 1500)
            {
                string grow = WashPetSelect(uin, skey, "1");
                string goodid = Reg_get(grow, "物品id：(?<w>\\d+)", "w");
                if(goodid == "")
                {
                    HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&feed=5&goodid=3378137807192066", uin, skey);
                    goodid = "3378137807192066";
                    Console.WriteLine($"购买了物品{goodid}，进行清洁值值补充");
                }
                else
                {
                    Console.WriteLine($"选取了物品{goodid}，进行清洁值值补充");
                }
                UsePet(uin, skey, goodid);
            }

            if(HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10", uin, skey).IndexOf("一键收获") == -1)
            {
                Console.WriteLine("开始播种植物");
                if (level >= 20)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10&prc=81&seedid=14&subcmd=5&sname=s8jX0w", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 18)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10&prc=57&seedid=13&subcmd=12&sname=z*O9tg", uin, skey).IndexOf("系统繁忙") != -1) ;
                if (level >= 14)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10&prc=34&seedid=11&subcmd=12&sname=st3drg", uin, skey).IndexOf("系统繁忙") != -1) ;
                if (level >= 12)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10&prc=24&seedid=10&subcmd=12&sname=xru5*w", uin, skey).IndexOf("系统繁忙") != -1) ;
                if (level >= 10)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10&prc=24&seedid=9&subcmd=5&sname=xM*5zw", uin, skey).IndexOf("系统繁忙") != -1) ;
                if (level >= 5)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10&prc=14&seedid=6&subcmd=5&sname=t6zH0Q", uin, skey).IndexOf("系统繁忙") != -1) ;
                if (level >= 4)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10&prc=12&seedid=5&subcmd=12&sname=x9HX0w", uin, skey).IndexOf("系统繁忙") != -1) ;
                if (level >= 1)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10&prc=4&seedid=1&subcmd=12&sname=xMGy3Q", uin, skey).IndexOf("系统繁忙") != -1) ;
                Console.WriteLine("植物播种完成");
            }
            else
            {
                Console.WriteLine("尝试收获植物");
                HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/farm", "cmd=10&subcmd=10", uin, skey);
            }

            if (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11", uin, skey).IndexOf("一键收获") == -1)
            {
                Console.WriteLine("开始投放鱼苗");
                if (level >= 55)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=32&prc=140&sname=sNTN9dPj", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 50)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=31&prc=120&sname=zuXJq9Pj", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 34)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=23&prc=81&sname=tPPA9rrszrLT4w", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 32)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=28&prc=73&sname=wt66utPj", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 30)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=21&prc=65&sname=y-PT4w", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 25)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=20&prc=63&sname=0KHB*s*6", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 16)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=16&prc=32&sname=yq*w39Pj", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 14)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=11&prc=40&sname=u6LGpNPj", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 12)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=15&prc=27&sname=vaPT4w", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 4)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=5&prc=8&sname=tMzQ6-bz0*M", uin, skey).IndexOf("系统繁忙") != -1);
                if (level >= 2)
                    while (HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=12&fryid=1&prc=4&sname=x**1ttPj", uin, skey).IndexOf("系统繁忙") != -1);
                Console.WriteLine("鱼苗投放完成");
            }
            else
            {
                Console.WriteLine("尝试收获鱼苗");
                HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/fish", "cmd=11&fish_sub=10&fryid=1", uin, skey);
            }

            if (Reg_get(state, "状态：(?<w>..)", "w") == "空闲")
            {
                bool can_study = false;
                int c = 1;
                try
                {
                    c = int.Parse(replay_get(13, qq));
                }
                catch { }
                if(c < 28)
                {
                    if (c == 0)
                        c = 1;
                    Console.WriteLine($"宠物状态发现为空闲，从第{c}课开始尝试");

                    for (int i = c; i <= 27; i++)
                    {
                        int r = 4;
                        while (r == 4)
                        {
                            r = StudyPet(uin, skey, i.ToString());
                            //Console.WriteLine($"正在尝试，返回值为{r}");
                        }

                        if(r == 1 || r == 3)
                        {
                            can_study = true;
                            Console.WriteLine($"宠物已开始学习{i}课");
                            //del(13, qq);
                            //insert(13, qq, i.ToString());
                            break;
                        }
                        else if(r == 2)
                        {
                            Console.WriteLine($"无法上{i}课，尝试下一课");
                        }
                        else if(r == 5)
                        {
                            Console.WriteLine($"请求被拦截，下次再试");
                            break;
                        }
                    }
                }
                if(!can_study)
                {
                    Console.WriteLine($"发现没课能上了，去旅游");
                    HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "g_f=0&cmd=7&tour=4", uin, skey);
                }
            }

        }

        /// <summary>
        /// 获取宠物基本信息
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string GetPetState(string uin, string skey)
        {
            string result = "";
            string html = HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1)
                return pleaseLogin;
            result += Reg_get(html, "alt=\"QQ宠物\"/></p><p>(?<say>.*?)<", "say").Replace(" ", "") + "\r\n"
                    + "宠物名：" + Reg_get(html, "昵称：(?<level>.*?)<", "level").Replace(" ", "") + "\r\n"
                    + "等级：" + Reg_get(html, "等级：(?<level>.*?)<", "level").Replace(" ", "") + "\r\n"
                    + "状态：" + Reg_get(html, "状态：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "成长值：" + Reg_get(html, "成长值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "饥饿值：" + Reg_get(html, "饥饿值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "清洁值：" + Reg_get(html, "清洁值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "心情：" + Reg_get(html, "心情：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n" + petMore;
            return result;
        }

        /// <summary>
        /// 获取宠物详细信息
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string GetPetMore(string uin, string skey)
        {
            string result = "";
            string html = HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=2", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1)
                return pleaseLogin;
            result += "主人昵称：" + Reg_get(html, "主人昵称：(?<name>.*?)&nbsp;", "name").Replace(" ", "") + "\r\n"
                    + "宠物性别：" + Reg_get(html, "宠物性别：(?<level>.*?)<", "level").Replace(" ", "") + "\r\n"
                    + "生日：" + Reg_get(html, "生日：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "等级：" + Reg_get(html, "等级：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "年龄：" + Reg_get(html, "年龄：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "代数：" + Reg_get(html, "代数：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "成长值：" + Reg_get(html, "成长值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "武力值：" + Reg_get(html, "武力值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "智力值：" + Reg_get(html, "智力值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "魅力值：" + Reg_get(html, "魅力值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "活跃度：" + Reg_get(html, "活跃度：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "配偶：" + Reg_get(html, "配偶：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + petMore;
            return result;
        }

        /// <summary>
        /// 喂养宠物选择页面
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string FeedPetSelect(string uin, string skey,string page)
        {
            string result = "";
            string html = HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&page=" + page, uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1)
                return pleaseLogin;
            result += "我的账户：" + Reg_get(html, "我的账户：(?<name>.*?)<", "name").Replace(" ", "") + "\r\n";
            int i = 0;
            MatchCollection matchs = Reg_solve(Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "<p>(?<name>.*?)<br");
            string[] name = new string[matchs.Count];
            string[] id = new string[matchs.Count];
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    name[i++] += item.Groups["name"].Value;
                }
            }
            i = 0;
            matchs = Reg_solve(Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "goodid=(?<id>.*?)\"");
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    id[i++] += "物品id：" + item.Groups["id"].Value;
                }
            }
            
            for(int j = 0; j < i;j++)
            {
                result += name[j] + "\r\n" + id[j] + "\r\n";
            }
            if(Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") != "")
            {
                result += "第" + Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                       + Reg_get(html, "上页</a>(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }
            else
            {
                result += "第" + Reg_get(html, "上页(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                        + Reg_get(html, "上页(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }


            return result + petMore;
        }

        /// <summary>
        /// 清洁宠物选择页面
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string WashPetSelect(string uin, string skey, string page)
        {
            string result = "";
            string html = HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&page=" + page + "&eatwash=1", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1)
                return pleaseLogin;
            result += "我的账户：" + Reg_get(html, "我的账户：(?<name>.*?)<", "name").Replace(" ", "") + "\r\n";
            int i = 0;
            MatchCollection matchs = Reg_solve(Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "<p>(?<name>.*?)<br");
            string[] name = new string[matchs.Count];
            string[] id = new string[matchs.Count];
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    name[i++] += item.Groups["name"].Value;
                }
            }
            i = 0;
            matchs = Reg_solve(Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "goodid=(?<id>.*?)\"");
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    id[i++] += "物品id：" + item.Groups["id"].Value;
                }
            }

            for (int j = 0; j < i; j++)
            {
                result += name[j] + "\r\n" + id[j] + "\r\n";
            }
            if (Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") != "")
            {
                result += "第" + Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                       + Reg_get(html, "上页</a>(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }
            else
            {
                result += "第" + Reg_get(html, "上页(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                        + Reg_get(html, "上页(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }

            return result + petMore;
        }

        /// <summary>
        /// 治疗宠物选择页面
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string CurePetSelect(string uin, string skey, string page)
        {
            string result = "";
            string html = HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&page=" + page + "&eatwash=2", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1)
                return pleaseLogin;
            result += "我的账户：" + Reg_get(html, "我的账户：(?<name>.*?)<", "name").Replace(" ", "") + "\r\n";
            int i = 0;
            MatchCollection matchs = Reg_solve(Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "<p>(?<name>.*?)<br");
            string[] name = new string[matchs.Count];
            string[] id = new string[matchs.Count];
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    name[i++] += item.Groups["name"].Value;
                }
            }
            i = 0;
            matchs = Reg_solve(Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "goodid=(?<id>.*?)\"");
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    id[i++] += "物品id：" + item.Groups["id"].Value;
                }
            }

            for (int j = 0; j < i; j++)
            {
                result += name[j] + "\r\n" + id[j] + "\r\n";
            }
            if (Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") != "")
            {
                result += "第" + Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                       + Reg_get(html, "上页</a>(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }
            else
            {
                result += "第" + Reg_get(html, "上页(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                        + Reg_get(html, "上页(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }

            return result + petMore;
        }

        /// <summary>
        /// 使用宠物物品
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string UsePet(string uin, string skey, string goodid)
        {
            string result = "";
            string html = HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&feed=3&goodid=" + goodid, uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1)
                return pleaseLogin;
            if (html.IndexOf("不合法") != -1)
                return "你没有这个东西，使用失败\r\n" + petMore;
            result += "物品使用结果：\r\n" + Reg_get(html, "值：(?<name>.*?)<br", "name").Replace(" ", "").Replace("</b>", "") + "\r\n"
                    + petMore;
            return result;
        }

        /// <summary>
        /// 宠物学习选择页面
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string StudyPetSelect()
        {
            return "课程id：\r\n小学体育：1\r\n小学劳技：2\r\n小学武术：5\r\n小学语文：4\r\n小学数学：5\r\n小学政治：6\r\n小学美术：7\r\n小学音乐：8\r\n小学礼仪：9\r\n\r\n中学体育：10\r\n中学劳技：11\r\n中学武术：12\r\n中学语文：13\r\n中学数学：14\r\n中学政治：15\r\n中学美术：16\r\n中学音乐：17\r\n中学礼仪：18\r\n\r\n大学体育：19\r\n大学劳技：20\r\n大学武术：21\r\n大学语文：22\r\n大学数学：23\r\n大学政治：24\r\n大学美术：25\r\n大学音乐：26\r\n大学礼仪：27\r\n" + petMore;
        }

        /// <summary>
        /// 学习宠物课程
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static int StudyPet(string uin, string skey, string classid)
        {
            string html = HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=5&courseid=" + classid + "&study=2", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1)
                return 0;
            if (html.IndexOf("亲爱的") != -1 && html.IndexOf("很抱歉") == -1)
            {
                //上课成功
                return 1;
            }
            else if (html.IndexOf("您已经完成了") != -1 || html.IndexOf("很抱歉") != -1)
            {
                //已经上过这个课了
                return 2;
            }
            else if (html.IndexOf("你的宠物") != -1)
            {
                //正在上课
                return 3;
            }
            else if (html.IndexOf("不合法") != -1)
            {
                //不合法
                return 5;
            }
            else
            {
                //获取失败
                return 4;
            }

            //return "课程学习结果：\r\n" + result + petMore;
        }

        /// <summary>
        /// 直接获取正则表达式的最后一组匹配值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regstr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Reg_get(string str, string regstr, string name)
        {
            string result = "";
            MatchCollection matchs = Reg_solve(str, regstr);
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    result = item.Groups[name].Value;
                }
            }
            return result;
        }

        public static MatchCollection Reg_solve(string str, string regstr)
        {
            Regex reg = new Regex(regstr, RegexOptions.IgnoreCase);
            return reg.Matches(str);
        }


        /// <summary>  
        /// GET 请求与获取结果  
        /// </summary>  
        public static string HttpGetPet(string Url, string postDataStr, string uin, string skey)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Headers.Add("cookie", "uin=" + uin + "; skey=" + skey);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);

                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch { }
            return "";
        }
    }
}
