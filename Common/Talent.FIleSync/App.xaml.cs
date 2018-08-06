using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Talent.FIleSync
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //MsgFilter msg = new MsgFilter();
            //msg.ClosedApp+=new Action(Quit);
            //System.Windows.Forms.Application.AddMessageFilter(msg);
        }

        private void Quit()
        {
            MessageBox.Show("msg");
            //System.Windows.Forms.Application.Exit();
        }
    }
}
