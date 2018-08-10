using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Talent.CarMeasureClient.ConfirmView;
using Talent.CarMeasureClient.MyUC;
using Talent.CarMeasureClient.ViewModel;
using Talent.ClientCommMethod;
using Talent.ClientCommonLib;
using Talent.Io.Controller;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.CommonMethod;
using Talent.CarMeasureClient.Common;
using Talent.Weight.Interface;
using System.Runtime.InteropServices;
using Talent_LT.HelpClass;
using Talent.Measure.WPF.Log;
namespace Talent.CarMeasureClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Only_WindowBase
    {
        /// <summary>
        /// 倒计时动画对象(王磊新加)
        /// </summary>
        System.Windows.Media.Animation.Storyboard std = null;
        /// <summary>
        /// 是否允许启动文件同步
        /// </summary>
        bool _enableFileSync = true;
        Process proSync;
        // DispatcherTimer timerWeightTextBorder;

        private DispatcherTimer timer;

        /// <summary>
        /// 倒数数字
        /// </summary>
        private int count;
        /// <summary>
        /// 初始化IO控制
        /// </summary>
        IoController io = null;
        /// <summary>
        /// 提示信息滚动字幕动画
        /// </summary>
        Storyboard sbLargeMsg = null;

        /// <summary>
        /// 通知提示动画
        /// </summary>
        Storyboard sbNoticeMsg = null;
        //string txtMsg1Str = "";
        //string txtMsg2Str = "";
        /// <summary>
        /// 手动停止计量
        /// </summary>
        private bool stopCountdown;
        /// <summary>
        /// 窗体帮助类
        /// </summary>
        WindowsFormHelpClass formHClass = new WindowsFormHelpClass();//
        /// <summary>
        /// 是否测试
        /// </summary>
        bool isTest = true;
        /// <summary>
        /// 日志记录
        /// </summary>
        LogsHelpClass logH = new LogsHelpClass();
        /// <summary>
        /// 计算机信息帮助类
        /// </summary>
        ComputerInfoHelpClass cpH = new ComputerInfoHelpClass();
        DateTime firstStartTime = Convert.ToDateTime("1799-01-01 00:00:00");
        int runTimeCount = 0;
        /// <summary>
        /// 构造函数，1.窗体加载 2.获取倒计时动画3.调用vm里的方法
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                this.Focus();
                this.Loaded += MainWindow_Loaded;
                sbLargeMsg = new Storyboard();
                sbNoticeMsg = new Storyboard();
                stopCountdown = false;
                runTimeCount = 0;
                //获取倒计时动画对象
                std = (System.Windows.Media.Animation.Storyboard)this.Resources["SBCountdown2"];
                //动画结束在这里调用方法
                std.Completed += (t, r) => this.MyMethod();
                Messenger.Default.Register<object>(this, "ClearAll", ClearAll);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<string>("系统初始化信息失败！原因：" + ex.Message, "MainWindow");
            }
        }

        private void MyMethod()
        {
            CarMeasureClientViewModel viewModel = this.DataContext as CarMeasureClientViewModel;
            viewModel.AfterAnimation();

        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShowInTaskbar = false;
            #region 日志
            LogModel log1 = new LogModel()
            {
                Origin = "汽车衡_" + ClientInfo.Name,
                FunctionName = "称点主窗体_MainWindow_Loaded",
                Level = LogConstParam.LogLevel_Info,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                Msg = "系统启动成功"
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
            #endregion
            //SoundReading sr = new SoundReading("测试12121212");
            //sr.Voice();
            //检查更新……
            CheckUpdate();
            #region 是否显示测试内容
            isTest = ConfigurationManager.AppSettings["IsTest"].ToString().Trim() == "1" ? true : false;
            if (isTest)
            {
                canvas1.Visibility = System.Windows.Visibility.Visible;
                testCardNoBtn.Visibility = System.Windows.Visibility.Visible;
                testCardNoTxt.Visibility = System.Windows.Visibility.Visible;
                TestSendTaskBtn.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                canvas1.Visibility = System.Windows.Visibility.Hidden;
                testCardNoBtn.Visibility = System.Windows.Visibility.Hidden;
                testCardNoTxt.Visibility = System.Windows.Visibility.Hidden;
                TestSendTaskBtn.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion
            //版本
            txtVer.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 3);
            //展示系统当前时间
            firstStartTime = DateTime.Now;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            // ErrorAutoStart();
            CarMeasureClientViewModel viewModel = this.DataContext as CarMeasureClientViewModel;
            viewModel.std = std;
            viewModel.gridReader = rendergv;
            viewModel.gridSupplier = gridSupplier;
            viewModel.gridMeasure = gridMeasure;
            viewModel.gridMeasureWeight = gridMeasureWeight;
            viewModel.isTest = isTest;//是否测试模式
            viewModel.window = this;
            TestWeightviewModel = viewModel;
            this.KeyDown += Window_KeyDown;
            //启动文件同步程序
            if (_enableFileSync == true)
            {
                string processName = "Talent.FIleSync";
                Process[] procList = Process.GetProcessesByName(processName);
                if (procList != null && procList.Length > 0)
                {
                    proSync = procList[0];
                }
                else
                {
                    proSync = new Process();
                    string syncExePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, processName + ".exe");
                    proSync.StartInfo.FileName = syncExePath;
                    proSync.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    try
                    {
                        proSync.Start();
                        LogModel log = new LogModel()
                        {
                            Origin = "汽车衡_" + ClientInfo.Name,
                            FunctionName = "称点主窗体_MainWindow_Loaded",
                            Level = LogConstParam.LogLevel_Info,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            Msg = "启动文件同步程序成功。"
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    }
                    catch (Win32Exception err)
                    {
                        #region log
                        LogModel log = new LogModel()
                        {
                            Origin = "汽车衡_" + ClientInfo.Name,
                            FunctionName = "称点主窗体_MainWindow_Loaded",
                            Level = LogConstParam.LogLevel_Error,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            Msg = "启动文件同步程序失败。原因：" + err.Message
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                }
            }
            FileHelpClass fHClass = new FileHelpClass();
            fHClass.FileLogDelete(15, false);

            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            //this.Topmost = true;
            this.Activate();
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            #region log
            LogModel log11 = new LogModel()
            {
                Origin = "汽车衡_" + ClientInfo.Name,
                FunctionName = "称点主窗体_MainWindow",
                Level = LogConstParam.LogLevel_Info,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                Msg = "程序运行超过设定时间" + viewModel.AutoRunTime.ToString()
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log11));
            #endregion
        }


      
        private void timer_Tick(object sender, EventArgs e)
        {
            txtCurrentTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];
            txtCurrentTime.Text += " " + week;
            //bool isPing=cpH.CheckPingIsSuccess("10.1.196.86");
            //if(!isPing)
            //{
            //    FileHelpClass.WriteSysLog("ping 10.1.196.86 返回：" + isPing);
            //}  
            CarMeasureClientViewModel viewModel = this.DataContext as CarMeasureClientViewModel;
            runTimeCount += 1;
            if ((viewModel.BullState == eBullTag.free || viewModel.BullState == eBullTag.init) && runTimeCount > 60 * viewModel.AutoRunTime)
            {
                int setTimerCount = 60 * viewModel.AutoRunTime;
                #region log
                LogModel log = new LogModel()
                {
                    Origin = "汽车衡_" + ClientInfo.Name,
                    FunctionName = "称点主窗体_MainWindow",
                    Level = LogConstParam.LogLevel_Info,                
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    Msg = "程序运行超过设定时间" + setTimerCount.ToString() + "，自动重启."
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //CloseInfos();
                viewModel.ClosedHardWare();
                System.Windows.Forms.Application.Restart(); //程序运行超过设定时间，自动重启
                Application.Current.Shutdown(0);
            }

            #region 写日志
            Talent.ClinetLog.TimeLog.Log(txtCurrentTime.Text + "  BullState:" + viewModel.BullState);
            #endregion

            FileHelpClass.WriteTalent(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //CheckIsGetWeight();

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确定要退出系统吗？", "退出", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                #region log
                LogModel log = new LogModel()
                {
                    Origin = "汽车衡_" + ClientInfo.Name,
                    FunctionName = "称点主窗体_MainWindow",
                    Level = LogConstParam.LogLevel_Error,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    Msg = "秤点主窗体关闭按钮被点击，退出系统。"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                SocketCls.Emit(ClientSendCmdEnum.logout, "");//退出
                if (SocketCls.s != null)
                {
                    SocketCls.s.Dispose();
                }
                Application.Current.Shutdown();
            }
        }

        private void txtMsg2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Thread t1 = new Thread(new ThreadStart(LargeMsg));
            t1.Start();
        }
        #region 测试

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            CarMeasureClientViewModel viewModel = this.DataContext as CarMeasureClientViewModel;
            viewModel.TestOpenInputPlanCodeView();


            //// System.Windows.Forms.Application.Restart();
            ////
            //CarMeasureClientViewModel viewModel = this.DataContext as CarMeasureClientViewModel;
            ////this.userNotic.Content = viewModel.TestCardNo;//测试滚动显示文字
            ////this.userNotic.DataContext = viewModel.TestCardNo;
            //viewModel.iic_OnReadCardNo("COM1", viewModel.TestCardNo);
            ////viewModel.getBllRulesServiceInfo("鲁GM8925", "", "", 0, "", ClientInfo.ClientId, 1);
            ////viewModel.iic_OnReadCardNo("COM1", "");
            ////viewModel.iwc.IsFinish = true;
            ////viewModel.BullState = eBullTag.end;
            ////List<ReaderInfo> rlist = new List<ReaderInfo>();

            ////for (int i = 0; i < 10; i++)
            ////{
            ////    ReaderInfo info0 = new ReaderInfo();
            ////    info0.no = i;
            ////    info0.bindStr = "carno";
            ////    info0.lblText = "车   号";
            ////    rlist.Add(info0);
            ////}
            ////rendergv.Children.Clear();
            ////for (int i = rendergv.RowDefinitions.Count - 1; i > -1; i--)
            ////{
            ////    rendergv.RowDefinitions.RemoveAt(i);
            ////}

            ////RenderMainUI ui = new RenderMainUI(500, rendergv, viewModel.CarTaskModel.BullInfo, rlist);
            ////ui.SetRenderMainUI();

        }
        /// <summary>
        /// 手动发送计量任务(测试使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CarMeasureClientViewModel viewModel = this.DataContext as CarMeasureClientViewModel;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(10000);
                viewModel.Weight = 0;
            });
            InputBusinessInfos bInfosView = new InputBusinessInfos();
            bInfosView.Owner = this;
            bInfosView.ShowDialog();

            if (!string.IsNullOrEmpty(bInfosView.BusinessInfoJson))
            {
                viewModel.ReceiveIcNo(bInfosView.BusinessInfoJson);
            }
            //MeasureServiceModel msm = InfoExchange.DeConvert(typeof(MeasureServiceModel), bInfosView.BusinessInfoJson) as MeasureServiceModel;
            //TaskModel carTaskModel = new TaskModel
            //{
            //    CreateTime = DateTime.Now,
            //    ClientId = ClientInfo.ClientId,
            //    ClientName = ClientInfo.Name,
            //    IsBusinessInfoQuery = false,
            //    CarNumber = "鲁A00001",
            //    MeasureType = "现场自助",
            //    ErrorMsg = "业务不完整",
            //    Weight = 120.38M,
            //    ServiceModel = msm
            //};
            //string paraJsonStr = JsonConvert.SerializeObject(carTaskModel);// Talent.CommonMethod.InfoExchange.ToJson(carTaskModel);
            //SocketCls.Emit(ClientSendCmdEnum.measureData, paraJsonStr);
        }
        #endregion


        private void gridCountdown_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// 窗体按键事件
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsDown)
            {
                this.WindowState = WindowState.Maximized;
                CarMeasureClientViewModel viewModel = this.DataContext as CarMeasureClientViewModel;
                if (viewModel.MeasureTypeInfo == eMeasureType.远程计量.ToString())//如果是远程计量， 键盘按键 不起作用……2016-3-31 15:38:26……
                {
                    return;
                }
                int keyData = Convert.ToInt32(Enum.Parse(typeof(Key), e.Key.ToString()));
                if (viewModel.ikb != null)
                {
                    var keyCode = viewModel.ikb.GetKeyCommand(keyData);
                    if (keyCode == Keyboard.Interface.KeyCommand.OK)
                    {
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Msg = "获取当前系统的BullState：" + viewModel.BullState + "是否摁过：" + viewModel.isEnterOK + "是否允许恩下：" + viewModel.isAllowEnterOk,
                            FunctionName = "称点主窗体_Window_KeyDown",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Level = LogConstParam.LogLevel_Info
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        if (viewModel.BullState == eBullTag.metering || viewModel.BullState == eBullTag.specification)
                        {
                            if (!viewModel.isAllowEnterOk)//当系统提示允许恩确认键时
                            {
                                return;
                            }
                            if (viewModel.isEnterOK)//恩过 确认键之后
                            {
                                return;
                            }
                            if (viewModel.isEnterOnly)//摁过求助键  不允许再摁回撤键
                            {
                                return;
                            }
                            viewModel.isEnterOnly = true;
                            #region 写日志
                            LogModel kLog = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Msg = "车号:" + viewModel.CarNumber + "按下确认键准备保存重量",
                                FunctionName = "称点主窗体_Window_KeyDown",
                                Origin = "汽车衡_" + ClientInfo.Name,
                                Level = LogConstParam.LogLevel_Info
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(kLog));
                            #endregion
                            Thread t1 = new Thread(new ThreadStart(viewModel.saveWeightServiceInfo));
                            t1.Start();

                        }
                    }
                    else if (keyCode == Keyboard.Interface.KeyCommand.CANCEL)
                    {
                        if (viewModel.isEnterCancel)
                        {
                            return;
                        }
                        //if (!string.IsNullOrEmpty(viewModel.measureServiceResult) && viewModel.measureServiceResult != null && !string.IsNullOrEmpty(viewModel.CarNumber))
                        //{
                        //    std.Stop();
                        //    Thread t1 = new Thread(new ThreadStart(viewModel.tipMsg));
                        //    t1.Start();
                        //}

                    }
                    else if (keyCode == Keyboard.Interface.KeyCommand.HELP)//远程求助……2016-2-29 13:03:53……
                    {
                        if (viewModel.isEnterHelp)
                        {
                            return;
                        }
                        if (!string.IsNullOrEmpty(viewModel.measureServiceResult) && viewModel.measureServiceResult != null && !string.IsNullOrEmpty(viewModel.CarNumber))
                        {
                            if (viewModel.isEnterOnly)//摁过回车键  不允许再摁求助键 
                            {
                                return;
                            }
                            viewModel.isEnterOnly = true;
                            #region 写日志
                            LogModel log = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Msg = "车号：" + viewModel.CarNumber + "按下求助键远程求助",
                                FunctionName = "称点主窗体_Window_KeyDown",
                                Origin = "汽车衡_" + ClientInfo.Name,
                                Level = LogConstParam.LogLevel_Info
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            #endregion
                            viewModel.SendTaskToServer("司机按键远程求助", viewModel.measureServiceResult, true, true);
                        }
                    }
                }
            }
        }
        private void LargeMsg()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                sbLargeMsg.Stop();
                sbLargeMsg.Children.Clear();
                //消息的长度
                double msgWidth = MeasureTextWidth(txtMsg2.Text, txtMsg2.FontSize, txtMsg2.FontFamily.ToString());
                //滚动的时间
                double runTime = 10 * (this.Width > msgWidth ? this.Width : msgWidth) / this.Width;
                txtMsg2.Margin = new Thickness(txtMsg2.Margin.Left, txtMsg2.Margin.Top, txtMsg2.Margin.Right > msgWidth ? txtMsg2.Margin.Right : -msgWidth, txtMsg2.Margin.Bottom);
                DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
                sbLargeMsg.Children.Add(doubleAnimationUsingKeyFrames);
                Storyboard.SetTarget(doubleAnimationUsingKeyFrames, txtMsg2);
                Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));
                EasingDoubleKeyFrame keyFrame1 = new EasingDoubleKeyFrame((msgWidth > this.Width) ? msgWidth : this.Width + 130, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
                EasingDoubleKeyFrame keyFrame2 = new EasingDoubleKeyFrame(-(this.txtMsg2.ActualWidth), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(runTime)));
                doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame1);
                doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame2);
                sbLargeMsg.RepeatBehavior = RepeatBehavior.Forever;
                sbLargeMsg.Begin();
            }));
        }
        private void LargeMsg3()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (txtMsg3.Text.Length > 0)
                {
                    cancasNoticeMsg.Visibility = System.Windows.Visibility.Visible;
                    sbNoticeMsg.Stop();
                    sbNoticeMsg.Children.Clear();
                    //消息的长度
                    double msgWidth = MeasureTextWidth(txtMsg3.Text, txtMsg3.FontSize, txtMsg3.FontFamily.ToString());
                    //滚动的时间
                    double runTime = 11 * (880 > msgWidth ? 880 : msgWidth) / 880;
                    txtMsg3.Margin = new Thickness(txtMsg3.Margin.Left, txtMsg3.Margin.Top, txtMsg3.Margin.Right > msgWidth ? txtMsg3.Margin.Right : -msgWidth, txtMsg3.Margin.Bottom);
                    DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
                    sbNoticeMsg.Children.Add(doubleAnimationUsingKeyFrames);
                    Storyboard.SetTarget(doubleAnimationUsingKeyFrames, txtMsg3);
                    Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));
                    EasingDoubleKeyFrame keyFrame1 = new EasingDoubleKeyFrame(880, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
                    EasingDoubleKeyFrame keyFrame2 = new EasingDoubleKeyFrame(-(this.txtMsg3.ActualWidth), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(runTime)));
                    doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame1);
                    doubleAnimationUsingKeyFrames.KeyFrames.Add(keyFrame2);
                    sbNoticeMsg.RepeatBehavior = RepeatBehavior.Forever;
                    sbNoticeMsg.Begin();
                }
                else
                {
                    cancasNoticeMsg.Visibility = System.Windows.Visibility.Hidden;
                }
            }));
        }

        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontFamily"></param>
        /// <returns></returns>
        private double MeasureTextWidth(string text, double fontSize, string fontFamily)
        {
            FormattedText formattedText = new FormattedText(
            text,
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface(fontFamily.ToString()),
            fontSize,
            Brushes.Black
            );
            return formattedText.WidthIncludingTrailingWhitespace;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            //CloseInfos();
            base.OnClosing(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            #region 日志
            LogModel log1 = new LogModel()
            {
                Origin = "汽车衡_" + ClientInfo.Name,
                FunctionName = "称点主窗体_OnClosed",
                Level = LogConstParam.LogLevel_Info,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                Msg = "计量终端已关闭"
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
            #endregion
            CloseInfos();
            base.OnClosed(e);
            // ClearPross("Talent.");//程序关闭,清除所有有关进程
        }

        /// <summary>
        /// 车号显示值改变事件(临时不可见的textBox，用以清楚窗体中的"重量"信息)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CarNoTempTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.CarNoTempTextBox.Text.Trim()))
            {
                CarMeasureClientViewModel tvm = (CarMeasureClientViewModel)this.DataContext;
                tvm.CarTaskModel = null;
            }
        }

        #region 拖动 测试重量
        //todo:lt 2016-1-27 11:10:57……拖动获取重量
        CarMeasureClientViewModel TestWeightviewModel;
        private void DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double left = Canvas.GetLeft(thumb1) + e.HorizontalChange;
            if (left > 250)
            {
                left = 250;
            }
            if (left < 0)
            {
                left = 0;
            }
            Canvas.SetLeft(thumb1, left);
            if (TestWeightviewModel.iwc != null)
            {
                TestWeightviewModel.iwc.reciveWeightData(Convert.ToInt16(left) * 600);
            }
        }

        private void DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            thumb1.Background = Brushes.White;
        }

        private void DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            thumb1.Background = Brushes.Red;
        }

        #endregion 拖动

        private void txtMsg3_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }


        private void txtMsg3_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Thread t1 = new Thread(new ThreadStart(LargeMsg3));
            t1.Start();
        }
        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMinForm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.txtGrossWeight.Focus();
                this.mainWindow.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                #region log
                LogModel log = new LogModel()
                {
                    Origin = "汽车衡_" + ClientInfo.Name,
                    FunctionName = "称点主窗体_窗体最小化",
                    Level = LogConstParam.LogLevel_Error,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    Msg = "窗体最小化时异常,原因:" + ex.Message
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }

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
                    if (mainWindow.WindowState == WindowState.Normal)
                    {
                        isFirstChangeWindowsState = true;
                        mainWindow.WindowState = WindowState.Maximized;
                    }
                    if (isFirstChangeWindowsState == false)
                    {
                        if (mainWindow.WindowState == WindowState.Maximized)
                        {
                            mainWindow.WindowState = WindowState.Normal;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 检查更新
        /// </summary>
        private void CheckUpdate()
        {
            string ftp = ConfigurationManager.AppSettings["FtpAddress"].ToString();
            string ftpU = ConfigurationManager.AppSettings["FtpUserName"].ToString();
            string ftpP = ConfigurationManager.AppSettings["FtpPassWord"].ToString();
            //IAutoUpdater autoUpdater = new AutoUpdater();
            //autoUpdater.UpdateFtp(ftp, ftpU, ftpP);
            string param = ftp + "~" + ftpU + "~" + ftpP + "~" + "Talent.CarMeasureClient.exe";
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
        /// 清理
        /// </summary>
        /// <param name="fileInfos"></param>
        private void ClearPross(string fileInfos)
        {
            try
            {
                ProcessHelpClass.ClearProcessContainsName(fileInfos);
            }
            catch { }
        }
        /// <summary>
        /// 右键双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowClientName_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //CarMeasureClientViewModel tvm = (CarMeasureClientViewModel)this.DataContext;
                //tvm.initRfidController(System.AppDomain.CurrentDomain.BaseDirectory + @"/ClientConfig/SystemConfig.xml");
                bool isOpen = formHClass.SetForeWindow("智能化远程集中计量管理系统(配置管理)");
                if (!isOpen)
                {
                    string processName = "Talent.CarMeasureConfig";
                    string configExePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, processName + ".exe");
                    ProcessHelpClass.ProcessOpenExe(configExePath);
                }
            }
        }

        private void ClearAll(object data)
        {
            CloseInfos();
        }
        /// <summary>
        /// 关闭之前必须要关闭的活
        /// </summary>
        private void CloseInfos()
        {
            try
            {
                if (_enableFileSync == true)
                {
                    if (proSync != null && proSync.Id != 0 && proSync.HasExited == false)
                    {
                        uint msg = Win.RegisterWindowMessage(Win.msgstr);
                        if (msg != 0)
                        {
                            Win.PostMessage(Win.HWND_BROADCAST, msg, IntPtr.Zero, IntPtr.Zero);
                            // MessageBox.Show(Marshal.GetLastWin32Error().ToString());
                        }

                        //proSync.CloseMainWindow();
                        //proSync.Close();
                        //proSync.Dispose();
                        #region log
                        LogModel log = new LogModel()
                        {
                            Origin = "汽车衡_" + ClientInfo.Name,
                            FunctionName = "称点主窗体_关闭文件上传的程序",
                            Level = LogConstParam.LogLevel_Info,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            Msg = "停止文件同步程序成功。"
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                }
            }
            catch
            {

            }

            CarMeasureClientViewModel viewModel = this.DataContext as CarMeasureClientViewModel;
            viewModel.CloseCheat();
            if (!viewModel.IsSeatRestart)
            {
                try
                {
                    if (viewModel != null)
                    {
                        //viewModel.SendBackTaskInfo();
                    }
                }
                catch { }
                viewModel.ClosedHardWare();
            }

            //try
            //{
            //    if (_enableFileSync == true)
            //    {
            //        ClearPross("Talent.FIleSync");
            //    }
            //    ClearPross("Talent.CarMeasureConfig");
            //}
            //catch
            //{

            //}
            ClearPross("Talent.FIleSync");
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            //CarMeasureClientViewModel viewModel = this.DataContext as CarMeasureClientViewModel;
            //try
            //{
            //    viewModel.CloseIc();
            //}
            //catch
            //{

            //}
        }

        /// <summary>
        /// 接收全屏命令后触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fullScreenTb_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                try
                {
                    CarMeasureClientViewModel cmcvm = this.DataContext as CarMeasureClientViewModel;
                    if (cmcvm.inputPlanCodeView != null)
                    {
                        cmcvm.inputPlanCodeView.Activate();
                        cmcvm.inputPlanCodeView.businessNoTextBlock.Focus();
                    }
                    else
                    {
                        this.Topmost = true;
                        this.WindowState = WindowState.Maximized;
                        this.Focus();
                        this.Activate();
                        this.Topmost = false;
                    }
                    this.fullScreenTb.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    #region log
                    LogModel log = new LogModel()
                    {
                        Origin = "汽车衡_" + ClientInfo.Name,
                        FunctionName = "称点主窗体_接收全屏命令后触发的事件",
                        Level = LogConstParam.LogLevel_Error,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        Msg = "接收全屏命令后触发的事件异常:" + ex.Message
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }

        private void mainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.fullScreenTb.IsEnabled = false;
            }
        }

        /// <summary>
        /// 判断当前窗体或其子窗体是否是活动状态
        /// </summary>
        /// <returns></returns>
        public bool isActiveWindow()
        {
            if (this.IsActive)
                return true;
            foreach (Window w in this.OwnedWindows)
            {
                if (w.IsActive)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 关闭所有子窗体
        /// </summary>
        /// <returns></returns>
        public bool CloseChildWindow()
        {
            foreach (Window w in this.OwnedWindows)
            {
                w.Close();
            }
            return true;
        }
    }
}
