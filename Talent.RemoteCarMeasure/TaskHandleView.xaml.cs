using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.ClientCommMethod;
using Talent.ClientCommonLib;
using Talent.ClientCommonLib.Controls;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Commom.Control;
using Talent.RemoteCarMeasure.View;
using Talent.RemoteCarMeasure.ViewModel;
using Talent.Video.Controller;
using Talent.CommonMethod;
using System.ComponentModel;
using System.Threading;
using Talent.RemoteCarMeasure.Commom;
using System.Configuration;
using System.Net;
using Talent.Measure.WPF;
using Talent_LT.HelpClass;
using Talent.Measure.WPF.Log;
using Talent.Measure.WPF.Remote;
using Talent.Measure.DomainModel.ServiceModel;
using System.Diagnostics;

namespace Talent.RemoteCarMeasure
{
    /// <summary>
    /// TaskHandleView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskHandleView : Only_WindowBase
    {
        /// <summary>
        /// 倒计时自动确认重量计时器
        /// </summary>
        private System.Windows.Forms.Timer countDownTimer;
        /// <summary>
        /// 倒数数字
        /// </summary>
        private int count;
        Talent.Measure.DomainModel.VideoConfig cfg;
        Talent.Video.Controller.VideoController _curVideoController;
        Talent.Video.Controller.VideoController firstVideoController;
        List<IntPtr> handelList = new List<IntPtr>();
        /// <summary>
        /// 提示信息滚动字幕动画
        /// </summary>
        Storyboard sbLargeMsg = null;
        /// <summary>
        /// 日志记录
        /// </summary>
        LogsHelpClass logH = new LogsHelpClass();
        #region 临时变量
        private TaskModel tm;
        private string configName;
        private bool isOpenVideo;
        private string measureType;
        private int timeCount;
        private int weightTimeGap;
        private int weightTimeCount;
        private decimal cInWeight = 0;//传入的重量
        /// <summary>
        /// 是否允许关闭
        /// </summary>
        bool isAllowClose = false;
        WindowsFormHelpClass formHClass = new WindowsFormHelpClass();//窗体帮助类
        #region 任务统计需要反馈的参数
        private DateTime processTaskStartTime;
        /// <summary>
        /// 开始处理任务的时间
        /// </summary>
        public DateTime ProcessTaskStartTime
        {
            get { return processTaskStartTime; }
            set { processTaskStartTime = value; }
        }

        private DateTime processTaskEndTime;
        /// <summary>
        /// 结束处理任务的时间
        /// </summary>
        public DateTime ProcessTaskEndTime
        {
            get { return processTaskEndTime; }
            set { processTaskEndTime = value; }
        }
        /// <summary>
        /// 处理的任务的车牌号
        /// </summary>
        private string CarNum;
        /// <summary>
        /// 处理的任务的衡器编码
        /// </summary>
        private string ClientCode;
        /// <summary>
        /// 处理的任务的衡器名称
        /// </summary>
        private string ClientName;
        #endregion

        #region 窗体析构

        //~TaskHandleView()
        //{
        //    #region 写日志
        //    LogModel log = new LogModel()
        //    {
        //        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //        Direction = LogConstParam.Directions_In,
        //        FunctionName = "坐席_任务处理窗体_调用TaskHandleView的析构函数",
        //        Level = LogConstParam.LogLevel_Error,
        //        Msg = "开始调用TaskHandleView的析构函数",
        //        Origin = "汽车衡_" + LoginUser.Role.Name,
        //        OperateUserName = LoginUser.Name
        //    };
        //    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
        //    #endregion
        //    DisposeImage();
        //}

        ///// <summary>
        ///// 释放图片控件的资源
        ///// </summary>
        //private void DisposeImage()
        //{
        //    long memorySize = 0;
        //    try
        //    {
        //        #region 释放大小视频承载的PictureBox的资源
        //        #region 内存日志
        //        memorySize = GetMemoryAmount();
        //        LogModel log = new LogModel()
        //        {
        //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //            FunctionName = "坐席任务处理窗体_释放大视频前",
        //            Msg = "释放大视频前,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
        //        };
        //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
        //        #endregion
        //        //大视频
        //        this.videoBig.Dispose();
        //        this.videoBig = null;
        //        try
        //        {
        //            this.formHostBig.Dispose();
        //            this.formHostBig = null;
        //        }
        //        catch
        //        {
        //        }

        //        #region 内存日志
        //        memorySize = GetMemoryAmount();
        //        LogModel log4 = new LogModel()
        //        {
        //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //            FunctionName = "坐席任务处理窗体_释放大视频后",
        //            Msg = "释放大视频后,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
        //        };
        //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log4));
        //        #endregion
        //        #region 写日志
        //        LogModel log1 = new LogModel()
        //        {
        //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //            Direction = LogConstParam.Directions_In,
        //            FunctionName = "坐席_任务处理窗体_释放大视频图片控件的资源",
        //            Level = LogConstParam.LogLevel_Error,
        //            Msg = "释放大视频图片控件的资源成功",
        //            Origin = "汽车衡_" + LoginUser.Role.Name,
        //            OperateUserName = LoginUser.Name
        //        };
        //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
        //        #endregion
        //        //小视频
        //        foreach (var host in items1.Items)
        //        {
        //            System.Windows.Forms.Integration.WindowsFormsHost wfh = host as System.Windows.Forms.Integration.WindowsFormsHost;
        //            PictureBox pb = wfh.Child as PictureBox;
        //            pb.Dispose();
        //            pb = null;
        //            try
        //            {
        //                wfh.Dispose();
        //            }
        //            catch
        //            {

        //            }

        //        }
        //        #region 写日志
        //        LogModel log2 = new LogModel()
        //        {
        //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //            Direction = LogConstParam.Directions_In,
        //            FunctionName = "坐席_任务处理窗体_释放小视频图片控件集合的资源",
        //            Level = LogConstParam.LogLevel_Error,
        //            Msg = "释放小视频图片控件集合的资源成功",
        //            Origin = "汽车衡_" + LoginUser.Role.Name,
        //            OperateUserName = LoginUser.Name
        //        };
        //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
        //        #endregion
        //        #region 内存日志
        //        memorySize = GetMemoryAmount();
        //        LogModel log5 = new LogModel()
        //        {
        //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //            FunctionName = "坐席任务处理窗体_释放小视频后",
        //            Msg = "释放小视频后,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
        //        };
        //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log5));
        //        #endregion
        //        #endregion
        //        #region 写日志
        //        //LogModel log3 = new LogModel()
        //        //{
        //        //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //        //    Direction = LogConstParam.Directions_In,
        //        //    FunctionName = "坐席_任务处理窗体_调用Only_WindowBase的Dispose释放资源",
        //        //    Level = LogConstParam.LogLevel_Error,
        //        //    Msg = "调用Only_WindowBase的Dispose释放资源成功",
        //        //    Origin = "汽车衡_" + LoginUser.Role.Name,
        //        //    OperateUserName = LoginUser.Name
        //        //};
        //        //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        #region 写日志
        //        LogModel log1 = new LogModel()
        //        {
        //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //            Direction = LogConstParam.Directions_In,
        //            FunctionName = "坐席_任务处理窗体_释放图片控件的资源",
        //            Level = LogConstParam.LogLevel_Error,
        //            Msg = "释放图片控件的资源时异常:" + ex.Message + "堆栈:" + ex.StackTrace,
        //            Origin = "汽车衡_" + LoginUser.Role.Name,
        //            OperateUserName = LoginUser.Name
        //        };
        //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
        //        #endregion
        //    }
        //}

        #endregion
        #endregion
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tm"></param>
        /// <param name="configName"></param>
        /// <param name="isOpenVideo"></param>
        /// <param name="measureType"></param>
        /// <param name="timeCount"></param>
        /// <param name="weightTimeGap"></param>
        /// <param name="weightTimeCount"></param>
        /// <param name="inWeight">新增参数 lt 2016-2-18 14:08:30 解决查询相似重量时刚开始没有接收到消息服务器重量信息</param>
        public TaskHandleView(TaskModel tm, string configName, bool isOpenVideo, string measureType, int timeCount, int weightTimeGap, int weightTimeCount, decimal inWeight)
        {
            InitializeComponent();
            isAllowClose = false;
            cInWeight = inWeight;
            sbLargeMsg = new Storyboard();
            countDownTimer = new System.Windows.Forms.Timer();
            countDownTimer.Interval = 1000;
            countDownTimer.Tick += countDownTimer_Tick;

            this.tm = tm;
            this.configName = configName;
            this.isOpenVideo = isOpenVideo;
            this.measureType = measureType;
            this.timeCount = timeCount;
            this.weightTimeGap = weightTimeGap;
            this.weightTimeCount = weightTimeCount;
            this.processTaskStartTime = DateTime.Now;
            CarNum = this.tm == null ? "" : this.tm.CarNumber;
            this.ClientCode = this.tm == null ? "" : this.tm.ClientId;
            this.ClientName = this.tm == null ? "" : this.tm.ClientName;
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister(this);
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<bool>(this, "OpenFirstVideo", new Action<bool>(OpenBigVideo));
        }

        /// <summary>
        /// 打开大视频
        /// </summary>
        /// <param name="isOpen">是否打开大视频</param>
        private void OpenBigVideo(bool isOpen)
        {
            if (isOpen)
            {
                OpenFirstVideo();
            }
        }

        /// <summary>
        /// 读取视频配置
        /// </summary>
        private void ReadVideoConfig()
        {
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            string configPath = System.IO.Path.Combine(basePath, this.tm.ClientId + ".xml");
            Talent.Measure.DomainModel.ConfigReader cfgReader1 = new Talent.Measure.DomainModel.ConfigReader(configPath);
            cfg = ConfigReader.ReadVideoConfig();
            cfg.CameraList = (from r in cfg.CameraList select r).OrderBy(c => c.Position).ToList();
        }

        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="msg"></param>
        void _curVideoController_OnShowErrMsg(string msg)
        {
            //  System.Windows.Forms.MessageBox.Show(msg);
        }

        /// <summary>
        /// 第一个视频显示错误消息
        /// </summary>
        /// <param name="msg"></param>
        void firstVideoController_OnShowErrMsg(string msg)
        {
            System.Windows.Forms.MessageBox.Show(msg);
        }

        private void btnImgLeft_Click(object sender, RoutedEventArgs e)
        {
            scrollView.ScrollToHorizontalOffset(scrollView.HorizontalOffset - 140);
        }

        private void btnImgRight_Click(object sender, RoutedEventArgs e)
        {
            scrollView.ScrollToHorizontalOffset(scrollView.HorizontalOffset + 140);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TaskHandleViewModel viewModel = this.DataContext as TaskHandleViewModel;
            CloseHandleTask closeHandleTask = new CloseHandleTask(viewModel.HandleTask.ClientId);
            closeHandleTask.ShowDialog();
            switch (closeHandleTask.FormState)
            {
                case 0:
                    break;
                case 1:
                    viewModel.HandleResult = "任务回退";//任务回退
                    //回退任务业务处理暂时是和转发他人类似，只是回退任务和转发任务相比，少了选择坐席。
                    BackTask();
                    viewModel.ShowBusy = true;
                    CloseForm(viewModel);
                    //SaveDoResult(viewModel.HandleResult);
                    break;
                case 2:
                    //转发他人业务在关闭弹出的小窗体中已经实现过了
                    viewModel.HandleResult = "转发他人";
                    isAllowClose = true;
                    viewModel.ShowBusy = true;
                    CloseForm(viewModel);
                    //SaveDoResult(viewModel.HandleResult);
                    break;
            }
        }

        /// <summary>
        /// 关闭视频控制器
        /// </summary>
        private void CloseVideoControllers()
        {
            if (_curVideoController != null)
            {
                try
                {
                    _curVideoController.Stop();
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_停止小视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "停止小视频成功 ",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_停止小视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "停止小视频时异常:" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                try
                {
                    _curVideoController.Close();
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_关闭小视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "关闭小视频成功 ",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_关闭小视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "关闭小视频时异常:" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
            if (firstVideoController != null)
            {
                try
                {
                    firstVideoController.Stop();
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_停止大视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "停止大视频成功 ",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_停止大视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "停止大视频时异常:" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                try
                {
                    firstVideoController.Close();
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_关闭大视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "关闭大视频成功 ",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_关闭大视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "关闭大视频时异常:" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
            TaskHandleViewModel viewModel = this.DataContext as TaskHandleViewModel;
            if (viewModel.audioController != null && viewModel.IsVoiceOpend && viewModel.VoiceTalkButtonCotent.Equals("关闭对讲"))
            {
                try
                {
                    viewModel.audioController.Stop();
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_停止语音对讲",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "停止语音对讲成功 ",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_停止语音",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "停止语音时异常:" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                try
                {
                    viewModel.audioController.Close();
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_关闭语音对讲",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "关闭语音对讲成功 ",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_关闭语音",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "关闭语音时异常:" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }

        /// <summary>
        /// 退回任务
        /// </summary>
        private void BackTask()
        {
            TaskHandleViewModel viewModel = this.DataContext as TaskHandleViewModel;
            if (viewModel != null)
            {
                try
                {
                    SocketCls.Emit(SeatSendCmdEnum.backTask, viewModel.HandleTask.ClientId);
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        FunctionName = "坐席_任务处理窗体_退回任务",
                        Level = LogConstParam.LogLevel_Info,
                        Direction = LogConstParam.Directions_Out,
                        Msg = "发送退回任务命令【" + SeatSendCmdEnum.backTask + "】给任务服务器",
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.backTask }, new DataParam() { ParamName = "ClientId", ParamValue = viewModel.HandleTask.ClientId } },
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    isAllowClose = true;
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("退回任务失败,请确认与任务服务器连接是否正常!");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        FunctionName = "坐席_任务处理窗体_退回任务",
                        Level = LogConstParam.LogLevel_Error,
                        Direction = LogConstParam.Directions_Out,
                        Msg = "发送退回任务命令【" + SeatSendCmdEnum.backTask + "】给任务服务器失败",
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.backTask }, new DataParam() { ParamName = "ClientId", ParamValue = viewModel.HandleTask.ClientId } },
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }

        private PictureBox tempBorderMax = new PictureBox();
        private PictureBox tempBorderMin = new PictureBox();

        /// <summary>
        /// 窗体是否可用值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if ((bool)e.NewValue == false)
                {
                    TaskHandleViewModel tvm = this.DataContext as TaskHandleViewModel;
                    tvm.ShowBusy = true;
                    SocketCls.listenEvent -= tvm.SocketCls_listenEvent;
                    isAllowClose = true;
                    formHClass.CloseOneForm("智能化远程集中计量管理系统(相似重量)");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_任务处理窗体_Window_IsEnabledChanged",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "任务处理窗体即将关闭。",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    CloseForm(tvm);
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_Window_IsEnabledChanged",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "Window_IsEnabledChanged事件里发生异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 关闭窗体方法
        /// </summary>
        /// <param name="tvm"></param>
        private void CloseForm(TaskHandleViewModel tvm)
        {
            try
            {
                CloseVideoControllers();
                SaveFormLocation();
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_记录关闭时的坐标及释放内存",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "记录关闭时的坐标及释放内存时异常:" + ex.Message + "堆栈:" + ex.StackTrace,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
            }
            SaveDoResult(tvm.HandleResult);
        }

        private void videoMin_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                #region 先交换大小视频的宽高
                int minW = 0;
                int minH = 0;
                tempBorderMin = sender as PictureBox;
                if (tempBorderMin.Width > 210)
                    return;
                tempBorderMax = formHostBig.Child as PictureBox;
                minH = tempBorderMin.Height;
                minW = tempBorderMin.Width;
                tempBorderMin.Width = tempBorderMax.Width;
                tempBorderMin.Height = tempBorderMax.Height;
                tempBorderMax.Width = minW;
                tempBorderMax.Height = minH;
                #endregion
                ExchangeVideo(tempBorderMin, tempBorderMax);
                formHostBig.Child = tempBorderMin;
            }
        }

        /// <summary>
        /// 交换大小摄像头
        /// </summary>
        /// <param name="tempBorderMin">小摄像头</param>
        /// <param name="tempBorderMax">大摄像头</param>
        private void ExchangeVideo(PictureBox tempBorderMin, PictureBox tempBorderMax)
        {
            for (int i = 0; i < items1.Items.Count; i++)
            {
                PictureBox minPb = ((System.Windows.Forms.Integration.WindowsFormsHost)items1.Items[i]).Child as PictureBox;
                if (minPb == tempBorderMin)
                {
                    ((System.Windows.Forms.Integration.WindowsFormsHost)items1.Items[i]).Child = tempBorderMax;
                    break;
                }
            }
        }

        private void borderRoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Pop_Parent.IsOpen)
                Pop_Parent.IsOpen = false;
            else
                Pop_Parent.IsOpen = true;
        }

        private void btnPop1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Pop_Child1.IsOpen = true;
            this.ClosePopup(Pop_Child1);
        }

        private void btnPop2_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void btnPop_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.ClosePopup(null);
        }

        private void Pop_Child_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Popup pop = sender as Popup;
            //pop.IsOpen = false;
        }

        private void ClosePopup(Popup curPop)
        {
            UIElementCollection children = gridMenu.Children;
            foreach (UIElement ui in children)
            {
                if (ui is Popup)
                {
                    Popup pop = ui as Popup;
                    if (curPop != null)
                    {
                        if (pop.Name != curPop.Name)
                            pop.IsOpen = false;
                    }
                    else
                    {
                        pop.IsOpen = false;
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLastLocation();

            //Thread t1 = new Thread(new ThreadStart(initInfo));
            //t1.Start();

            Talent.ClinetLog.SysLog.Log("----------------------------------------------------------------------------------任务处理开始----------------------------------------------------------------------------------");

            ReadVideoConfig();
            //OpenFirstVideo();
            Thread t1 = new Thread(new ThreadStart(VideoOpen));
            t1.IsBackground = true;
            t1.Start();

            TaskHandleViewModel tvm = this.DataContext as TaskHandleViewModel;
            tvm.gridReader = rendergv;
            tvm.gridSupplier = gridSupplier;
            tvm.gridMeasure = gridMeasure;
            tvm.gridMeasureWeight = gridMeasureWeight;
            tvm.dropDownPop = this.DropDownPop;
            tvm.dataViewPop = DropDataViewPop;
            tvm.popParentGrid = this.gridOuter;
            tvm.InitForms(tm, configName, isOpenVideo, measureType, timeCount, weightTimeGap, weightTimeCount, cInWeight);
        }

        /// <summary>
        /// 构造窗体数据
        /// </summary>
        private void initInfo()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
           {
               Talent.ClinetLog.SysLog.Log("----------------------------------------------------------------------------------任务处理开始----------------------------------------------------------------------------------");
               TaskHandleViewModel tvm = this.DataContext as TaskHandleViewModel;
               //tvm.BusyText = "读取视频配置中..";
               ReadVideoConfig();
               //tvm.BusyText = "打开视频中..";
               OpenFirstVideo();
               //tvm.BusyText = "信息加载中..";
               tvm.IsGetWeight = true;
               tvm.gridReader = rendergv;
               tvm.gridSupplier = gridSupplier;
               tvm.gridMeasure = gridMeasure;
               tvm.gridMeasureWeight = gridMeasureWeight;
               tvm.dropDownPop = this.DropDownPop;
               tvm.dataViewPop = DropDataViewPop;
               tvm.popParentGrid = this.gridOuter;
               tvm.InitForms(tm, configName, isOpenVideo, measureType, timeCount, weightTimeGap, weightTimeCount, cInWeight);
           }));
        }

        /// <summary>
        /// 打开视频
        /// </summary>
        private void VideoOpen()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    TaskHandleViewModel thv = this.DataContext as TaskHandleViewModel;
                    thv.BusyText = "加载视频中..";
                    OpenFirstVideo();
                    if (isOpenVideo)
                    {
                        OpenRemainingVideos();
                    }
                    thv.IsGetWeight = true;
                    thv.ShowBusy = false;
                }));
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体_打开视频VideoOpen()",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "打开视频时异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
            }
        }

        private void rollMsg_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        /// <summary>
        /// 倒计时时间间隔触发的事件
        /// </summary>
        void countDownTimer_Tick(object sender, EventArgs e)
        {
            if (count > 0)
            {
                Countdown txt = new Countdown(this.gridCountdown, HandleTaskView);
                txt.TxtValue = count.ToString();
                txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                this.gridCountdown.Children.Add(txt);
                count--;
                this.ConfirmButton.Content = "确 定(" + count.ToString() + ")";
                this.txtCountdown.Text = count.ToString();
            }
            else
            {
                //倒计时完毕调用方法
                countDownTimer.Stop();
                TaskHandleViewModel viewModel = this.DataContext as TaskHandleViewModel;
                viewModel.TimeCountVisibility = Visibility.Hidden;
                viewModel.TimerStart();
                this.ConfirmButton.Content = "确 定";
            }
        }

        /// <summary>
        /// 打开配置文件中除第一个视频外的所有视频
        /// </summary>
        private void OpenRemainingVideos()
        {
            try
            {
                List<IntPtr> handelList = new List<IntPtr>();
                long memorySize = 0;
                for (int i = 1; i < cfg.CameraList.Count; i++)//从第二个视频开始，第一个视频是常打开视频,2016-01-05改动
                {
                    System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
                    host.Width = 124; host.Height = 124;
                    PictureBox pb = new PictureBox() { BackColor = System.Drawing.Color.Pink, Width = 100, Height = 120, Margin = new Padding(3, 0, 3, 0) };
                    pb.MouseUp += videoMin_MouseUp;
                    handelList.Add(pb.Handle);
                    host.Child = pb;
                    items1.Items.Add(host);
                }
                if (handelList.Count > 0)
                {
                    System.GC.Collect();
                    memorySize = GetMemoryAmount();
                    #region 内存日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "坐席任务处理窗体_打开小视频前",
                        Msg = "打开小视频前,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                    _curVideoController = new VideoController(new FtpConfig(), cfg, handelList);
                    _curVideoController.OnShowErrMsg += _curVideoController_OnShowErrMsg;
                    _curVideoController.Open();
                    _curVideoController.Start();
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_任务处理窗体_打开小视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "打开各小视频",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                #region 内存日志
                memorySize = GetMemoryAmount();
                LogModel log5 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体_打开小视频后",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "打开小视频后,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)",
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log5));
                #endregion
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_打开小视频",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "打开各小视频时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                OpenVideoCheckBox.IsChecked = false;
            }
        }

        /// <summary>
        /// 获取进程使用的内存大小
        /// </summary>
        /// <returns></returns>
        private long GetMemoryAmount()
        {
            long usedMemory = 0;
            try
            {
                Process proc = Process.GetCurrentProcess();
                usedMemory = proc.PrivateMemorySize64;
            }
            catch
            { }
            return usedMemory;
        }

        /// <summary>
        /// 倒计时可见性改变触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridCountdown_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TaskHandleViewModel viewModel = this.DataContext as TaskHandleViewModel;
            if (viewModel.TimeCountVisibility == Visibility.Visible)
            {
                this.count = viewModel.TimeCount;
                if (countDownTimer != null)
                {
                    countDownTimer.Start();
                }
            }

        }

        /// <summary>
        /// 打开视频单选checkBox选中触发的事件
        /// </summary>
        private void OpenVideoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_curVideoController != null)
                {
                    _curVideoController.Open();
                    _curVideoController.Start();
                }
                else
                {
                    System.GC.Collect();
                    OpenRemainingVideos();
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_打开小视频",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "初始化并启动小视频时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 打开第一个视频
        /// </summary>
        private void OpenFirstVideo()
        {
            try
            {
                List<IntPtr> handelList = new List<IntPtr>();
                long memorySize = GetMemoryAmount();

                if (cfg.CameraList.Count > 0)
                {
                    System.GC.Collect();
                    #region 内存日志
                    memorySize = GetMemoryAmount();
                    LogModel log2 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "坐席任务处理窗体_打开大视频前",
                        Msg = "打开大视频前,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                    #endregion
                    videoBig.MouseUp += videoMin_MouseUp;
                    handelList.Add(videoBig.Handle);
                    formHostBig.Child = videoBig;
                    
                    firstVideoController = new VideoController(new FtpConfig(), cfg, handelList);
                    //firstVideoController.OnShowErrMsg += firstVideoController_OnShowErrMsg;
                    firstVideoController.Open();
                    firstVideoController.Start();
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_任务处理窗体_打开大视频",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "打开大视频",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                #region 内存日志
                memorySize = GetMemoryAmount();
                LogModel log3 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体_打开大视频后",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "打开大视频后,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)",
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
                #endregion
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_打开大视频",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "打开大视频时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 打开视频单选checkBox未选中触发的事件
        /// </summary>
        private void OpenVideoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_curVideoController != null)
            {
                _curVideoController.Stop();
            }
        }

        /// <summary>
        /// 选择从服务查询得到的基础信息触发的事件
        /// </summary>
        private void SelectBasicInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button b = sender as System.Windows.Controls.Button;
            System.Windows.Controls.TextBox tb = this.DropDownPop.PlacementTarget as System.Windows.Controls.TextBox;
            tb.Text = b.Content.ToString();
            this.DropDownPop.IsOpen = false;
            this.ClosePopup(DropDownPop);
            TaskHandleViewModel vm = this.DataContext as TaskHandleViewModel;
            vm.SendBusinessInfosToTaskServer();
        }

        /// <summary>
        /// dataGrid行选中事件(目前只有计划号对应的查询是使用datagrid)
        /// </summary>
        private void PopDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DropDataViewPop.IsOpen = false;
            SetAdornerVisibility(Visibility.Collapsed);
            this.ClosePopup(DropDownPop);
            TaskCodeModel tcM = this.PopDataGrid.SelectedItem as TaskCodeModel;
            if (tcM != null)
            {
                TaskHandleViewModel vm = this.DataContext as TaskHandleViewModel;
                vm.DoTaskCode(tcM);
            }
        }

        /// <summary>
        /// 关闭之前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleTaskView_Closing(object sender, CancelEventArgs e)
        {
            if (!isAllowClose)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 保存任务结果
        /// </summary>
        /// <param name="handleResult">任务处理结果</param>
        private void SaveDoResult(string handleResult)
        {
            string url = "";
            string paramStr = "";
            try
            {
                this.ProcessTaskEndTime = DateTime.Now.AddSeconds(-1);//结束时间强制减少一秒2016-4-1 10:26:12……
                string serviceUrl = ConfigurationManager.AppSettings["saveTaskDoResult"].ToString();
                var param = new
                {
                    opname = LoginUser.Name,
                    opcode = LoginUser.Code,
                    taskbegintime = this.ProcessTaskStartTime.ToString("yyyyMMddHHmmss"),//时间改为24 小时制 2016-3-7 09:03:20……
                    taskendtime = this.ProcessTaskEndTime.ToString("yyyyMMddHHmmss"),
                    equcode = this.ClientCode,
                    equname = this.ClientName,
                    carno = this.CarNum,
                    taskdoresult = handleResult,
                    memo = measureType,
                    equtype = EquTypeEnum.Type_Car_Seat,
                    seatid = LoginUser.Role.Code,
                    seatname = LoginUser.Role.Name
                };
                paramStr = "[" + JsonConvert.SerializeObject(param) + "]";
                url = string.Format(serviceUrl, System.Web.HttpUtility.UrlEncode(paramStr, System.Text.Encoding.UTF8));
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(url, 10);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_保存任务处理结果",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "保存任务处理结果!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    Data = paramStr,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    OperateUserName = LoginUser.Name,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "Url", ParamValue = url } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                request.BeginGetResponse(new AsyncCallback(saveTaskDoResultCallback), request);
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_保存任务处理结果",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "保存任务处理结果时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    Data = paramStr,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    OperateUserName = LoginUser.Name,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "Url", ParamValue = url } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //异常情况下，继续走后续的流程
                saveTaskDoResultCallback(null);
            }
        }

        /// <summary>
        /// 保存任务结果回调方法(后续处理流程)
        /// </summary>
        /// <param name="ar"></param>
        private void saveTaskDoResultCallback(IAsyncResult ar)
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "坐席_任务处理窗体_保存任务结果回调方法",
                Level = LogConstParam.LogLevel_Info,
                Msg = "获取到保存任务结果服务的反馈。",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }));
        }
        /// <summary>
        /// 换肤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                e.Handled = true;
                TaskHandleViewModel rcvm = this.DataContext as TaskHandleViewModel;
                System.Windows.Point cP = e.GetPosition(null);
                ChangeSkinColor changeSkin = new ChangeSkinColor();
                changeSkin.Left = cP.X;
                changeSkin.ToolTip = cP.Y;
                changeSkin.SetValue(rcvm.BColor0);
                changeSkin.ShowDialog();
                string rtClore = changeSkin.rtColor;
                DoRtColor(rtClore);

            }
        }
        /// <summary>
        /// 处理返回的颜色
        /// </summary>
        /// <param name="rtColor"></param>
        private void DoRtColor(string rtColor)
        {
            if (!string.IsNullOrEmpty(rtColor))
            {
                TaskHandleViewModel rcvm = this.DataContext as TaskHandleViewModel;
                rcvm.BColor0 = rtColor;
                string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
                string url = basePath + @"/ClientConfig/UserDoResultColor.txt";
                FileHelper.WriteText(url, rtColor);
            }

        }

        // 支持标题栏拖动  
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //bool isFirstChangeWindowsState = false;
            base.OnMouseLeftButtonDown(e);
            // 获取鼠标相对标题栏位置  
            System.Windows.Point position = e.GetPosition(gridOuter);
            // 如果鼠标位置在标题栏内，允许拖动  
            if (position.X >= 0 && position.X < gridOuter.ActualWidth && position.Y >= 0 && position.Y < gridOuter.ActualHeight)
            {
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1)
                {
                    this.DragMove();
                }
                //if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                //{
                //    if (this.WindowState == WindowState.Normal)
                //    {
                //        isFirstChangeWindowsState = true;
                //        this.WindowState = WindowState.Maximized;
                //    }
                //    if (isFirstChangeWindowsState == false)
                //    {
                //        if (this.WindowState == WindowState.Maximized)
                //        {
                //            this.WindowState = WindowState.Normal;
                //        }
                //    }
                //}
            }
        }

        /// <summary>
        /// 设置真正的值
        /// </summary>
        /// <param name="rcvm"></param>
        private void SetShowValue(TaskHandleViewModel rcvm, System.Windows.Controls.TextBox tx)
        {
            tx.Text = string.Empty;
            string txName = tx.Name;
            switch (txName)
            {
                case "txtGrossb":
                    rcvm.HandleTask.BullInfo.grossb = 0;
                    rcvm.HandleTask.BullInfo.tareb = 0;
                    rcvm.HandleTask.BullInfo.suttleb = 0;
                    break;
                case "txtdeduction":
                    rcvm.HandleTask.BullInfo.deduction = 0;
                    break;
                case "txtTareb":
                    rcvm.HandleTask.BullInfo.tareb = 0;
                    break;
            }


        }
        /// <summary>
        /// 记录窗体关闭时的坐标
        /// </summary>
        private void SaveFormLocation()
        {
            try
            {
                Properties.Settings.Default.HandleTaskViewLeft = this.Left.ToString();
                Properties.Settings.Default.HandleTaskViewTop = this.Top.ToString();
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_记录关闭时的坐标",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "记录关闭时的坐标异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
            }
        }
        /// <summary>
        ///加载默认值
        /// </summary>
        private void LoadLastLocation()
        {
            try
            {
                this.Left = Convert.ToDouble(Properties.Settings.Default.HandleTaskViewLeft);
                this.Top = Convert.ToDouble(Properties.Settings.Default.HandleTaskViewTop);
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_加载上次保存的坐标",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "加载上次保存的坐标异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
            }
        }
        #region 值改变
        /// <summary>
        /// 供方毛重值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TaskHandleViewModel rcvm = this.DataContext as TaskHandleViewModel;
            string gStr = this.txtGrossb.Text.Trim();
            if (string.IsNullOrEmpty(gStr))
            {
                //logH.SaveLog("供方毛重值修改为空");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "坐席_任务处理窗体_供方毛重值改变",
                    Level = LogConstParam.LogLevel_Info,
                    Direction = LogConstParam.Directions_In,
                    Msg = "供方毛重值修改为空",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                SetShowValue(rcvm, this.txtGrossb);
            }
            else
            {
                decimal gd = 0;
                bool isD = WinFormHelpClass.GetIsDecimal(true, gStr, out gd);
                if (isD)
                {
                    if (gd >= 0)
                    {
                        rcvm.HandleTask.BullInfo.grossb = gd * 1000;
                        rcvm.ShowGrossb = gStr;
                        string tStr = this.txtTareb.Text.Trim();
                        if (string.IsNullOrEmpty(tStr))
                        {
                            rcvm.HandleTask.BullInfo.suttleb = gd * 1000;
                            rcvm.ShowSuttleb = gStr;
                        }
                        else
                        {
                            decimal td = 0;
                            WinFormHelpClass.GetIsDecimal(true, tStr, out td);
                            rcvm.HandleTask.BullInfo.suttleb = (gd - td) * 1000;
                            rcvm.ShowSuttleb = (gd - td).ToString();
                        }
                        //logH.SaveLog("供方毛重值修改为:" + gd + "  供方皮重为：" + tStr);
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            FunctionName = "坐席_任务处理窗体_供方毛重值改变",
                            Level = LogConstParam.LogLevel_Info,
                            Direction = LogConstParam.Directions_In,
                            Msg = "供方毛重值修改为:" + gd + "  供方皮重为：" + tStr,
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                    else
                    {
                        //logH.SaveLog("系统提示：供方毛重值必须大于0");
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            FunctionName = "坐席_任务处理窗体_供方毛重值改变",
                            Level = LogConstParam.LogLevel_Info,
                            Direction = LogConstParam.Directions_In,
                            Msg = "系统提示：供方毛重值必须大于0",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        System.Windows.Forms.MessageBox.Show("供方毛重值必须大于0");
                        SetShowValue(rcvm, this.txtGrossb);
                    }
                }
                else
                {
                    //logH.SaveLog("系统提示：供方毛重值请录入数值类型");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "坐席_任务处理窗体_供方毛重值改变",
                        Level = LogConstParam.LogLevel_Info,
                        Direction = LogConstParam.Directions_In,
                        Msg = "系统提示：供方毛重值请录入数值类型",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    System.Windows.Forms.MessageBox.Show("供方毛重值请录入数值类型");
                    SetShowValue(rcvm, this.txtGrossb);
                }
            }
        }

        /// <summary>
        /// 扣重录入值改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtdeduction_TextChanged(object sender, TextChangedEventArgs e)
        {
            TaskHandleViewModel rcvm = this.DataContext as TaskHandleViewModel;
            string dStr = this.txtdeduction.Text.Trim();
            if (string.IsNullOrEmpty(dStr))
            {
                SetShowValue(rcvm, this.txtdeduction);
            }
            else
            {
                decimal dd = 0;
                bool isD = WinFormHelpClass.GetIsDecimal(true, dStr, out dd);
                if (isD)
                {
                    if (dd >= 0)
                    {
                        rcvm.HandleTask.BullInfo.deduction = dd * 1000;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("扣重值必须大于0");
                        SetShowValue(rcvm, this.txtdeduction);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("扣重值请录入数值类型");
                    SetShowValue(rcvm, this.txtdeduction);
                }
            }
        }

        /// <summary>
        /// 供方皮重值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            TaskHandleViewModel rcvm = this.DataContext as TaskHandleViewModel;
            string tStr = this.txtTareb.Text.Trim();
            if (string.IsNullOrEmpty(tStr))
            {
                //logH.SaveLog("供方皮重值修改为空");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "坐席_任务处理窗体_供方皮重值改变",
                    Level = LogConstParam.LogLevel_Info,
                    Direction = LogConstParam.Directions_In,
                    Msg = "供方皮重值修改为空",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                SetShowValue(rcvm, this.txtTareb);
            }
            else
            {
                decimal td = 0;
                bool isD = WinFormHelpClass.GetIsDecimal(true, tStr, out td);
                if (isD)
                {
                    if (td >= 0)
                    {
                        rcvm.HandleTask.BullInfo.tareb = td * 1000;
                        rcvm.ShowTareb = tStr;
                        string gStr = this.txtGrossb.Text.Trim();
                        if (string.IsNullOrEmpty(gStr))
                        {
                            rcvm.HandleTask.BullInfo.suttleb = 0;
                            rcvm.ShowSuttleb = string.Empty;
                        }
                        else
                        {
                            decimal gd = 0;
                            WinFormHelpClass.GetIsDecimal(true, gStr, out gd);
                            rcvm.HandleTask.BullInfo.suttleb = (gd - td) * 1000;
                            rcvm.ShowSuttleb = (gd - td).ToString();
                        }
                        //logH.SaveLog("供方毛重值修改为:" + gStr + "  供方皮重为：" + tStr);
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            FunctionName = "坐席_任务处理窗体_供方皮重值改变",
                            Level = LogConstParam.LogLevel_Info,
                            Direction = LogConstParam.Directions_In,
                            Msg = "供方毛重值修改为:" + gStr + "  供方皮重为：" + tStr,
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                    else
                    {
                        //logH.SaveLog("系统提示：供方皮重值必须大于0");
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            FunctionName = "坐席_任务处理窗体_供方皮重值改变",
                            Level = LogConstParam.LogLevel_Info,
                            Direction = LogConstParam.Directions_In,
                            Msg = "系统提示：供方皮重值必须大于0",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        System.Windows.Forms.MessageBox.Show("供方皮重值必须大于0");
                        SetShowValue(rcvm, this.txtTareb);
                    }
                }
                else
                {
                    //logH.SaveLog("系统提示：供方皮重值请录入数值类型");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "坐席_任务处理窗体_供方皮重值改变",
                        Level = LogConstParam.LogLevel_Info,
                        Direction = LogConstParam.Directions_In,
                        Msg = "系统提示：供方皮重值请录入数值类型",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    System.Windows.Forms.MessageBox.Show("供方皮重值请录入数值类型");
                    SetShowValue(rcvm, this.txtTareb);
                }
            }
        }
        #endregion

        /// <summary>
        /// 查询业务号对应的信息
        /// </summary>
        private void GetBusinessAbortInfos(string infos)
        {
            string serviceUrl = ConfigurationManager.AppSettings["getBusinessNoAbortInfo"].ToString();
            string getUrl = string.Format(serviceUrl, infos);
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(getBusinessAbortInfosCallback), request);
            #region 日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_任务处理窗体_查询业务号对应的信息",
                Level = LogConstParam.LogLevel_Info,
                Msg = "开始从服务器查询业务号相关基础数据!",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name,
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "URL", ParamValue = getUrl } }
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        // Pri
        private CPri Pri = new CPri();
        private class CPri
        {
            public bool ThisLoadedIf = false;
            public AdornerLayer ThisAdornerLayer = null;
            public T4PopupControlOverlayAdorner OverlayAdorner = null;
            public T4PopupControlContentAdorner ContentAdorner = null;
            public Grid GrdContent = null;
        }
        /// <summary>
        /// 通过业务号查询相关基础信息的回调方法
        /// </summary>
        /// <param name="asyc"></param>
        public void getBusinessAbortInfosCallback(IAsyncResult asyc)
        {
            //以下代码未开发完
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                var taskCodeInfos = InfoExchange.DeConvert(typeof(List<TaskCodeModel>), strResult) as List<TaskCodeModel>;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_通过业务号查询相关基础信息的回调方法",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务器查询基础数据成功!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = taskCodeInfos,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                if (taskCodeInfos != null && taskCodeInfos.Count > 0)
                {
                    rendergv.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //DropDataViewPop.PlacementTarget = this.TaskCodeTxt;
                        //DropDataViewPop.Placement = PlacementMode.Bottom;
                        Pri.ThisAdornerLayer = AdornerLayer.GetAdornerLayer(this.TaskCodeTxt);
                        Pri.OverlayAdorner = new T4PopupControlOverlayAdorner(this.TaskCodeTxt);
                        Pri.ThisAdornerLayer.Add(Pri.OverlayAdorner);
                        SetAdornerVisibility(Visibility.Collapsed);
                        Pri.OverlayAdorner.MouseDown -= OverlayAdorner_MouseDown;
                        Pri.OverlayAdorner.MouseDown += OverlayAdorner_MouseDown;

                        this.DropDataViewPop.IsOpen = true;
                        SetAdornerVisibility(Visibility.Visible);
                        System.Windows.Controls.DataGrid dg = DropDataViewPop.FindName("PopDataGrid") as System.Windows.Controls.DataGrid;
                        dg.ItemsSource = taskCodeInfos;
                        //DropDataViewPop.IsOpen = true;
                    }));

                }
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_通过业务号查询相关基础信息的回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务查询基础信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        void OverlayAdorner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DropDataViewPop.IsOpen = false;
            SetAdornerVisibility(Visibility.Collapsed);
        }
        private void SetAdornerVisibility(Visibility vi)
        {
            if (vi == Visibility.Visible)
            {
                Pri.OverlayAdorner.Visibility = Visibility.Visible;
            }
            if (vi == Visibility.Collapsed)
            {
                Pri.OverlayAdorner.Visibility = Visibility.Collapsed;
            }
        }
        private void TaskCodeTxt_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }
            string txt = this.TaskCodeTxt.Text.Trim();
            if (string.IsNullOrEmpty(txt))
            {
                return;
            }
            GetBusinessAbortInfos(txt);
        }

        private void HandleTaskView_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                TaskHandleViewModel vm = this.DataContext as TaskHandleViewModel;
                vm.saveWeightServiceInfo();
            }
        }

     
    }
    public class T4PopupControlOverlayAdorner : Adorner
    {
        private CPri Pri = new CPri();
        private class CPri
        {
            public Grid OverlayGrid = null;
        }
        internal T4PopupControlOverlayAdorner(UIElement AdornedElement)
            : base(AdornedElement)
        {
            Pri.OverlayGrid = new Grid() { Background = new SolidColorBrush(Colors.Transparent), };
            this.IsHitTestVisible = true; this.AddVisualChild(Pri.OverlayGrid);
        }
        protected override int VisualChildrenCount { get { return 1; } }
        protected override Visual GetVisualChild(int index) { return this.Pri.OverlayGrid; }
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            return constraint;
        }
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            var Host = FindParent<Window>(this.AdornedElement);
            var Pos = this.AdornedElement.TranslatePoint(default(System.Windows.Point), Host);
            this.Pri.OverlayGrid.Arrange(new Rect()
            {
                X = -Pos.X,
                Y = -Pos.Y,
                Width = finalSize.Width,
                Height = finalSize.Height
            });
            return base.ArrangeOverride(finalSize);
        }
        private T FindParent<T>(DependencyObject Child) where T : DependencyObject
        {
            var Rlt = null as T;
            var Tmp = VisualTreeHelper.GetParent(Child);
            if (Tmp != null)
            {
                if ((Tmp as T) != null) { Rlt = Tmp as T; }
                else { Rlt = FindParent<T>(Tmp); }
            }
            return Rlt;
        }
    }

    public class T4PopupControlContentAdorner : Adorner
    {
        private CPri Pri = new CPri();
        private class CPri
        {
            public FrameworkElement ContentVisual = null;
        }
        internal T4PopupControlContentAdorner(UIElement AdornedElement, FrameworkElement ContentVisual)
            : base(AdornedElement)
        {
            Pri.ContentVisual = ContentVisual;
            this.IsHitTestVisible = true;
            this.AddVisualChild(Pri.ContentVisual);
        }
        protected override int VisualChildrenCount { get { return 1; } }
        protected override Visual GetVisualChild(int index) { return this.Pri.ContentVisual; }
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            Pri.ContentVisual.Measure(constraint);
            var NewSize = Pri.ContentVisual.DesiredSize;
            return NewSize;
        }
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            this.Pri.ContentVisual.Arrange(new Rect()
            {
                X = 0,
                Y = 0,
                Width = finalSize.Width,
                Height = finalSize.Height,
            });
            return base.ArrangeOverride(finalSize);
        }
    }
}
