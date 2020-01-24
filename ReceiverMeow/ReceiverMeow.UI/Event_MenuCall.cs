using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverMeow.UI
{
    public class Event_MenuCall : IMenuCall
    {
        private MainWindow window = null;

        public void MenuCall(object sender, CQMenuCallEventArgs e)
        {
            if(window == null)
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
    }
}
