using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.ViewModel;
using Talent.Video.Controller;
using Talent_LT.HelpClass;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// VideoMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class VideoMonitorView : Window
    {
        private AudioConfig nvrConfig;
        Talent.Measure.DomainModel.VideoConfig cfg;
        private VideoConfig gCfg;
        Talent.Video.Controller.VideoController _curVideoController;
        int _playSpeedCount = 0;
        VideoPlayBackController _curVideoPlayBackController;
        public VideoMonitorView()
        {
            InitializeComponent();
            DateTime tem = DateTime.Now;
            dptPlayEndTime.Value = tem;
            dptPlayStartTime.Value = DateTime.Now.Date; //tem.Subtract(new TimeSpan(1, 0, 0, 0));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VideoMonitorViewModel vm = (VideoMonitorViewModel)this.DataContext;

                #region 关闭语音对讲
                if (vm.audioController != null)
                {
                    vm.audioController.Stop();
                    vm.audioController.Close();
                }
                VoiceRemindHelper.SendVoiceTalkEndToMeasure(vm.CurClientModel.ClientId);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "视频监控窗体_窗体关闭事件",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "关闭【语音对讲】，同时通知任务服务器",
                    Origin = LoginUser.Role.Name,
                    Data = new { clientid = vm.CurClientModel.ClientId, cmd = ParamCmd.Voice_Prompt, msg = "语音对讲结束" },
                    IsDataValid = LogConstParam.DataValid_Ok,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                #endregion

                #region 关闭视频
                if (_curVideoController != null)
                {
                    _curVideoController.Stop();
                    _curVideoController.Close();
                    #region 写日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "视频监控窗体_窗体关闭事件",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "关闭视频成功",
                        Origin = LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                }
                _curVideoController = null;
                #endregion

                #region 关闭视频回放

                if (_curVideoPlayBackController != null)
                {
                    _curVideoPlayBackController.PlayStop();
                    #region 写日志
                    LogModel log2 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "视频监控窗体_窗体关闭事件",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "关闭历史回放视频成功",
                        Origin = LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                    #endregion
                }
                _curVideoPlayBackController = null;
                #endregion

                #region 释放视频监控资源
                try
                {
                    #region 内存日志
                    long memorySize = GetMemoryAmount();
                    LogModel log3 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "视频监控_释放WindowsFormsHost前",
                        Msg = "释放WindowsFormsHost前,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
                    #endregion
                    this.formHostBig.Dispose();
                    this.formHostBig = null;
                    #region 内存日志
                    memorySize = GetMemoryAmount();
                    LogModel log4 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "视频监控_释放WindowsFormsHost后",
                        Msg = "释放WindowsFormsHost后,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log4));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "视频监控窗体_窗体关闭事件",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "释放视频资源时异常:" + ex.Message,
                        Origin = LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                }
                #endregion

                #region 释放历史回放监控资源
                try
                {
                    this.formHis.Dispose();
                    this.formHis = null;
                    #region 写日志
                    LogModel log2 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "视频监控窗体_窗体关闭事件",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "释放历史回放视频资源成功",
                        Origin = LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "视频监控窗体_窗体关闭事件",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "释放历史回放视频资源时异常:" + ex.Message,
                        Origin = LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                }
                #endregion

                this.Close();
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "视频监控窗体_窗体关闭事件",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "窗体关闭时异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
        #region 方法

        /// <summary>
        /// 读取NVR配置信息
        /// </summary>
        private void ReadNVRConfig()
        {
            WeighterClientModel wcm = (this.DataContext as VideoMonitorViewModel).CurClientModel;
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            string configPath = System.IO.Path.Combine(basePath, wcm.ClientId + ".xml");
            Talent.Measure.DomainModel.ConfigReader cfgReader1 = new Talent.Measure.DomainModel.ConfigReader(configPath);
            this.nvrConfig = ConfigReader.ReadAudioConfig();
        }


        /// <summary>
        /// 加载视频配置(某称点视频列表集合)
        /// </summary>
        private void LoadVideoList()
        {
            cfg = InitReadVideoConfig();
            videoList.ItemsSource = cfg.CameraList;
        }

        private VideoConfig InitReadVideoConfig()
        {
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            WeighterClientModel wcm = (this.DataContext as VideoMonitorViewModel).CurClientModel;
            string configPath = System.IO.Path.Combine(basePath, wcm.ClientId + ".xml");
            Talent.Measure.DomainModel.ConfigReader cfgReader1 = new Talent.Measure.DomainModel.ConfigReader(configPath);
            VideoConfig gCfg = ConfigReader.ReadVideoConfig();
            gCfg.CameraList = (from r in gCfg.CameraList select r).OrderBy(c => c.Position).ToList();
            return gCfg;
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_OutIn,
                FunctionName = "视频监控窗体_Window_Loaded",
                Level = LogConstParam.LogLevel_Info,
                Msg = "开始加载视频监控窗体"
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            VideoMonitorViewModel vm = (VideoMonitorViewModel)this.DataContext;
            vm.init();
            LoadVideoList();
        }

        private void videoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (formHostBig.Child != null)
                {
                    formHostBig.Child.Dispose();
                }
                var getCamera = videoList.SelectedItem as Camera;
                OpenVideo(getCamera);
            }
            catch(Exception ex)
            {
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "视频监控窗体_切换视频",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "切换视频时异常:" + ex.Message + "堆栈:" + ex.StackTrace,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
            }
        }

        private void OpenVideo(Camera getCamera)
        {
            List<IntPtr> handelList = new List<IntPtr>();
            PictureBox pb = new PictureBox() { BackColor = System.Drawing.Color.Black, Margin = new Padding(3, 0, 3, 0), Dock = DockStyle.Fill };//Pink 改为 black 2016-3-3 11:25:18……
            handelList.Add(pb.Handle);
            formHostBig.Child = pb;
            if (gCfg == null)
            {
                gCfg = InfoExchange.Clone(cfg);//InitReadVideoConfig();
            }
            gCfg.CameraList.Clear();
            getCamera.Position = "1";
            gCfg.CameraList.Add(getCamera);
            if (_curVideoController != null)
            {
                _curVideoController.Stop();
                _curVideoController.Close();
            }
            _curVideoController = null;
            System.GC.Collect();
            _curVideoController = new VideoController(new FtpConfig(), gCfg, handelList);
            _curVideoController.OnShowErrMsg += _curVideoController_OnShowErrMsg;
            _curVideoController.Open();
            _curVideoController.Start();
        }
        void _curVideoController_OnShowErrMsg(string msg)
        {
            //System.Windows.Forms.MessageBox.Show("预览错误：" + msg);
            //FileHelpClass.WriteLog("预览错误："+msg,"DVR");
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "视频监控窗体_底层视频控制抛异常",
                Level = LogConstParam.LogLevel_Warning,
                Msg = "预览错误：" + msg,
                Origin = LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        #region 视频回放相关
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlayByTime_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startTime = dptPlayStartTime.Value;
            DateTime? endTime = dptPlayEndTime.Value;
            if (startTime == null || endTime == null || startTime >= endTime)
            {
                return;
            }
            if (_curVideoPlayBackController != null)
            {
                _curVideoPlayBackController.PlayStop();
            }
            var getCamera = videoList.SelectedItem as Camera;
            if (getCamera != null)
            {
                ReadNVRConfig();
                VideoConfig gCfg = InfoExchange.Clone(cfg); //InitReadVideoConfig();
                gCfg.CameraList.Clear();
                getCamera.Position = "1";
                getCamera.Ip = nvrConfig.Ip;
                getCamera.PassWord = nvrConfig.PassWord;
                getCamera.UserName = nvrConfig.UserName;
                getCamera.Port = nvrConfig.Port;

                #region 日志
                string msg = string.Format("Ip={0},Port={1},UserName={2},PassWord={3},Channel={4}", nvrConfig.Ip, nvrConfig.Port, nvrConfig.UserName, nvrConfig.PassWord, getCamera.Channel);

                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "ReadNVRConfig()",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = msg
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //getCamera.Ip = dvrIp;
                //getCamera.PassWord = dvrPassW;
                //getCamera.UserName = dvrUserId;
                //getCamera.Channel = getCamera.DvrChannel;
                //System.Windows.Forms.MessageBox.Show(getCamera.Channel);
                //gCfg.VideoDriver=@"IoDllNew\hk\Talent.Video.HKVideo.dll";
                gCfg.CameraList.Add(getCamera);
                _curVideoPlayBackController = new VideoPlayBackController(gCfg, videoHis.Handle);
                _curVideoPlayBackController.OnShowErrMsg += _curVideoPlayBackController_OnShowErrMsg;
                _curVideoPlayBackController.PlayByTime(startTime.Value, endTime.Value);

                this.ButtonPlayPause.Content = "暂停";
            }
            else
            {
                //请选择
            }
        }

        private void _curVideoPlayBackController_OnShowErrMsg(string msg)
        {
            //System.Windows.Forms.MessageBox.Show("回放错误：" + msg);
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "视频监控窗体_视频回放_底层视频回放抛异常",
                Level = LogConstParam.LogLevel_Warning,
                Msg = "视频回放错误：" + msg,
                Origin = LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_curVideoPlayBackController != null)
            {
                if (ButtonPlayPause.Content.ToString().Equals("暂停"))
                {
                    _curVideoPlayBackController.PlayPause();
                    this.ButtonPlayPause.Content = "继续";
                }
                else
                {
                    _curVideoPlayBackController.PlayStart();
                    this.ButtonPlayPause.Content = "暂停";
                }
            }
        }

        /// <summary>
        /// 快进
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlayFast_Click(object sender, RoutedEventArgs e)
        {
            if (_curVideoPlayBackController != null)
            {
                if (_curVideoPlayBackController.PlayFast())
                {
                    _playSpeedCount++;
                    this.ButtonPlayFast.Content = "快进(" + _playSpeedCount.ToString() + ")";
                }
            }
        }
        /// <summary>
        /// 快退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlaySlow_Click(object sender, RoutedEventArgs e)
        {
            if (_curVideoPlayBackController != null)
            {
                if (_curVideoPlayBackController.PlaySlow())
                {
                    _playSpeedCount--;
                    this.ButtonPlayFast.Content = "快进(" + _playSpeedCount.ToString() + ")";
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlayStop_Click(object sender, RoutedEventArgs e)
        {
            if (_curVideoPlayBackController != null)
            {
                if (_curVideoPlayBackController.PlayStop())
                {
                    _playSpeedCount = 0;
                    this.ButtonPlayFast.Content = "快进";
                }
            }
        }
        #endregion

    }
}
