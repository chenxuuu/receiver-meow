using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverMeow
{
    class Settings
    {
        private bool _colorful = true;


        /// <summary>
        /// 保存配置
        /// </summary>
        private void Save()
        {
            File.WriteAllText(Tools.Path + "settings.json", JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// 终端是否开启彩色界面
        /// </summary>
        public bool Colorful
        {
            get => _colorful;
            set
            {
                _colorful = value;
                Save();
            }
        }
    }
}
