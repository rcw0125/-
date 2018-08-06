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
using Talent.Measure.DomainModel;
using Talent_LT.HelpClass;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// ConfirmMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmMessageBox : Window
    {
        /// <summary>
        /// 是否允许关闭窗体
        /// </summary>
        private bool isAllowClose = false;
        /// <summary>
        /// 是否系统强制关闭
        /// </summary>
        public bool isSystermClose = false;
        /// <summary>
        /// 是否用户关闭
        /// </summary>
        public bool isUserClose = false;
        private bool isOk;
        /// <summary>
        /// 是否确定
        /// </summary>
        public bool IsOk
        {
            get { return isOk; }
            set { isOk = value; }
        }

        private int count;
        /// <summary>
        /// 倒计时(秒)
        /// </summary>
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        private string okContent;
        private bool isTimer;
        /// <summary>
        /// 是否启用timer倒计时
        /// </summary>
        public bool IsTimer
        {
            get { return isTimer; }
            set { isTimer = value; }
        }

        private List<BullInfo> showList;
        /// <summary>
        /// 展示的列表
        /// </summary>
        public List<BullInfo> ShowList
        {
            get { return showList; }
            set
            {
                showList = value;
                if (showList.Count > 0)
                {
                    this.showGridButton.Visibility = Visibility.Visible;
                    //默认保存一遍……
                    ShowLikeWeightView show = new ShowLikeWeightView(ShowList, cMatchid, cMeasureType);
                    show.SaveInfos();
                    show.Close();
                }
            }
        }

        private bool isShowClose = false;
        /// <summary>
        /// 是否显示关闭按钮
        /// </summary>
        public bool IsShowClose
        {
            get { return isShowClose; }
            set
            {
                isShowClose = value;
                if (isShowClose)
                {
                    this.btnClose.Visibility = Visibility.Visible;
                }
                else
                {
                    this.btnClose.Visibility = Visibility.Hidden;
                }
            }
        }
        /// <summary>
        /// 是否使用已经超期的皮重 2016-3-17 10:54:11……
        /// </summary>
        public bool isUseTimeOutTare = false;
        /// <summary>
        /// 当前MATCHID…… 相似重量使用2016-3-15 10:24:46
        /// </summary>
        public string cMatchid = string.Empty;//
        /// <summary>
        /// 当前计量方式……相似重量使用
        /// </summary>
        public string cMeasureType = string.Empty;//
        /// <summary>
        /// 确认倒计时
        /// </summary>
        private System.Windows.Forms.Timer confirmTimer;
        /// <summary>
        /// 背景色
        /// </summary>
        Color color = (Color)ColorConverter.ConvertFromString("#F1D609");
        WindowsFormHelpClass formHClass = new WindowsFormHelpClass();//窗体帮助类
        /// <summary>
        /// 初始化消息框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">信息内容</param>
        /// <param name="showok">是否显示确定按钮</param>
        /// <param name="showcancel">是否显示取消按钮</param>
        /// <param name="okContent">确定按钮显示内容</param>
        /// <param name="cancelContent">取消按钮显示内容</param>
        /// <param name="isTimer">是否启动倒计时(缺省为不启动)</param>
        /// <param name="count">倒计时秒数(缺省为30秒)</param>
        public ConfirmMessageBox(string title, string msg, bool showok, bool showcancel, string okContent, string cancelContent, bool isTimer = false, int count = 30)
        {
            InitializeComponent();
            this.IsTimer = isTimer;
            this.Count = count;
            this.okContent = okContent;
            this.Title = title;
            //this.msgLable.Content = msg;
            this.msgLable.Text = msg;//修改为 textbox
            ///是不是显示皮重超期……2016-3-17 10:51:13
            if (msg.Contains("皮重已超期"))
            {
                this.TareTimeOutCheckBox.Visibility = Visibility.Visible;
            }
            this.Loaded += ConfirmMessageBox_Loaded;
            SetBackColor(msg);
            this.okButton.Content = okContent;
            this.cancelButton.Content = cancelContent;
            this.okButton.Visibility = showok ? Visibility.Visible : Visibility.Hidden;
            this.cancelButton.Visibility = showcancel ? Visibility.Visible : Visibility.Hidden;
            if (isTimer)
            {
                this.okButton.Content = okContent + "(" + count.ToString() + ")";
                #region 注册记时时间
                confirmTimer = new System.Windows.Forms.Timer();
                confirmTimer.Interval = 1000;
                confirmTimer.Tick += confirmTimer_Tick;
                confirmTimer.Start();
                #endregion
            }
        }

        /// <summary>
        /// 确认倒计时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void confirmTimer_Tick(object sender, EventArgs e)
        {
            if (count > 1)
            {
                count--;
                this.okButton.Content = this.okContent + "(" + count.ToString() + ")";
            }
            else
            {
                this.okButton.Content = this.okContent;
                IsOk = true;
                isAllowClose = true;
                isUseTimeOutTare = Convert.ToBoolean(this.TareTimeOutCheckBox.IsChecked);//设定是否选中使用 超期的皮重
                confirmTimer.Stop();
                confirmTimer.Dispose();
                this.Close();
            }
        }

        /// <summary>
        /// 确定按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            isAllowClose = true;
            isUseTimeOutTare = Convert.ToBoolean(this.TareTimeOutCheckBox.IsChecked);//设定是否选中使用 超期的皮重
            if (confirmTimer != null)
            {
                confirmTimer.Stop();
                confirmTimer.Dispose();
            }
            this.Close();
        }

        /// <summary>
        /// 取消按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            isAllowClose = true;
            isUseTimeOutTare = Convert.ToBoolean(this.TareTimeOutCheckBox.IsChecked);//设定是否选中使用 超期的皮重
            if (confirmTimer != null)
            {
                confirmTimer.Stop();
                confirmTimer.Dispose();
            }
            this.Close();
        }

        /// <summary>
        /// 弹框显示明细信息 lt 2016-2-18 11:46:19……
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showGridButton_Click(object sender, RoutedEventArgs e)
        {
            bool isOpen = formHClass.SetForeWindow("智能化远程集中计量管理系统(相似重量)");
            if (!isOpen)
            {
                ShowLikeWeightView show = new ShowLikeWeightView(ShowList, cMatchid, cMeasureType);
                show.Show();
            }

        }
        ///// <summary>
        ///// 关闭窗体
        ///// </summary>
        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    IsOk = false;
        //    this.Close();
        //}
        /// <summary>
        /// 去掉关闭窗口 2016-2-16 16:44:02……
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (isAllowClose)
            {
                return;
            }
            e.Cancel = true;
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        public void CloseForm()
        {
            isAllowClose = true;
            isSystermClose = true;
            if (confirmTimer != null)
            {
                confirmTimer.Stop();
                confirmTimer.Dispose();
            }
            this.Close();
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnClose_Click(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            isAllowClose = true;
            isUserClose = true;
            isSystermClose = false;
            if (confirmTimer != null)
            {
                confirmTimer.Stop();
                confirmTimer.Dispose();
            }
            this.Close();
        }
        // 支持标题栏拖动  
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            bool isFirstChangeWindowsState = false;
            base.OnMouseLeftButtonDown(e);
            // 获取鼠标相对标题栏位置  
            Point position = e.GetPosition(gridTitleBar);
            // 如果鼠标位置在标题栏内，允许拖动  
            if (position.X >= 0 && position.X < gridTitleBar.ActualWidth && position.Y >= 0 && position.Y < gridTitleBar.ActualHeight)
            {
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1)
                {
                    this.DragMove();
                }
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                {
                    if (this.WindowState == WindowState.Normal)
                    {
                        isFirstChangeWindowsState = true;
                        this.WindowState = WindowState.Maximized;
                    }
                    if (isFirstChangeWindowsState == false)
                    {
                        if (this.WindowState == WindowState.Maximized)
                        {
                            this.WindowState = WindowState.Normal;
                        }
                    }

                }

            }

        }
        /// <summary>
        /// 设置超差背景颜色
        /// </summary>
        /// <param name="msg"></param>
        private void SetBackColor(string msg)
        {
            if (msg.Contains("净重超出浮动范围，已超:"))
            {
                msg = msg.Substring(msg.IndexOf("净重超出浮动范围，已超:")).Replace("净重超出浮动范围，已超:", "");
                msg = msg.Substring(0, msg.IndexOf("/"));
                try
                {
                    decimal sD = Convert.ToDecimal(msg) * 1000;
                    if (sD >= 600 || sD <= -600)
                    {
                        this.Background = new SolidColorBrush(color);
                    }
                }
                catch
                {

                }
            }
            if (msg.Contains("毛重超出浮动范围，已超:"))
            {
                msg = msg.Substring(msg.IndexOf("毛重超出浮动范围，已超:")).Replace("毛重超出浮动范围，已超:", "");
                msg = msg.Substring(0, msg.IndexOf("/"));
                try
                {
                    decimal sD = Convert.ToDecimal(msg) * 1000;
                    if (sD >= 600 || sD <= -600)
                    {
                        this.Background = new SolidColorBrush(color);
                    }
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// 窗体加载
        /// </summary>
        private void ConfirmMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLastLocation();
        }
        /// <summary>
        ///加载默认值
        /// </summary>
        private void LoadLastLocation()
        {
            try
            {
                this.Left = (Convert.ToDouble(Properties.Settings.Default.HandleTaskViewLeft)) + (this.Width);
                this.Top = (Convert.ToDouble(Properties.Settings.Default.HandleTaskViewTop)) + (this.Height);
            }
            catch
            {
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                // 支持标题栏拖动  
                this.DragMove();
            }
        }
    }
}
