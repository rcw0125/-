using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.Keyboard.Controller;
using Talent.Keyboard.Interface;

namespace Talent.CarMeasureClient
{
    /// <summary>
    /// ConfirmMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmMessageBox : Window
    {
        private KeyBoardController ikb;
        private bool isOk;
        /// <summary>
        /// 是否确定
        /// </summary>
        public bool IsOk
        {
            get { return isOk; }
            set { isOk = value; }
        }


        public ConfirmMessageBox()
        {
            InitializeComponent();
            string logConfigPath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\SystemConfig.xml"; 
            this.KeyDown += Window_KeyDown;
            ikb = new KeyBoardController(logConfigPath);
            ikb.OnReceivedKeyData += ikb_OnReceivedKeyData;
            ikb.Open();
            ikb.Start();
            this.weightLabel.Content = 0;
            this.unitLabel.Content = 0;
        }

        public ConfirmMessageBox(string weight, string unit, string filepath)
        {
            InitializeComponent();
            this.KeyDown += Window_KeyDown;
            ikb = new KeyBoardController(filepath);
            ikb.OnReceivedKeyData += ikb_OnReceivedKeyData;
            ikb.Open();
            ikb.Start();
            this.weightLabel.Content = weight;
            this.unitLabel.Content = unit;
        }

        void ikb_OnReceivedKeyData(KeyDataType pType, string pData, KeyCommand pCommand)
        {
            if (pType == KeyDataType.COMMAND)//表示命令
            {
                ikb.Stop();
                ikb.Close();
                if (pCommand == KeyCommand.OK)//按了确定键
                {
                    IsOk = true;
                   // this.Close();
                }
                else
                {
                    IsOk = false;
                   // this.Close();
                }
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.Close();
                }));
            }
            else if (pType == KeyDataType.DATA)//按了取消键
            {
            }
        }

        /// <summary>
        /// 窗体按键事件
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.IsDown)
            //{
            //    if (e.Key==Key.Enter)
            //    {
            //        IsOk = true;
            //        this.Close();
            //    }
            //    else if (e.Key==Key.Escape)
            //    {
            //        IsOk = false;
            //        this.Close();
            //    }
            //}
        }
    }
}
