using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReceiverMeow.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IMenuCall
    {
        private MainWindow window = null;

        public void MenuCall(object sender, CQMenuCallEventArgs e)
        {
            if (window == null)
            {
                window = new MainWindow();
                window.Closing += (ss, ee) =>
                {
                    window = null;
                };
                window.Show();
            }
            else
            {
                window.Activate();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Global.Settings;
        }

        private void InitialButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("点击确定就会开始下载，本窗口可能会卡顿，下载进度请见酷Q日志窗口");
            Global.InitialScript();
            MessageBox.Show("初始化脚本操作执行完毕，执行结果请见酷Q日志窗口");
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Global.InitialLua();
            MessageBox.Show("所有虚拟机均已重置完成！");
        }
    }
}
