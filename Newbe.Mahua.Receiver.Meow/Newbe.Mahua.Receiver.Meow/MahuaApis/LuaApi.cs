using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class LuaApi
    {
        /// <summary>
        /// 获取图片对象
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="length">高度</param>
        /// <returns>图片对象</returns>
        public static Bitmap GetBitmap(int width, int length)
        {
            Bitmap bmp = new Bitmap(width, length);
            return bmp;
        }

        /// <summary>
        /// 摆放文字
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="text">文字内容</param>
        /// <param name="type">字体名称</param>
        /// <param name="size">字体大小</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        /// <returns>图片对象</returns>
        public static Bitmap PutText(Bitmap bmp,int x, int y, string text, string type="宋体", int size=9,
            int r=0,int g=0,int b=0)
        {
            Graphics pic = Graphics.FromImage(bmp);
            Font font = new Font(type, size);
            Color myColor = Color.FromArgb(r, g, b);
            SolidBrush myBrush = new SolidBrush(myColor);
            pic.DrawString(text, font, myBrush, new PointF() { X = x, Y = y });
            return bmp;
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">起始x坐标</param>
        /// <param name="y">起始y坐标</param>
        /// <param name="xx">结束x坐标</param>
        /// <param name="yy">结束y坐标</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        /// <returns>图片对象</returns>
        public static Bitmap putBlock(Bitmap bmp, int x, int y, int xx, int yy,
            int r = 0, int g = 0, int b = 0)
        {
            Color myColor = Color.FromArgb(r, g, b);
            //遍历矩形框内的各象素点
            for (int i = x; i <= xx; i++)
            {
                for (int j = y; j <= yy; j++)
                {
                    bmp.SetPixel(i, j, myColor);//设置当前象素点的颜色
                }
            }
            return bmp;
        }

        /// <summary>
        /// 摆放图片
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">起始x坐标</param>
        /// <param name="y">起始y坐标</param>
        /// <param name="path">图片路径</param>
        /// <param name="xx">摆放图片宽度</param>
        /// <param name="yy">摆放图片高度</param>
        /// <returns>图片对象</returns>
        public static Bitmap setImage(Bitmap bmp, int x,int y, string path, int xx=0,int yy=0)
        {
            if (!File.Exists(path))
                return bmp;
            Bitmap b = new Bitmap(path);
            Graphics pic = Graphics.FromImage(bmp);
            if(xx!=0&&yy!=0)
                pic.DrawImage(b, x, y, xx, yy);
            else
                pic.DrawImage(b, x, y);
            return bmp;
        }

        /// <summary>
        /// 保存并获取图片路径
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <returns>图片路径</returns>
        public static string GetDir(Bitmap bmp)
        {
            string result = Tools.GetRandomString(32, true, false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            bmp.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/download/lua" + result + ".jpg", ImageFormat.Jpeg);
            return "download/lua" + result + ".jpg";
        }

        /// <summary>
        /// 获取程序运行目录
        /// </summary>
        /// <returns>主程序运行目录</returns>
        public static string GetPath()
        {
            return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

    }
}
