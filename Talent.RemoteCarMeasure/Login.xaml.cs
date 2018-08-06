using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Talent.RemoteCarMeasure.ViewModel;
using Talent_LT.HelpClass;

namespace Talent.RemoteCarMeasure
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        System.Windows.Media.Animation.Storyboard std = null;
        System.Windows.Media.Animation.Storyboard std2 = null;
        System.Windows.Media.Animation.Storyboard storyboardCar = null;
        public Login()
        {
            InitializeComponent();
            std = (System.Windows.Media.Animation.Storyboard)layoutroot.Resources["std"];
            std2 = (System.Windows.Media.Animation.Storyboard)layoutroot2.Resources["std2"];
            storyboardCar = (System.Windows.Media.Animation.Storyboard)this.Resources["StoryboardCar"];
            std.Completed += (t, r) => this.Close();
            std2.Completed += (t, r) => this.Close();
            this.layoutroot.Loaded += (sd, ee) =>
            {
            };
            this.Loaded += Login_Loaded;
            this.Run();
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            //CheckUpdate();//检查更新
            //程序版本
            //清理进程
            ClearOldProcess();
            DelteLogFile();
            LoginViewModel VM = this.DataContext as LoginViewModel;
            if (VM != null)
                VM.ReadUserInfo();

            //txtVer.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 3);

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string a = txtUserName.Text.Trim();
            string b = txtPassword.Password.Trim();
            
            if (!string.IsNullOrEmpty(txtUserName.Text.Trim()) && !string.IsNullOrEmpty(txtPassword.Password.Trim()))
            {
                var vm = this.DataContext as LoginViewModel;
                if (vm !=null)
                {
                    string rtStr= CheckIsAllowIP();
                    if(!string.IsNullOrEmpty(rtStr))
                    {
                        vm.ErrMsg = rtStr;
                        return;
                    }
                    vm.Password = b;
                    vm.UserName = a;
                    vm.LoginMethod();
                }               
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (std != null)
            {
                std.Begin();
            }
            //Application.Current.Shutdown();
        }

        private void LoginInfo(string flag)
        {
            if (flag == "" || flag == "成功")
            {
                this.Close();
            }
            else
            {
                lblMsg.Text = flag;
            }
        }

        public void Run()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister(this);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<string>(this, "LoginViewModel", new Action<string>(LoginInfo));
        }

        private void btnMinimize1_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose1_Click(object sender, RoutedEventArgs e)
        {
            if (std2 != null)
            {
                std2.Begin();
            }
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            StopLoginStd();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            BeginLoginStd();
        }

        /// <summary>
        /// 开始登录动画
        /// </summary>
        private void BeginLoginStd()
        {
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.To = -180d;
            this.axr.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
            storyboardCar.Begin();
        }

        /// <summary>
        /// 结束登录动画
        /// </summary>
        private void StopLoginStd()
        {
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.To = 0d;
            this.axr.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
            storyboardCar.Stop();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string a = txtUserName.Text.Trim();
                string b = txtPassword.Password.Trim();
                var vm = this.DataContext as LoginViewModel;
                if (vm != null)
                {
                    vm.Password = b;
                    vm.UserName = a;
                    vm.LoginMethod();
                }        
            }
        }

        /// <summary>
        /// 动画是否可用改变触发的事件
        /// </summary>
        private void StoryBoardTextBlock_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((this.DataContext as LoginViewModel).IsStoryReady)
            {
                if ((bool)e.NewValue)
                {
                    BeginLoginStd();
                }
                else
                {
                    StopLoginStd();
                }
            }
        }
        /// <summary>
        /// 检查更新
        /// </summary>
        private void CheckUpdate()
        {
            //MessageBox.Show("我更新了");
            string ftp = ConfigurationManager.AppSettings["FtpAddress"].ToString();
            string ftpU = ConfigurationManager.AppSettings["FtpUserName"].ToString();
            string ftpP = ConfigurationManager.AppSettings["FtpPassWord"].ToString();
            string param = ftp + "~" + ftpU + "~" + ftpP + "~" + "Talent.RemoteCarMeasure.exe";
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "/AutoUpdaterFtp.exe";
            Process myprocess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(path, param);
            myprocess.StartInfo = startInfo;

            //通过以下参数可以控制exe的启动方式，具体参照 myprocess.StartInfo.下面的参数，如以无界面方式启动exe等
            myprocess.StartInfo.UseShellExecute = false;
            myprocess.Start();
            myprocess.Close();
           
        }
        /// <summary>
        /// 清除进程
        /// </summary>
        private void  ClearOldProcess()
        {
            Process process = Process.GetCurrentProcess();//获取当前程序进程
            ProcessHelpClass.ClearProcess(process);
          
        }
        /// <summary>
        /// 定时删除日志
        /// </summary>
        private void DelteLogFile()
        {
            FileHelpClass fHClass = new FileHelpClass();
            fHClass.FileLogDelete(10,false);
        }
        /// <summary>
        /// 判断是不是允许IP
        /// </summary>
        /// <returns></returns>
        private string CheckIsAllowIP()
        {
            string rtStr = string.Empty;
            //string cIp = ComputerInfoHelpClass.GetIPAddress();
            //if (!string.IsNullOrEmpty(cIp) && cIp.Contains("."))
            //{
            //    string[] ipStr = cIp.Split('.');
            //    if (!ipStr[2].Equals("18"))
            //    {
            //        rtStr = "当前IP为：" + cIp + "非18段,不允许登录坐席";
            //    }
            //}
            //else
            //{
            //    rtStr = "获取IP为空,请检查是否多个网卡";
            //}
            return rtStr;
        }
    }
}
