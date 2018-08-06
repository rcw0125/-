using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Talent_LT.HelpClass;
using System.Diagnostics;
using System.Threading;

namespace ErrorAutoStart
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            
            this.timer1.Enabled = true;
            this.timer1.Start();
            FileHelpClass.WriteLog("启动自动检测程序");

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Hide();
            //WerFault    fire
            bool isExist = ProcessHelpClass.CheckContainsProcess("WerFault");
            if(isExist)
            {
                FileHelpClass.WriteLog("检测到WerFault.exe进程");
                ProcessHelpClass.ClearProcessContainsName("Talent");
                ProcessHelpClass.ClearProcessContainsName("WerFault");
                FileHelpClass.WriteLog("清理完成talent进程");
                string exePath = GetRestarExePath();
                FileHelpClass.WriteLog("重新开启："+exePath);
                Thread.Sleep(1000);
                if(!string.IsNullOrEmpty(exePath))
                {
                    ProcessHelpClass.ProcessOpenExe(exePath);
                }
               
            }
        }

        private string GetRestarExePath()
        {
            string rtStr = string.Empty;
            rtStr = FileHelpClass.TxtReaderToString(Application.StartupPath + "\\Config\\startExePath.txt");
            return rtStr;
        }
    }
}
