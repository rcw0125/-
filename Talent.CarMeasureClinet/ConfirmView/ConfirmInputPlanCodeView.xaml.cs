using Newtonsoft.Json;
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
using System.Windows.Shapes;
using Talent.Keyboard.Controller;
using Talent.Keyboard.Interface;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.CarMeasureClient.ConfirmView
{
    public delegate void FormCLoseHandler();
    /// <summary>
    /// ConfirmInputPlanCodeView.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmInputPlanCodeView : Window
    {
        public event FormCLoseHandler FormCloseEvent;
        /// <summary>
        /// 键盘控制类
        /// </summary>
        private KeyBoardController ikb;
        private string bussinessNo;
        /// <summary>
        /// 业务号
        /// </summary>
        public string BussinessNo
        {
            get { return bussinessNo; }
            set { bussinessNo = value; }
        }

        private bool isHelp;
        /// <summary>
        /// 是否求助
        /// </summary>
        public bool IsHelp
        {
            get { return isHelp; }
            set { isHelp = value; }
        }

        private bool isClear;
        /// <summary>
        /// 是否为IsClear(暂用为标记计皮业务)
        /// </summary>
        public bool IsClear
        {
            get { return isClear; }
            set { isClear = value; }
        }

        private bool isOk;
        /// <summary>
        /// 是否确定
        /// </summary>
        public bool IsOk
        {
            get { return isOk; }
            set { isOk = value; }
        }

        private bool isClose;
        /// <summary>
        /// 关闭
        /// </summary>
        public bool IsClose
        {
            get { return isClose; }
            set { isClose = value; }
        }

        public ConfirmInputPlanCodeView(string weight, string unit, string filepath, bool isStandardBoard, string carNO)
        {
            InitializeComponent();
            this.weightLabel.Content = weight;
            this.unitLabel.Content = unit;
            this.carLabel.Content = carNO;
            businessNoTextBlock.Focus();
            this.Loaded +=Window_Loaded;
            //if (isStandardBoard)
            //{
            //    this.businessNoTextBlock.Focus();
            //    this.KeyDown += Window_KeyDown;
            //}
            //else
            //{
            //    ikb = new KeyBoardController(filepath);
            //    ikb.OnReceivedKeyData += ikb_OnReceivedKeyData;
            //    ikb.Open();
            //    ikb.Start();
            //}
            this.businessNoTextBlock.Focus();
            this.businessNoTextBlock.KeyDown += Window_KeyDown;
            this.businessNoTextBlock.KeyUp += businessNoTextBlock_KeyUp;
            this.Closed += ConfirmInputPlanCodeView_Closed;
            ikb = new KeyBoardController(filepath);
            ikb.OnReceivedKeyData += ikb_OnReceivedKeyData;
            ikb.Open();
            ikb.Start();
        }
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowInTaskbar = false;
        }
        void ConfirmInputPlanCodeView_Closed(object sender, EventArgs e)
        {
            if (FormCloseEvent != null)
            {
                FormCloseEvent();
            }
        }

        /// <summary>
        /// 非标准键盘响应事件对应的方法
        /// </summary>
        /// <param name="pType">数据类型</param>
        /// <param name="pData">数据(数据类型为命令时，pData为空。)</param>
        /// <param name="pCommand">键盘命令</param>
        void ikb_OnReceivedKeyData(KeyDataType pType, string pData, KeyCommand pCommand)
        {
            if (pType == KeyDataType.COMMAND)//表示命令
            {
                bool isCloseForm = false;
                if (pCommand == KeyCommand.OK)//按了确定键
                {
                    isCloseForm = true;
                    IsOk = true;
                }
                else if (pCommand == KeyCommand.CANCEL)//按了取消键
                {
                    isCloseForm = true;
                    IsOk = false;
                }
                else if (pCommand == KeyCommand.CLEAR)//按了清空键
                {
                    this.businessNoTextBlock.Text = string.Empty;
                }
                else if (pCommand == KeyCommand.DELETE)//按了删除键
                {
                    if (this.businessNoTextBlock.Text.Trim().Length >= 2)
                    {
                        string curBusinessNo = this.businessNoTextBlock.Text.Trim();
                        this.businessNoTextBlock.Text = curBusinessNo.Substring(0, curBusinessNo.Length - 1);
                    }
                    else
                    {
                        this.businessNoTextBlock.Text = string.Empty;
                    }
                }
                else if (pCommand == KeyCommand.HELP)//按了求助键
                {
                    isCloseForm = true;
                    IsHelp = true;
                }
                if (isCloseForm)
                {
                    ikb.Stop();
                    ikb.Close();
                    this.BussinessNo = this.businessNoTextBlock.Text.Trim();
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Close();
                    }));
                }
            }
            else if (pType == KeyDataType.DATA)//按了键盘中的数字键
            {
                this.businessNoTextBlock.Text = this.businessNoTextBlock.Text.Trim() + pData;
            }
        }

        void businessNoTextBlock_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.IsUp)
            {
                int keyData = Convert.ToInt32(Enum.Parse(typeof(Key), e.Key.ToString()));
                var keyCode = ikb.GetKeyCommand(keyData);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "称点主窗体_业务号TextBlock的KeyUp事件",
                    Msg = "e.key.tostring():" + e.Key.ToString() + ";keyCode:" + keyCode,
                    Origin = "汽车衡_" +ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                bool isCloseForm = false;
                if (keyCode == Keyboard.Interface.KeyCommand.OK)//按下确定键
                {
                    if (string.IsNullOrEmpty(this.businessNoTextBlock.Text.Trim()))
                    {
                        errInfoLabel.Content = "业务号不能为空!";
                    }
                    else
                    {
                        errInfoLabel.Content = "";
                        isCloseForm = true;
                        IsOk = true;
                    }
                }
                else if (keyCode == Keyboard.Interface.KeyCommand.CANCEL)//按下取消键
                {
                    isCloseForm = true;
                    IsClose = true;
                }
                else if (keyCode == Keyboard.Interface.KeyCommand.DELETE)//按下删除键
                {
                    this.businessNoTextBlock.Text = this.businessNoTextBlock.Text.Trim();
                }
                else if (keyCode == Keyboard.Interface.KeyCommand.CLEAR)//按下清空键
                {
                    isCloseForm = true;
                    IsClear = true;
                }
                else if (keyCode == Keyboard.Interface.KeyCommand.HELP)//按下求助键
                {
                    isCloseForm = true;
                    IsHelp = true;
                }
                if (isCloseForm)
                {
                    if (ikb != null)
                    {
                        ikb.Stop();
                        ikb.Close();
                    }
                    this.BussinessNo = this.businessNoTextBlock.Text.Trim();
                    this.Close();
                }
            }
        }

        /// <summary>
        /// 窗体按键事件
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.IsDown)
            //{
            //    int keyData = Convert.ToInt32(Enum.Parse(typeof(Key), e.Key.ToString()));
            //    var keyCode = ikb.GetKeyCommand(keyData);
            //    #region 日志
            //    LogModel log = new LogModel()
            //    {
            //        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //        FunctionName = "称点主窗体",
            //        Msg = "e.key.tostring():" + e.Key.ToString() + ";keyCode:" + keyCode,
            //        Origin = "汽车衡_" +ClientInfo.Name
            //    };
            //    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            //    #endregion
            //    bool isCloseForm = false;
            //    if (keyCode == Keyboard.Interface.KeyCommand.OK)//按下确定键
            //    {
            //        if (string.IsNullOrEmpty(this.businessNoTextBlock.Text.Trim()))
            //        {
            //            MessageBox.Show("业务号不能为空!");
            //        }
            //        else
            //        {
            //            isCloseForm = true;
            //            IsOk = true;
            //        }
            //    }
            //    else if (keyCode == Keyboard.Interface.KeyCommand.CANCEL)//按下取消键
            //    {
            //        isCloseForm = true;
            //        IsOk = false;
            //    }
            //    else if (keyCode == Keyboard.Interface.KeyCommand.DELETE)//按下删除键
            //    {
            //        if (this.businessNoTextBlock.Text.Trim().Length >= 2)
            //        {
            //            string curBusinessNo = this.businessNoTextBlock.Text.Trim();
            //            this.BussinessNo = curBusinessNo.Substring(0, curBusinessNo.Length - 1);
            //            this.businessNoTextBlock.Text = this.BussinessNo;
            //        }
            //        else
            //        {
            //            this.BussinessNo = string.Empty;
            //            this.businessNoTextBlock.Text = this.BussinessNo;
            //        }
            //    }
            //    else if (keyCode == Keyboard.Interface.KeyCommand.CLEAR)//按下清空键
            //    {
            //        this.BussinessNo = string.Empty;
            //        this.businessNoTextBlock.Text = this.BussinessNo;
            //    }
            //    else if (keyCode == Keyboard.Interface.KeyCommand.HELP)//按下求助键
            //    {
            //        isCloseForm = true;
            //        IsHelp = true;
            //    }
            //    else
            //    {
            //        //this.BussinessNo = this.BussinessNo + e.Key.ToString(); //KeyInterop.VirtualKeyFromKey();
            //        //this.businessNoTextBlock.Text = this.BussinessNo;
            //        //this.businessNoTextBlock.Text = this.businessNoTextBlock.Text.Trim() + e.Key;
            //    }
            //    if (isCloseForm)
            //    {
            //        if (ikb!=null)
            //        {
            //            ikb.Stop();
            //            ikb.Close();
            //        }
            //        this.BussinessNo = this.businessNoTextBlock.Text.Trim();
            //        this.Close();
            //    }
            //}
        }

        private void businessNoTextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.businessNoTextBlock.Text))
            {
                this.errInfoLabel.Content = "";
            }
        }
    }
}
