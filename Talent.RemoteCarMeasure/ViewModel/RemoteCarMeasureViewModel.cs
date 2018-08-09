using GalaSoft.MvvmLight.Messaging;
using Microsoft.Expression.Interactivity.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Talent.ClientCommMethod;
using Talent.ClientCommonLib;
using Talent.ClientCommonLib.Controls;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.RemoteCarMeasure.View;
using Talent.Video.Controller;
using Talent.CommonMethod;
using System.Net;
using Talent.RemoteCarMeasure.Commom;
using System.Windows.Media;
using Talent.RemoteCarMeasure.Properties;
using Talent.RemoteCarMeasure.Commom.Control;
using Talent_LT.HelpClass;
using Talent.Measure.WPF;
using Talent.Measure.WPF.Log;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Talent.RemoteCarMeasure.ViewModel
{
    public class RemoteCarMeasureViewModel : Only_ViewModelBase
    {
        #region 属性

        /// <summary>
        /// 任务结束时本机的随机数
        /// </summary>
        private int endTaskRandomNum = 0;
        /// <summary>
        /// 任务服务器反馈的任务结束随机数
        /// </summary>
        private int taskServerRandomNum = 0;

        /// <summary>
        /// 坐席关注的称点ID集合
        /// </summary>
        private string ids = string.Empty;

        private string clientStateInfo;
        /// <summary>
        /// 坐席客户端状态信息
        /// </summary>
        public string ClientStateInfo
        {
            get { return clientStateInfo; }
            set
            {
                clientStateInfo = value;
                this.RaisePropertyChanged("ClientStateInfo");
            }
        }

        private bool taskServerConnState = false;
        /// <summary>
        /// 与任务服务器连接状态
        /// </summary>
        public bool TaskServerConnState
        {
            get { return taskServerConnState; }
            set
            {
                taskServerConnState = value;
                this.RaisePropertyChanged("TaskServerConnState");
            }
        }

        private bool isControlEnable;
        /// <summary>
        /// 控件是否可用(用于正常计量和暂停计量)
        /// </summary>
        public bool IsControlEnable
        {
            get { return isControlEnable; }
            set
            {
                isControlEnable = value;
                this.RaisePropertyChanged("IsControlEnable");
            }
        }

        private string pauseButtonText = "暂停计量";
        /// <summary>
        /// 暂停按钮显示信息(暂停计量/继续计量)
        /// </summary>
        public string PauseButtonText
        {
            get { return pauseButtonText; }
            set
            {
                pauseButtonText = value;
                this.RaisePropertyChanged("PauseButtonText");
            }
        }

        private List<WeighterClientModel> carWeighterClientInfos;
        /// <summary>
        /// 汽车衡客户端集合
        /// </summary>
        public List<WeighterClientModel> CarWeighterClientInfos
        {
            get { return carWeighterClientInfos; }
            set
            {
                carWeighterClientInfos = value;
                this.RaisePropertyChanged("CarWeighterClientInfos");
            }
        }

        private ObservableCollection<TaskModel> taskInfos;
        /// <summary>
        /// 任务集合
        /// </summary>
        public ObservableCollection<TaskModel> TaskInfos
        {
            get { return taskInfos; }
            set
            {
                taskInfos = value;
                this.RaisePropertyChanged("TaskInfos");
            }
        }

        private TaskModel _selectTaskInfo;
        /// <summary>
        /// 任务列表中选中的任务(用于抢任务)
        /// </summary>
        public TaskModel selectTaskInfo
        {
            get { return _selectTaskInfo; }
            set
            {
                _selectTaskInfo = value;
                this.RaisePropertyChanged("selectTaskInfo");
            }
        }

        private TaskModel selectedTask;
        /// <summary>
        /// 选择的任务
        /// </summary>
        public TaskModel SelectedTask
        {
            get { return selectedTask; }
            set
            {
                selectedTask = value;
                this.RaisePropertyChanged("SelectedTask");
            }
        }

        private bool isBellMove;
        /// <summary>
        /// 铃铛图标是否开始移动(界面中的动画)
        /// </summary>
        public bool IsBellMove
        {
            get { return isBellMove; }
            set
            {
                isBellMove = value;
                this.RaisePropertyChanged("IsBellMove");
            }
        }

        private int taskCount;
        /// <summary>
        /// 任务数量
        /// </summary>
        public int TaskCount
        {
            get { return taskCount; }
            set
            {
                taskCount = value;
                this.RaisePropertyChanged("TaskCount");
            }
        }

        private Random rd = new Random();

        /// <summary>
        /// 任务服务器IP
        /// </summary>
        private string getTaskIp;
        /// <summary>
        /// 任务服务器端口
        /// </summary>
        private int getTaskPort;
        /// <summary>
        /// 事件名称集合
        /// </summary>
        private List<string> eventNameList;

        private string sysTime;
        /// <summary>
        /// 系统时间
        /// </summary>
        public string SysTime
        {
            get { return sysTime; }
            set
            {
                sysTime = value;
                this.RaisePropertyChanged("SysTime");
            }
        }

        private int acceptTaskTime;
        /// <summary>
        /// 接收任务的时间(秒)
        /// </summary>
        public int AcceptTaskTime
        {
            get { return acceptTaskTime; }
            set
            {
                acceptTaskTime = value;
                this.RaisePropertyChanged("AcceptTaskTime");
            }
        }

        /// <summary>
        /// 时间计时，用于界面中时间的跳动
        /// </summary>
        private System.Windows.Forms.Timer sysTimer;

        /// <summary>
        /// 内存释放计数(秒)
        /// </summary>
        private int fulshMemeryCount = 0;

        /// <summary>
        /// 选择的称点终端
        /// </summary>
        private WeighterClientModel selectedWeighterClient;

        private bool isFormEnable = true;
        /// <summary>
        /// 窗体是否可用
        /// </summary>
        public bool IsFormEnable
        {
            get { return isFormEnable; }
            set
            {
                isFormEnable = value;
                this.RaisePropertyChanged("IsFormEnable");
            }
        }
        private string bColor0 = "#FF528DAD";
        /// <summary>
        /// 窗体背景色
        /// </summary>
        public string BColor0
        {
            get { return bColor0; }
            set
            {
                bColor0 = value;
                this.RaisePropertyChanged("BColor0");
            }
        }

        private string bColor1 = "#FF6DAEA6";
        /// <summary>
        /// 窗体背景色
        /// </summary>
        public string BColor1
        {
            get { return bColor1; }
            set
            {
                bColor1 = value;
                this.RaisePropertyChanged("BColor1");
            }
        }
        private int formEnableSource;
        /// <summary>
        /// 窗体是否可用来源(1:注销;2:关闭)
        /// 注:系统注销和关闭都通过设置窗体是否可用来触发cs中的事件，而
        /// 注销和关闭的业务处理又不同，故而这里设置此属性，cs文件中通过
        /// 此属性判断后续业务处理
        /// </summary>
        public int FormEnableSource
        {
            get { return formEnableSource; }
            set { formEnableSource = value; }
        }

        /// <summary>
        /// 计量方式(自动计量;手动计量)
        /// </summary>
        public string MeasureType;

        private bool isOpenVideo;
        /// <summary>
        /// 是否打开视频(用户任务处理界面中,视频是否打开)
        /// </summary>
        public bool IsOpenVideo
        {
            get { return isOpenVideo; }
            set { isOpenVideo = value; }
        }

        /// <summary>
        /// 倒计时时间
        /// </summary>
        public int TimeCount;

        /// <summary>
        /// 取数周期(坐席自动计量时,任务处理时,取实时数据的周期)
        /// </summary>
        public int WeightTimeGap;
        /// <summary>
        /// 取数次数
        /// </summary>
        public int WeightTimeCount;
        /// <summary>
        /// 是否任务正在处理中(任务处理开关)
        /// </summary>
        private bool IsHandleTasking = false;
        /// <summary>
        /// 声音播放
        /// </summary>
        private MediaPlayer mediaPlayer;//有新任务播放铃音
        /// <summary>
        /// 坐席窗体
        /// </summary>
        private MainWindow seatMainWindow;//用于接收新任务直接弹出窗体使用
        /// <summary>
        /// 判断SysTimer是否执行
        /// </summary>
        private bool isStartSysTimer = false;//用户控制提示音或者转发……
        /// <summary>
        /// 判断坐席权限
        /// </summary>
        CheckRemotePower checkRemoteP = new CheckRemotePower();
        /// <summary>
        /// 任务弹框位置
        /// </summary>
        Thickness newTaskGridThick = new Thickness(285, -179, 287, 179);

        WindowsFormHelpClass formHClass = new WindowsFormHelpClass();//窗体帮助类

        /// <summary>
        /// 等待接收任务计时器
        /// </summary>
        private Calculagraph waitTaskReusltTimer;

        /// <summary>
        /// 等待接收任务计时器可执行次数
        /// </summary>
        private int waitTaskResultCount;
        /// <summary>
        /// 等待接收任务计时器执行次数
        /// </summary>
        private int waitTaskCount;
        /// <summary>
        /// 时间计时，用于判断当前坐席状态
        /// </summary>
        private Calculagraph taskConTimer;
        #endregion

        #region 命令
        /// <summary>
        /// 任务处理命令
        /// </summary>
        public ICommand TaskProcessCommand { get; private set; }
        /// <summary>
        /// 计量命令
        /// </summary>
        public ICommand WeighterCommand { get; private set; }
        /// <summary>
        /// 视频监控
        /// </summary>
        public ICommand VideoMonitorCommand { get; private set; }
        /// <summary>
        /// 表头清零
        /// </summary>
        public ICommand MeasureWeightClearCommand { get; private set; }
        /// <summary>
        /// 终端重启
        /// </summary>
        public ICommand MeasureClientRestartCommand { get; private set; }
        /// <summary>
        /// 系统注销命令
        /// </summary>
        public ICommand SystemLoginOutCommand { get; private set; }
        /// <summary>
        /// 系统关闭命令
        /// </summary>
        public ICommand SystemCloseCommand { get; private set; }
        /// <summary>
        /// 终端自动更新命令
        /// </summary>
        public ICommand MeasureClientUpdateCommand { get; private set; }
        /// <summary>
        /// 获取任务任务（抢任务）
        /// </summary>
        public ICommand GetSelectTaskInfoCommand { get; private set; }
        /// <summary>
        /// 暂停计量
        /// </summary>
        public ICommand MeasureClientStopCommand { get; private set; }
        /// <summary>
        /// 称点全屏
        /// </summary>
        public ICommand FullScreenCommand { get; private set; }
        #endregion

        /// <summary>
        /// 日志记录
        /// </summary>
        LogsHelpClass logH = new LogsHelpClass();
        #region 构造

        public RemoteCarMeasureViewModel()
        {
            if (this.IsInDesignMode)
                return;
            IsControlEnable = true;
            TaskServerConnState = false;
            TaskProcessCommand = new ActionCommand(TaskProcess);
            WeighterCommand = new ActionCommand(WeighterMehod);
            VideoMonitorCommand = new ActionCommand(VideoMonitorMethod);
            MeasureWeightClearCommand = new ActionCommand(MeasureWeightClearMethod);
            MeasureClientRestartCommand = new ActionCommand(MeasureClientRestartMethod);
            SystemLoginOutCommand = new ActionCommand(SystemLoginOutMethod);
            SystemCloseCommand = new ActionCommand(SystemCloseMethod);
            MeasureClientUpdateCommand = new ActionCommand(MeasureClientUpdateMethod);
            // GetSelectTaskInfoCommand = new ActionCommand(GetSelectTaskInfoMethod);
            MeasureClientStopCommand = new ActionCommand(MeasureClientStopMethod);
            FullScreenCommand = new ActionCommand(MeasureClientFullScreen);
            DoBackGroundColor();//获取用户自定义默认背景色 2016-3-8 10:23:29……
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ClientConfig");
            var configPath = System.IO.Path.Combine(basePath, "SystemConfig.xml");
            #region 获取等待任务时间
            int waitTaskResultTime = 1;
            string waitTaskResultTimeItem = ConfigurationManager.AppSettings["WaitTaskResultTime"].ToString();
            string getWaitTaskResultTimeItem = XpathHelper.GetValue(configPath, waitTaskResultTimeItem);
            if (!string.IsNullOrEmpty(getWaitTaskResultTimeItem))
            {
                waitTaskResultTime = Convert.ToInt32(getWaitTaskResultTimeItem);
            }
            #endregion
            #region 获取等待任务次数
            waitTaskResultCount = 1;
            string waitTaskResultCountItem = ConfigurationManager.AppSettings["WaitTaskResultCount"].ToString();
            string getWaitTaskResultCountItem = XpathHelper.GetValue(configPath, waitTaskResultCountItem);
            if (!string.IsNullOrEmpty(getWaitTaskResultCountItem))
            {
                waitTaskResultCount = Convert.ToInt32(getWaitTaskResultCountItem);
            }
            #endregion
            #region 等待接收任务计时器
            waitTaskReusltTimer = new Calculagraph("");
            waitTaskReusltTimer.Timeout = waitTaskResultTime;
            waitTaskReusltTimer.TimeOver += new TimeoutCaller(waitTaskReusltTimer_TimeOver);
            #endregion
            #region 注册任务服务器连接的timer

            #region 获取任务服务器连接的timer间隔次数
            int defaultCount = 10;//默认10秒
            string countItem = ConfigurationManager.AppSettings["ReLoginCheckCount"].ToString();
            string countItemValue = XpathHelper.GetValue(configPath, countItem);
            if (!string.IsNullOrEmpty(countItemValue))
            {
                defaultCount = Convert.ToInt32(countItemValue);
            }
            #endregion

            taskConTimer = new Calculagraph("定时请求坐席集合");
            taskConTimer.Timeout = defaultCount;
            taskConTimer.TimeOver += new TimeoutCaller(taskConTimer_Tick);
            taskConTimer.Start();
            #endregion
        }

        /// <summary>
        /// 等待任务计时器时间到触发的事件
        /// </summary>
        /// <param name="userdata"></param>
        private void waitTaskReusltTimer_TimeOver(object userdata)
        {
            if (this.SelectedTask == null)
            {
                waitTaskCount++;
                //通过clientId调用任务服务器查询任务信息
                if (waitTaskCount <= waitTaskResultCount)
                {
                    SocketCls.Emit(SeatSendCmdEnum.getTask, taskid);
                    waitTaskReusltTimer.Start();
                    #region 写日志
                    //LogModel log = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_InOut,
                    //    FunctionName = "坐席主窗体",
                    //    Level = LogConstParam.LogLevel_Info,
                    //    Msg = "收到服务器发送的命令【newTask】,请求获取新任务。",
                    //    Origin = LoginUser.Role.Name,
                    //    OperateUserName = LoginUser.Name,
                    //    ParamList = new List<DataParam>() { new DataParam() { ParamName = "clientId", ParamValue = taskid }, new DataParam() { ParamName = "cmd", ParamValue = "getTask" } }
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                else
                {
                    waitTaskReusltTimer.Stop();
                }
            }
            else
            {
                waitTaskReusltTimer.Stop();
            }
        }
        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="SeatAttentionInfos"></param>
        public void InitForm(List<SeatAttentionWeightModel> SeatAttentionInfos, MainWindow mainWindow)
        {
            this.ShowBusy = true;
            if (SeatAttentionInfos != null && SeatAttentionInfos.Count > 0)
            {
                //汽车衡
                var carWeights = (from r in SeatAttentionInfos.OrderBy(c => c.equcode).ToList()
                                  where r.isinseat == "是"
                                      && r.seatid == LoginUser.Role.Code
                                      && r.seattype == "RC"

                                  select new WeighterClientModel()
                                  {
                                      ClientId = r.equcode,
                                      LeftLightState = LeftLightStates.None,
                                      RightLightState = RightLightStates.None,
                                      ClientState = WeighterStates.None,
                                      ClientName = r.equname,
                                      Weight = 0,
                                      ClientConfigName = r.equcode + "_" + r.equname + ".xml",
                                      LeftLine = Visibility.Hidden,
                                      RightLine = Visibility.Hidden
                                  }).ToList();
                this.CarWeighterClientInfos = carWeights;
            }
            seatMainWindow = mainWindow;
            newTaskGridThick = GetNewTaskGridThick();//获取坐标
            //seatMainWindow.handleTaskButton.KeyDown += handleTaskButton_KeyDown;
            sysTimer = new System.Windows.Forms.Timer();
            sysTimer.Interval = 1000;
            sysTimer.Tick += sysTimer_Tick;
            sysTimer.Start();
            try
            {
                Thread t1 = new Thread(new ThreadStart(initInfo));
                t1.Start();
            }
            catch (Exception ex)
            {
                this.ShowMessage("提示", "系统初始化信息失败！原因:" + ex.Message, true, false);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_窗体初始化",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "系统初始化信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            this.TaskInfos = new ObservableCollection<TaskModel>();
            this.TaskCount = 0;
            this.IsOpenVideo = true;
            ClientStateInfo = "空闲";
        }

        /// <summary>
        /// 用于判断和任务服务器连接的timer计时到触发的事件
        /// </summary>
        void taskConTimer_Tick(object userdata)
        {
            #region 日志
            //LogModel log1 = new LogModel()
            //{
            //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //    Direction = LogConstParam.Directions_In,
            //    FunctionName = "坐席主窗体_请求坐席列表",
            //    Level = LogConstParam.LogLevel_Info,
            //    Msg = "计时器计时时间到。IsHandleTasking=" + IsHandleTasking,
            //    Data = currentTaskHView,
            //    Origin = "汽车衡_" + LoginUser.Role.Name,
            //    OperateUserName = LoginUser.Name
            //};
            //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
            #endregion
            try
            {
                Console.WriteLine("发送请求list_" + DateTime.Now.ToString());
                if (!IsHandleTasking && currentTaskHView == null && TaskServerConnState)
                {
                    #region 日志
                    //LogModel log = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_In,
                    //    FunctionName = "坐席主窗体_请求坐席列表",
                    //    Level = LogConstParam.LogLevel_Info,
                    //    Msg = "发送命令【list】请求任务服务器反馈坐席列表",
                    //    Origin = "汽车衡_" + LoginUser.Role.Name,
                    //    OperateUserName = LoginUser.Name
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    SocketCls.Emit("list", "");
                }
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log2 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_请求坐席列表",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "计时器计时时间到,向服务器发送请求时异常,原因:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                #endregion
            }
            if (currentTaskHView == null)
            {
                taskConTimer.Start();
            }
            else
            {
                taskConTimer.Stop();
            }
        }

        /// <summary>
        /// 计量按钮命令对应的实现方法
        /// </summary>
        private void WeighterMehod()
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.ztjl))
            {
                this.ShowMessage("提示", "无暂停计量权限!", true, false);
                return;
            }
            if (!TaskServerConnState)
            {
                this.ShowMessage("提示", "与任务服务器断开连接!", true, false);
                return;
            }
            //logH.SaveLog("点击暂停计量");
            #region 写日志
            LogModel logSave = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "坐席主窗体_" + PauseButtonText,
                Level = LogConstParam.LogLevel_Info,
                Msg = LoginUser.Role.Name + PauseButtonText,
                Data = selectedTask,
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logSave));
            #endregion
            if (!PauseButtonText.Equals("继续计量"))
            {
                ClientStateInfo = "暂停";
                IsControlEnable = false;
                PauseButtonText = "继续计量";
                //发送命令给服务器坐席暂停了
                SocketCls.Emit(SeatSendCmdEnum.pause, "暂停计量");
                IsBellMove = false;
                #region 写日志
                //LogModel log = new LogModel()
                //{
                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    Direction = LogConstParam.Directions_In,
                //    FunctionName = "坐席主窗体_暂停计量",
                //    Level = LogConstParam.LogLevel_Info,
                //    Msg = LoginUser.Role.Name + "暂停(无分配的任务)",
                //    Data = selectedTask,
                //    Origin = "汽车衡_" + LoginUser.Role.Name,
                //    OperateUserName = LoginUser.Name
                //};
                //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                if (SelectedTask != null)
                {
                    #region 写日志
                    LogModel log5 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席主窗体_暂停",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "暂停时任务存在,AcceptTaskTime:" + AcceptTaskTime + "isStartSysTimer:" + isStartSysTimer,
                        Origin = "坐席",
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log5));
                    #endregion
                    if (AcceptTaskTime != 0 || isStartSysTimer)
                    {
                        //接任务倒计时停止，防止此窗体打开情况下，
                        //那边异步计时器计时10秒到而回退任务，而这里又选择任务处理，造成此任务被分给另外坐席，
                        //两种情况并发情况下，造成一任务多坐席处理了
                        isStartSysTimer = false;
                        ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", "存在已经分配的任务,您希望暂停后进行的操作是?", true, true, "任务处理", "回退任务");
                        confirmBox.Owner = seatMainWindow;
                        confirmBox.ShowDialog();
                        if (confirmBox.IsOk)
                        {
                            #region 写日志
                            LogModel logconfirmBox = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_Out,
                                FunctionName = "坐席主窗体_暂停接任务",
                                Level = LogConstParam.LogLevel_Info,
                                Msg = "暂停时有任务,用户点击了【任务处理】",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                                OperateUserName = LoginUser.Name
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logconfirmBox));
                            #endregion
                            seatMainWindow.NewTaskGrid.Visibility = Visibility.Hidden;
                            IsBellMove = false;
                            //任务处理
                            this.HandleTask();
                        }
                        else
                        {
                            #region 写日志
                            LogModel logconfirmBox = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_Out,
                                FunctionName = "坐席主窗体_暂停回退",
                                Level = LogConstParam.LogLevel_Info,
                                Msg = "暂停时有任务,用户点击了【任务回退】",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                                OperateUserName = LoginUser.Name
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logconfirmBox));
                            #endregion
                            seatMainWindow.NewTaskGrid.Visibility = Visibility.Hidden;
                            if (selectedTask == null)
                            {
                                return;
                            }
                            //发送"退回任务"命令给任务服务器
                            SocketCls.Emit(SeatSendCmdEnum.backTask, SelectedTask.ClientId);
                            #region 写日志
                            LogModel log1 = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_Out,
                                FunctionName = "坐席主窗体",
                                Level = LogConstParam.LogLevel_Info,
                                Msg = LoginUser.Role.Name + "暂停并退回已经分配的任务。",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                                OperateUserName = LoginUser.Name,
                                Data = SelectedTask,
                                IsDataValid = LogConstParam.DataValid_Ok,
                                ParamList = new List<DataParam>() { new DataParam() { ParamName = "clientid", ParamValue = SelectedTask.ClientId } }
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                            #endregion
                            SelectedTask = null;
                            IsBellMove = false;
                        }
                    }
                    else
                    {
                        seatMainWindow.NewTaskGrid.Visibility = Visibility.Hidden;
                        if (selectedTask == null)
                        {
                            return;
                        }
                        //发送"退回任务"命令给任务服务器
                        SocketCls.Emit(SeatSendCmdEnum.backTask, SelectedTask.ClientId);
                        SelectedTask = null;
                        IsBellMove = false;
                        #region 写日志
                        LogModel logr = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            FunctionName = "坐席主窗体_暂停计量",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = LoginUser.Role.Name + "点击了暂停计量,AcceptTaskTime:" + AcceptTaskTime + ",任务自动回退",
                            Origin = "坐席",
                            OperateUserName = LoginUser.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logr));
                        #endregion
                    }
                }
            }
            else
            {
                if (this.SelectedTask != null)
                {
                    this.ShowMessage("提示", "请先处理完当前的任务!", true, false);
                }
                else
                {
                    ClientStateInfo = "空闲";
                    IsControlEnable = true;
                    PauseButtonText = "暂停计量";
                    //发送命令给服务器坐席继续计量了
                    SocketCls.Emit(SeatSendCmdEnum.resume, "正常使用中......");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席主窗体_" + "继续计量",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = LoginUser.Role.Name + "恢复接收任务",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }

        private void initInfo()
        {
            try
            {
                string configSet = ConfigurationManager.AppSettings["SysConfigFileName"].ToString();
                string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
                string configUrl = basePath + configSet;

                #region 注册Socket
                initSocket(configUrl);
                #endregion
            }
            catch (Exception ex)
            {
                TaskServerConnState = false;
                this.ShowMessage("提示", "系统初始化信息失败！原因:" + ex.Message, true, false);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_注册Socket",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "注册Socket失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            this.ShowBusy = false;
        }

        /// <summary>
        /// 获取进程使用的内存大小
        /// </summary>
        /// <returns></returns>
        public long GetMemoryAmount()
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
        /// 计时触发(改变窗体中时间属性)
        /// </summary>
        void sysTimer_Tick(object sender, EventArgs e)
        {
            this.SysTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                if (fulshMemeryCount >= 5)
                {
                    FlushMemory();
                    fulshMemeryCount = 0;
                }
                else
                {
                    fulshMemeryCount++;
                }
            }
            catch (Exception ex)
            {
                #region 异常日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_定时释放内存",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "定时释放内存时异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
            }
            if (!isStartSysTimer)
            {
                return;
            }
            if (this.SelectedTask != null)
            {
                AcceptTaskTime = AcceptTaskTime + 1;
            }
            else
            {
                AcceptTaskTime = 0;
            }
            //设置提示音……
            if (AcceptTaskTime == 1)
            {
                mediaPlayer = new MediaPlayer();
                //播放语音的控件 初始化……lt 2016-2-15 13:03:24…… 初始化一次 偶尔会不播放声音
                mediaPlayer.Volume = 1;
                mediaPlayer.Open(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\RING.WAV"));
                mediaPlayer.Play();
                //弹出窗体，提示计量员处理任务……lt 2016-2-15 15:20:29……
                //seatMainWindow.WindowState = WindowState.Maximized;
                //seatMainWindow.Activate();
                seatMainWindow.NewTaskGrid.Visibility = Visibility.Visible;
                seatMainWindow.handleTaskButton.Focus();
                //seatMainWindow.handleTaskButton.KeyDown += handleTaskButton_KeyDown;
                seatMainWindow.KeyDown -= handleTaskButton_KeyDown;
                seatMainWindow.KeyDown += handleTaskButton_KeyDown;
                //seatMainWindow.Topmost = true;                
                seatMainWindow.NewTaskGrid.Margin = GetNewTaskGridThick();
            }
            else if (AcceptTaskTime == 9)//铃音提示正好8秒……
            {
                mediaPlayer.Stop();
                mediaPlayer = null;
            }
            else if (AcceptTaskTime == 10)//等于10 自动转发即回退任务…… lt 2016-2-15 13:06:44……
            {
                SaveDoResult();
                seatMainWindow.Topmost = false;
                seatMainWindow.NewTaskGrid.Visibility = Visibility.Hidden;
                AutoBackTask();
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席主窗体",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "等待10秒依然没接任务自动回退任务给任务服务器",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "backTask", ParamValue = SeatSendCmdEnum.backTask }, new DataParam() { ParamName = "clientId", ParamValue = SelectedTask.ClientId } },
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                isStartSysTimer = false;
                AcceptTaskTime = 0;
                IsBellMove = false;
                if (!PauseButtonText.Equals("继续计量"))
                {
                    ClientStateInfo = "空闲";
                }
                else
                {
                    IsControlEnable = false;
                }
            }
        }

        /// <summary>
        /// 任务自动回退
        /// </summary>
        private async void AutoBackTask()
        {
            await System.Threading.Tasks.Task.Delay(3000);
            if (SelectedTask != null && currentTaskHView == null)
            {
                try
                {
                    //发送"回收任务"命令给任务服务器
                    SocketCls.Emit(SeatSendCmdEnum.backTask, SelectedTask.ClientId);
                    SelectedTask = null;
                }
                catch (Exception ex)
                {
                    #region 日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席主窗体_任务自动回退",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "任务自动回退异常,原因：" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }

        #endregion

        #region 方法

        #region Socket初始化
        private void initSocket(string configUrl)
        {
            #region 读取任务服务器IP
            string TaskIpItem = ConfigurationManager.AppSettings["TaskIpInfo"].ToString();
            getTaskIp = XpathHelper.GetValue(configUrl, TaskIpItem);
            #endregion

            #region 读取端口
            string SeatTaskItem = ConfigurationManager.AppSettings["TaskPortInfo"].ToString();
            getTaskPort = CommonTranslationHelper.ToInt(XpathHelper.GetValue(configUrl, SeatTaskItem));
            #endregion

            #region 读取计量方式
            string measureType = ConfigurationManager.AppSettings["MeasureType"].ToString();
            this.MeasureType = XpathHelper.GetValue(configUrl, measureType);
            #endregion

            #region 自动确认倒计时时间
            string confirmTime = ConfigurationManager.AppSettings["ConfirmTime"].ToString();
            string confirmTimeItem = XpathHelper.GetValue(configUrl, confirmTime);
            if (!string.IsNullOrEmpty(confirmTimeItem))
            {
                this.TimeCount = Int32.Parse(confirmTimeItem);
            }
            #endregion

            #region 取数周期时间
            string weightTime = ConfigurationManager.AppSettings["WeightTimePeriod"].ToString();
            string weightTimeStr = XpathHelper.GetValue(configUrl, weightTime);
            if (!string.IsNullOrEmpty(weightTimeStr))
            {
                this.WeightTimeGap = Int32.Parse(weightTimeStr);
            }
            #endregion

            #region 取数次数
            string weightTimeCount = ConfigurationManager.AppSettings["WeightTimeCount"].ToString();
            string weightTimeCountStr = XpathHelper.GetValue(configUrl, weightTimeCount);
            if (!string.IsNullOrEmpty(weightTimeCountStr))
            {
                this.WeightTimeCount = Int32.Parse(weightTimeCountStr);
            }
            #endregion

            #region 注册事件
            try
            {
                eventNameList = new List<string>();
                //如果有接收任务权限 则注册这些事件……lt 2016-2-19 14:38:22……
                if (checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.jsrw))
                {
                    eventNameList.Add(SeatlistenCmdEnum.newTask);
                    eventNameList.Add(SeatlistenCmdEnum.Task);
                    eventNameList.Add(SeatlistenCmdEnum.waitingTask);
                    eventNameList.Add(SeatlistenCmdEnum.redirectTask);
                    eventNameList.Add(SeatlistenCmdEnum.reply);
                }
                eventNameList.Add(SeatlistenCmdEnum.Txt);
                eventNameList.Add("listrtn");
                eventNameList.Add(SeatlistenCmdEnum.logok);
                eventNameList.Add(SeatlistenCmdEnum.realData);
                eventNameList.Add(ClientSendCmdEnum.logout);
                eventNameList.Add(ClientlistenCmdEnum.relogin);
                eventNameList.Add(ClientlistenCmdEnum.reconn);
                eventNameList.Add(ClientlistenCmdEnum.sendCMD);
                eventNameList.Add("reply");
                SocketCls.ConnectServer(getTaskIp, getTaskPort, eventNameList, true);
                SocketCls.listenEvent += SocketCls_listenEvent;
                SocketCls.scoketError += SocketCls_scoketError;
                SocketCls.scoketClose += SocketCls_scoketClose;
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_Socket初始化",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "注册Socket事件！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// Socket关闭
        /// </summary>
        void SocketCls_scoketClose(object sender, EventArgs e)
        {
            TaskServerConnState = false;
        }

        /// <summary>
        /// Socket异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SocketCls_scoketError(object sender, EventArgs e)
        {
            TaskServerConnState = false;
        }
        #endregion

        /// <summary>
        /// 登陆任务服务器
        /// </summary>
        private void LoginTaskServer()
        {
            ids = string.Empty;
            var clientids = (from r in this.CarWeighterClientInfos select r.ClientId).ToList();
            foreach (var id in clientids)
            {
                ids = ids + id + ",";
            }
            if (!string.IsNullOrEmpty(ids) && ids.Length > 0)
            {
                ids = ids.Substring(0, ids.Length - 1);
            }
            var pram = new
            {
                seatcode = LoginUser.Role.Code,
                name = LoginUser.Role.Name,
                filter = ids,
                status = PauseButtonText.Equals("继续计量") ? "暂停服务" : "空闲"
            };
            string pramJsonStr = InfoExchange.ToJson(pram);
            SocketCls.Emit(SeatSendCmdEnum.login, pramJsonStr);
        }

        /// <summary>
        /// 任务处理
        /// </summary>
        public void TaskProcess()
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.jsrw))
            {
                return;
            }
            if (this.SelectedTask != null)
            {
                //logH.SaveLog("接收任务：衡器名称：" + this.SelectedTask.ClientName + "  车号：" + this.SelectedTask.ServiceModel.data.carNo);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_任务处理",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "接收任务：衡器名称：" + this.SelectedTask.ClientName + "  车号：" + this.SelectedTask.ServiceModel.data.carNo,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                try
                {
                    if (mediaPlayer != null)
                    {
                        mediaPlayer.Stop();//停止提示音播放……
                        mediaPlayer = null;
                    }
                }
                catch (Exception ex)
                {
                    #region 日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席主窗体_停止提示音播放",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "停止提示音播放异常,原因：" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                }
                seatMainWindow.KeyDown -= handleTaskButton_KeyDown;
                seatMainWindow.NewTaskGrid.Visibility = Visibility.Hidden;
                IsBellMove = false;
                isStartSysTimer = false;//是否启动任务提示铃音或者自动回退任务……
                AcceptTaskTime = 0;//任务等待时间变为0
                seatMainWindow.Topmost = false;
                DoClientState();//处理衡器显示状态……lt 2016-2-18 16:50:10…… 通知衡器，正在计量
                //ShowMessage("提示", "确定接收新任务?\r\n\r\n【确定】:接收新任务。\r\n\r\n【取消】:回退任务。", true, true, "AcceptTask");
                //取消弹出提示框，直接打开任务处理界面…… lt 2016-2-15 13:15:08……
                //this.ProcessTaskStartTime = DateTime.Now;
                HandleTask();
            }

        }

        /// <summary>
        /// 是否第一次让铃铛动画动起来(铃铛动画在计时器触发时的开关，临时使用)
        /// </summary>
        //private bool isFirstOpenBell = true;

        /// <summary>
        /// 视频监控方法
        /// </summary>
        /// <param name="obj"></param>
        private void VideoMonitorMethod(object obj)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.spjk))
            {
                this.ShowMessage("提示", "无视频监控查看权限!", true, false);
                return;
            }
            WeighterClientModel viedoSelectedWeighterClient = obj as WeighterClientModel;
            //bool isOpen = formHClass.SetForeWindow("智能化远程集中计量管理系统(视频监控)");
            //if (!isOpen)
            //{
            //    logH.SaveLog("点击视频监控打开新窗体:" + viedoSelectedWeighterClient.ClientName);
            //    VideoMonitorView view = new VideoMonitorView();
            //    (view.DataContext as VideoMonitorViewModel).CurClientModel = obj as WeighterClientModel;
            //    view.Show();

            //}
            //else
            //{
            //    logH.SaveLog("点击视频监控呼出之前窗体");
            //}
            VideoMonitorView view = new VideoMonitorView();
            (view.DataContext as VideoMonitorViewModel).CurClientModel = obj as WeighterClientModel;
            view.ShowDialog();
        }

        /// <summary>
        /// 称点全屏
        /// </summary>
        /// <param name="obj"></param>
        private void MeasureClientFullScreen(object obj)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.cdqp))
            {
                this.ShowMessage("提示", "无称点全屏权限!", true, false);
                return;
            }
            selectedWeighterClient = obj as WeighterClientModel;
            if (selectedWeighterClient.ClientState == WeighterStates.None)
            {
                //logH.SaveLog("点击称点全屏，系统提示：衡器端断开连接,不可全屏");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_称点_称点全屏",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击称点全屏，系统提示：衡器端断开连接,不可全屏",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.ShowMessage("提示", "衡器端断开连接,不可全屏。", true, false);
            }
            else
            {
                int unm = CommonMethod.CommonMethod.GetRandom();
                var para = new
                {
                    clientid = selectedWeighterClient.ClientId,
                    cmd = ParamCmd.ClientFullScreen,
                    msg = "称点全屏",
                    msgid = unm
                };
                string paraJsonStr = JsonConvert.SerializeObject(para);
                SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
                #region 写日志
                //LogModel log = new LogModel()
                //{
                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    Direction = LogConstParam.Directions_Out,
                //    FunctionName = "坐席主窗体",
                //    Level = LogConstParam.LogLevel_Info,
                //    Msg = selectedWeighterClient.ClientName + "表头清零",
                //    Origin = LoginUser.Role.Name,
                //    Data = para,
                //    IsDataValid = LogConstParam.DataValid_Ok,
                //    ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                //    OperateUserName = LoginUser.Name
                //};
                //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 表头清零
        /// </summary>
        /// <param name="obj">称点对象</param>
        private void MeasureWeightClearMethod(object obj)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.btql))
            {
                this.ShowMessage("提示", "无表头清零权限!", true, false);
                return;
            }
            selectedWeighterClient = obj as WeighterClientModel;
            if (selectedWeighterClient.ClientState == WeighterStates.Working)
            {
                //logH.SaveLog("点击表头清零，系统提示：衡器端正在计量,不可清零");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_称点_表头清零",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击表头清零，系统提示：衡器端正在计量,不可清零",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.ShowMessage("提示", "衡器端正在计量,不可清零。", true, false);
            }
            else if (selectedWeighterClient.ClientState == WeighterStates.None)
            {
                //logH.SaveLog("点击表头清零，系统提示：衡器端断开连接,不可清零");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_称点_表头清零",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击表头清零，系统提示：衡器端断开连接,不可清零",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.ShowMessage("提示", "衡器端断开连接,不可清零。", true, false);
            }
            else
            {
                this.ShowMessage("提示", "确定将表头清零?", true, true, "MeasureWeightClear");
            }
        }

        /// <summary>
        /// 终端重启
        /// </summary>
        /// <param name="obj">称点对象</param>
        private void MeasureClientRestartMethod(object obj)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.zdcq))
            {
                this.ShowMessage("提示", "无终端重启权限!", true, false);
                return;
            }
            selectedWeighterClient = obj as WeighterClientModel;
            if (selectedWeighterClient.ClientState == WeighterStates.Working)
            {
                //logH.SaveLog("点击终端重启，系统提示：衡器端正在计量,不可重新启动");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_称点_终端重启",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击终端重启，系统提示：衡器端正在计量,不可重新启动",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.ShowMessage("提示", "衡器端正在计量,不可重新启动。", true, false);
            }
            else if (selectedWeighterClient.ClientState == WeighterStates.None)
            {
                //logH.SaveLog("点击终端重启，系统提示：衡器端断开连接,不可重新启动");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_称点_终端重启",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击终端重启，系统提示：衡器端断开连接,不可重新启动",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.ShowMessage("提示", "衡器端断开连接,不可重新启动。", true, false);
            }
            else
            {
                this.ShowMessage("提示", "确定重新启动终端?", true, true, "MeasureClientRestart");
            }
        }

        /// <summary>
        /// 称点自动更新方法
        /// </summary>
        private void MeasureClientUpdateMethod(object obj)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.bbgx))
            {
                this.ShowMessage("提示", "无终端更新版本权限!", true, false);
                return;
            }
            selectedWeighterClient = obj as WeighterClientModel;
            if (selectedWeighterClient.ClientState == WeighterStates.Working)
            {
                this.ShowMessage("提示", "衡器端正在计量,不可自动更新。", true, false);
            }
            else if (selectedWeighterClient.ClientState == WeighterStates.None)
            {
                this.ShowMessage("提示", "衡器端断开连接,不可自动更新。", true, false);
            }
            else
            {
                this.ShowMessage("提示", "确定自动更新该终端?", true, true, "MeasureClientUpdate");
            }
        }
        /// <summary>
        /// 抢任务
        /// </summary>
        /// <param name="obj"></param>
        private void GetSelectTaskInfoMethod(object obj)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.qrw))
            {
                this.ShowMessage("提示", "无抢任务权限!", true, false);
                return;
            }
            if (selectTaskInfo != null)
            {
                if (!PauseButtonText.Equals("继续计量"))
                {
                    this.ShowMessage("提示", "请先暂停计量!", true, false);
                    return;
                }
                if (SelectedTask != null)
                {
                    this.ShowMessage("提示", "请先处理已经分配的任务!", true, false);
                    return;
                }
                SocketCls.Emit(SeatSendCmdEnum.getTask2, selectTaskInfo.ClientId);
                IsControlEnable = true;
            }
            else
            {
                this.ShowMessage("提示", "请选择等待的任务!", true, false);
            }
        }

        /// <summary>
        /// 终端暂停计量
        /// </summary>
        /// <param name="obj">称点对象</param>
        private void MeasureClientStopMethod(object obj)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.zdztjl))
            {
                this.ShowMessage("提示", "无停用终端权限!", true, false);
                return;
            }
            selectedWeighterClient = obj as WeighterClientModel;
            if (selectedWeighterClient.ClientState == WeighterStates.Working)
            {
                //logH.SaveLog("点击终端暂停计量，系统提示：计量终端正在计量,不可停用");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_称点_终端暂停计量",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击终端暂停计量，系统提示：计量终端正在计量,不可停用",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.ShowMessage("提示", "计量终端正在计量,不可停用。", true, false);
            }
            else if (selectedWeighterClient.ClientState == WeighterStates.None)
            {
                //logH.SaveLog("点击终端暂停计量，系统提示：衡器端已断开,无法停用");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_称点_终端暂停计量",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击终端暂停计量，系统提示：衡器端已断开,无法停用",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.ShowMessage("提示", "衡器端已断开,无法停用。", true, false);
            }
            else
            {
                this.ShowMessage("提示", "确定停用计量终端吗？", true, true, "MeasureClientStop");
            }
        }

        /// <summary>
        /// 系统注销方法
        /// </summary>
        private void SystemLoginOutMethod()
        {
            this.ShowMessage("提示", "确定注销系统?", true, true, "LogOutSystem");
        }

        /// <summary>
        /// 系统关闭方法
        /// </summary>
        /// <param name="obj"></param>
        private void SystemCloseMethod()
        {
            this.ShowMessage("提示", "确定关闭系统?", true, true, "CloseSystem");
        }

        #endregion

        #region 事件
        string taskid = "";
        #region Socket回调事件
        void SocketCls_listenEvent(object sender, CallBackEventArgs e)
        {
            #region 写日志
            //if (!e.EventName.Equals("realData"))
            //{
            // Console.WriteLine("得到任务服务器命令:" + e.EventName + "__" + e.Message);
            //string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            //string url = basePath + @"/Log/SysLog/11111.txt";
            //FileHelper.AppendText(url,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"【得到任务服务器命令:" + e.EventName + "__" + e.Message);

            //LogModel log = new LogModel()
            //{
            //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //    Direction = LogConstParam.Directions_InOut,
            //    FunctionName = "坐席主窗体",
            //    Level = LogConstParam.LogLevel_Info,
            //    Msg = "【【【【【【得到任务服务器命令:" + e.EventName + "__" + e.Message,
            //    Origin = LoginUser.Role.Name,
            //    OperateUserName = LoginUser.Name
            //};
            //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            //}
            #endregion
            //Console.WriteLine("得到命令:" + e.EventName + "__" + e.Message + "_" + DateTime.Now.ToString());
            switch (e.EventName)
            {
                case "newTask"://有新任务,得到客户端ID
                    GetNewTask(e.Message);
                    break;
                case "txt"://接收提示信息
                    break;
                case "task"://获取任务
                    GetTask(e.Message);
                    break;
                case "relogin"://重新登陆
                    relogin();
                    break;
                case "reconn"://重新连接
                    reconn();
                    break;
                case "loginok"://登陆成功
                    loginok(e.Message);
                    break;
                case "logout":
                    LoginOut();
                    break;
                case "waitingTask"://获取等待的任务集合
                    GetWaitingTaskList(e.Message);
                    break;
                case "realData":
                    GetRealData(e.Message);
                    break;
                case "reply":
                    GetReplyData(e.Message);
                    break;
                case "listrtn":
                    GetSeatStates(e.Message);
                    break;
                case "reply2":
                    GetEndTaskRandomNum(e.Message);
                    break;
            }
        }

        /// <summary>
        /// 获取任务服务器反馈的任务结束时的随机数
        /// </summary>
        /// <param name="randomNum">随机数</param>
        private void GetEndTaskRandomNum(string randomNum)
        {
            int.TryParse(randomNum, out taskServerRandomNum);
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_任务主窗体_反馈任务结束随机数",
                Level = LogConstParam.LogLevel_Info,
                Msg = "收到任务服务器反馈的任务结束随机数:" + taskServerRandomNum + "_本地任务结束随机数:" + endTaskRandomNum,
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        #region 任务服务器返回信息

        /// <summary>
        /// 获取坐席的状态
        /// </summary>
        /// <param name="msg"></param>
        private void GetSeatStates(string msg)
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_InOut,
                FunctionName = "坐席主窗体_listrtn",
                Level = LogConstParam.LogLevel_Info,
                Msg = msg,
                Origin = LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            if (!string.IsNullOrEmpty(msg))
            {
                var seats = msg.Split(';');
                if (seats != null && seats.Count() > 0)
                {
                    var curSeats = seats.Where(r => r.Contains(LoginUser.Role.Code)).ToList();
                    if (curSeats != null && curSeats.Count > 0)
                    {
                        if (ClientStateInfo == "空闲")
                        {
                            if (curSeats.First().Contains("暂停服务"))
                            {
                                #region 写日志
                                LogModel log2 = new LogModel()
                                {
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Direction = LogConstParam.Directions_InOut,
                                    FunctionName = "坐席主窗体_listrtn",
                                    Level = LogConstParam.LogLevel_Info,
                                    Msg = "后台发送恢复计量命令给任务服务器",
                                    Data = msg,
                                    Origin = LoginUser.Role.Name,
                                    OperateUserName = LoginUser.Name
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                                #endregion
                                //发送命令给服务器坐席继续计量了
                                SocketCls.Emit(SeatSendCmdEnum.resume, "正常使用中......");
                            }
                        }
                        else if (ClientStateInfo == "暂停")
                        {
                            if (!curSeats.First().Contains("暂停服务"))
                            {
                                #region 写日志
                                LogModel log2 = new LogModel()
                                {
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Direction = LogConstParam.Directions_InOut,
                                    FunctionName = "坐席主窗体_listrtn",
                                    Level = LogConstParam.LogLevel_Info,
                                    Msg = "后台发送暂停计量命令给任务服务器",
                                    Data = msg,
                                    Origin = LoginUser.Role.Name,
                                    OperateUserName = LoginUser.Name
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                                #endregion
                                //发送命令给服务器坐席暂停计量了
                                SocketCls.Emit(SeatSendCmdEnum.pause, "暂停计量");
                            }
                        }
                        //if (curSeats.First().Contains("暂停服务"))//服务端是暂停
                        //{
                        //    #region 写日志
                        //    LogModel log1 = new LogModel()
                        //    {
                        //        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        //        Direction = LogConstParam.Directions_InOut,
                        //        FunctionName = "坐席主窗体_listrtn",
                        //        Level = LogConstParam.LogLevel_Info,
                        //        Msg = "检测到任务服务器中,本坐席为暂停服务状态",
                        //        Data = msg,
                        //        Origin = LoginUser.Role.Name,
                        //        OperateUserName = LoginUser.Name
                        //    };
                        //    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                        //    #endregion
                        //    if (PauseButtonText.Equals("暂停计量") && currentTaskHView == null)//当前是空闲
                        //    {
                        //        //发送命令给服务器坐席继续计量了
                        //        SocketCls.Emit(SeatSendCmdEnum.resume, "正常使用中......");
                        //    }
                        //}
                    }
                    else//无当前坐席信息，则重新登录任务服务器
                    {
                        LoginTaskServer();
                    }
                }
            }
        }

        /// <summary>
        /// 获取任务服务器传来的任务
        /// </summary>
        /// <param name="obj"></param>
        private void GetTask(string taskJsonStr)
        {
            try
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_InOut,
                    FunctionName = "坐席主窗体_GetTask",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "收到服务器发送的新任务。",
                    Origin = LoginUser.Role.Name,
                    Data = taskJsonStr,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //如果任务不为空
                if (this.SelectedTask != null)
                {
                    this.waitTaskReusltTimer.Stop();
                    return;
                }
                if (!string.IsNullOrEmpty(taskJsonStr) && taskJsonStr.Length > 0)
                {
                    this.waitTaskReusltTimer.Stop();
                }
                TaskModel task = JsonConvert.DeserializeObject<TaskModel>(taskJsonStr);
                if (task != null && task.ServiceModel == null)//卡异常任务时，ServiceModel为null会有问题。
                {
                    task.ServiceModel = new MeasureServiceModel()
                    {
                        data = new BullData(),
                        flags = new List<flagMsg>(),
                        hardwarectrl = new List<hardwarectrlCls>(),
                        mores = new List<RenderUI>(),
                        rows = new List<BullInfo>(),
                        success = true,
                        total = 0
                    };
                }
                if (!this.IsHandleTasking)
                {
                    taskConTimer.Stop();
                    this.SelectedTask = InfoExchange.Clone<TaskModel>(task);
                    this.SelectedTask.BullInfos = task.ServiceModel.rows;
                    IsBellMove = true;
                    AcceptTaskTime = 0;
                    //新任务之后启动timer…… lt 2016-2-15 13:58:34……
                    isStartSysTimer = true;
                    if (!PauseButtonText.Equals("继续计量"))
                    {
                        ClientStateInfo = "等待接受新任务...";
                    }
                    if (this.MeasureType.Equals("自动计量"))
                    {
                        var task1 = System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => { HandleTask(); }));
                        task1.Completed += new EventHandler(task_Completed);
                    }
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log5 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席主窗体_GetTask",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "拿到新任务后处理时异常:" + ex.Message,
                    Data = this.SelectedTask,
                    Origin = "坐席",
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log5));
                #endregion
            }
        }
        void task_Completed(object sender, EventArgs e)
        {
            string st = string.Empty;
            //UpLoadHandleTaskInfo();
        }

        /// <summary>
        /// 任务处理完成后
        /// </summary>
        private void AfterTaskHandle()
        {
            this.SelectedTask = null;
            if (!PauseButtonText.Equals("继续计量"))
            {
                ClientStateInfo = "空闲";
            }
            else
            {
                ClientStateInfo = "暂停";
                IsControlEnable = false;
            }
            this.IsHandleTasking = false;
            taskConTimer.Start();
        }

        /// <summary>
        /// 获取新任务
        /// </summary>
        /// <param name="clientid"></param>
        private void GetNewTask(string clientId)
        {
            try
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_InOut,
                    FunctionName = "坐席主窗体_GetNewTask",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "收到服务器发送的新任务称点ID:" + clientId + ",IsHandleTasking:" + this.IsHandleTasking,
                    Origin = LoginUser.Role.Name,
                    Data = clientId,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                taskServerRandomNum = endTaskRandomNum;
                waitTaskCount = 0;
                //通过clientId调用任务服务器查询任务信息
                taskid = clientId;
                SocketCls.Emit(SeatSendCmdEnum.getTask, clientId);
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_InOut,
                    FunctionName = "坐席主窗体_GetNewTask",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "向任务服务器发送getTask，请求获取称点" + clientId + "的任务",
                    Origin = LoginUser.Role.Name,
                    Data = clientId,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
                if (!this.IsHandleTasking)
                {
                    waitTaskReusltTimer.Start();
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_InOut,
                    FunctionName = "坐席主窗体",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "处理GetNewTask时异常,原因:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 重新登录
        /// </summary>
        public void relogin()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    //if (currentTaskHView == null)
                                    //{
                                    LoginTaskServer();
                                    #region 写日志
                                    LogModel log = new LogModel()
                                    {
                                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        Direction = LogConstParam.Directions_InOut,
                                        FunctionName = "坐席主窗体_重新登录",
                                        Level = LogConstParam.LogLevel_Info,
                                        Msg = "收到任务服务器命令【relogin】,请求重新登录任务服务器。",
                                        Origin = "汽车衡_" + LoginUser.Role.Name,
                                        OperateUserName = LoginUser.Name,
                                        IsDataValid = LogConstParam.DataValid_Ok,
                                        ParamList = new List<DataParam>() 
                                            { new DataParam() { ParamName = "seatcode", ParamValue = LoginUser.Role.Code }, new DataParam() { ParamName = "name", ParamValue = LoginUser.Role.Name },
                                             new DataParam() { ParamName = "filter", ParamValue = ids }}
                                    };
                                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                                    #endregion
                                    //}
                                }));

            //if (this.SelectedTask != null)
            //{
            //    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            //                    {
            //                        this.ShowMessage("提示", "与任务服务器断开,任务将自动回退!", true, false);
            //                        this.SelectedTask = null;
            //                        IsBellMove = false;
            //                    }));
            //}
        }
        /// <summary>
        /// 重新连接任务服务器
        /// </summary>
        private void reconn()
        {
            try
            {
                SocketCls.ConnectServer(getTaskIp, getTaskPort, eventNameList, true);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_InOut,
                    FunctionName = "坐席主窗体_重新连接任务服务器",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "收到任务服务器命令【reconn】,重新与任务服务器建立连接。",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "Ip", ParamValue =getTaskIp }, new DataParam() { ParamName = "Port", ParamValue = getTaskPort.ToString() },
                    new DataParam() { ParamName = "eventNameList", ParamValue = eventNameList.ToString() }}
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<string>("重新连接任务服务器失败,原因：" + ex.Message, "RemoteCarMeasureViewModel");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席主窗体_重新连接任务服务器",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "重新连接任务服务器失败,原因：" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "Ip", ParamValue =getTaskIp }, new DataParam() { ParamName = "Port", ParamValue = getTaskPort.ToString() },
                    new DataParam() { ParamName = "eventNameList", ParamValue = eventNameList.ToString() }}
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 登录成功
        /// </summary>
        private void loginok(string info)
        {
            if (info.Equals("login_fail"))
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席主窗体_登录",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "收到任务服务器命令【loginok】,反馈信息为:" + info + "登录失败!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            else
            {
                TaskServerConnState = true;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席主窗体_登录",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "收到任务服务器命令【loginok】,反馈信息为:" + info + "登录任务服务器成功。",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void LoginOut()
        {
            TaskServerConnState = false;
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "坐席主窗体_断开连接",
                Level = LogConstParam.LogLevel_Warning,
                Msg = "收到任务服务器命令【logout】,与任务服务器断开连接。",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        /// <summary>
        /// 抢任务后返回的信息
        /// </summary>
        /// <param name="replyDataJsonStr"></param>
        private void GetReplyData(string replyDataJsonStr)
        {
            if (replyDataJsonStr == "fail_notask")
            {
                this.ShowMessage("提示", "该任务已不存在!", true, false);
            }
            else if (replyDataJsonStr == "fail_busy")
            {
                this.ShowMessage("提示", "系统繁忙,请稍后再试!", true, false);
            }
        }
        /// <summary>
        /// 获取称点的实时数据
        /// </summary>
        /// <param name="realDataJsonStr"></param>
        private void GetRealData(string realDataJsonStr)
        {
            try
            {
                if (!string.IsNullOrEmpty(realDataJsonStr))
                {
                    if (CarWeighterClientInfos == null)
                    {
                        CarWeighterClientInfos = new List<WeighterClientModel>();
                    }
                    WeighterClientModel weighterClient = JsonConvert.DeserializeObject<WeighterClientModel>(realDataJsonStr);
                    #region 写日志
                    //LogModel log = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_Out,
                    //    FunctionName = "坐席主窗体_接收的实时数据",
                    //    Level = LogConstParam.LogLevel_Info,
                    //    Msg = "从任务服务器获取到" + weighterClient.ClientName + "的实时数据",
                    //    Origin = LoginUser.Role.Name,
                    //    Data = weighterClient
                    //};
                    //Talent.ClinetLog.IwdLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    var list = (from r in CarWeighterClientInfos where r.ClientId == weighterClient.ClientId select r).ToList();
                    if (list != null && list.Count > 0)
                    {
                        WeighterClientModel curRealDataWeight = list.First();
                        if (weighterClient.ClientState == WeighterStates.None)//称点退出情况下
                        {
                            curRealDataWeight.RightLine = Visibility.Hidden;
                            curRealDataWeight.LeftLine = Visibility.Hidden;
                            curRealDataWeight.LeftLightState = LeftLightStates.None;
                            curRealDataWeight.RightLightState = RightLightStates.None;
                            curRealDataWeight.Weight = 0M;
                            curRealDataWeight.ClientState = weighterClient.ClientState;
                            curRealDataWeight.WeightMsg = "";
                        }
                        else
                        {
                            curRealDataWeight.ClientId = weighterClient.ClientId;
                            curRealDataWeight.ClientName = weighterClient.ClientName;
                            curRealDataWeight.ClientState = weighterClient.ClientState;
                            curRealDataWeight.WeightMsg = weighterClient.WeightMsg;
                            curRealDataWeight.LeftLine = weighterClient.LeftLine;
                            curRealDataWeight.RightLine = weighterClient.RightLine;
                            switch (weighterClient.EquTag)
                            {
                                case "W":
                                    curRealDataWeight.Weight = weighterClient.Weight;
                                    break;
                                case "L":
                                    curRealDataWeight.LeftLightState = weighterClient.LeftLightState;
                                    break;
                                case "R":
                                    curRealDataWeight.RightLightState = weighterClient.RightLightState;
                                    break;
                                case "LR":
                                    curRealDataWeight.LeftLightState = weighterClient.LeftLightState;
                                    curRealDataWeight.RightLightState = weighterClient.RightLightState;
                                    break;
                            }
                        }
                    }
                    #region 写日志
                    //LogModel log5 = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_In,
                    //    FunctionName = "坐席主窗体",
                    //    Level = LogConstParam.LogLevel_Info,
                    //    Msg = "收到服务器命令【realData】,获取到" + weighterClient.ClientName + "实时数据。",
                    //    Origin = LoginUser.Role.Name,
                    //    Data = realDataJsonStr,
                    //    IsDataValid = LogConstParam.DataValid_Ok,
                    //    OperateUserName = LoginUser.Name
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log5));
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log6 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_处理获取到的实时数据异常",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "处理获取到的实时数据异常,原因:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log6));
                #endregion
            }
        }

        /// <summary>
        /// 获取等待的任务集合
        /// </summary>
        /// <param name="jsonArrayStr">等待的任务集合json字符串</param>
        private void GetWaitingTaskList(string jsonArrayStr)
        {
            if (!string.IsNullOrEmpty(jsonArrayStr))
            {
                if (jsonArrayStr.Equals("[]"))//0的话清空…… 2016-2-29 14:07:32……
                {
                    this.TaskInfos = null;
                    this.TaskCount = 0;
                    return;
                }
                try
                {
                    var str = jsonArrayStr;
                    jsonArrayStr = jsonArrayStr.Replace(@"\", "");
                    jsonArrayStr = jsonArrayStr.Replace("\"{", "{");
                    jsonArrayStr = jsonArrayStr.Replace("}\"", "}");
                    var list = InfoExchange.DeConvert(typeof(List<TaskModel>), jsonArrayStr) as List<TaskModel>;
                    //给的数据中 CarNumber为空而 data中carno有数据 要将车号显示出来 lt 2016-2-16 15:15:35……
                    for (int i = 0; i < list.Count; i++)
                    {
                        string oldCarNumber = list[i].CarNumber;
                        if (string.IsNullOrEmpty(oldCarNumber))
                        {
                            list[i].CarNumber = list[i].ServiceModel.data.carNo;
                        }
                    }
                    this.TaskInfos = new ObservableCollection<TaskModel>(list);
                    this.TaskCount = this.TaskInfos.Count;
                    #region 写日志
                    //LogModel log = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_In,
                    //    FunctionName = "坐席主窗体",
                    //    Level = LogConstParam.LogLevel_Info,
                    //    Msg = "收到服务器命令【waitingTask】,获取到等待的任务信息。",
                    //    Origin = LoginUser.Role.Name,
                    //    Data = jsonArrayStr,
                    //    IsDataValid = LogConstParam.DataValid_Ok,
                    //    OperateUserName = LoginUser.Name
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席主窗体_获取等待的任务信息",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "获取到等待的任务信息处理时异常:" + ex.Message,
                        Origin = LoginUser.Role.Name,
                        Data = jsonArrayStr,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }

        //public TaskHandleViewModel handleTaskViewModel;
        TaskHandleView currentTaskHView;
        /// <summary>
        /// 处理任务
        /// </summary>
        public void HandleTask()
        {
            if (this.SelectedTask == null)
            {
                return;
            }
            var list = (from r in this.CarWeighterClientInfos where r.ClientId == this.SelectedTask.ClientId select r).ToList();
            string configName = "";
            decimal cWeightInfos = 0;
            if (list.Count > 0)
            {
                configName = list.First().ClientConfigName;
                cWeightInfos = list.First().Weight;
            }
            try
            {
                currentTaskHView = new TaskHandleView(this.SelectedTask, configName, IsOpenVideo, this.MeasureType, this.TimeCount, this.WeightTimeGap, this.WeightTimeCount, cWeightInfos);
                //currentTaskHView.Owner = this.seatMainWindow;
                currentTaskHView.Closed += taskHandle_Closed;
                this.IsHandleTasking = true;
                ClientStateInfo = "任务处理中";
                currentTaskHView.ShowDialog();
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务主窗体_HandleTask",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "接收并准备打开任务处理窗体时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }

        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);
        /// <summary>
        /// 释放内存
        /// <returns>释放后的内存大小(M)</returns>
        /// </summary>
        public double FlushMemory()
        {
            double memorySize = GetMemoryAmount();
            try
            {
                #region 内存日志
                //LogModel log2 = new LogModel()
                //{
                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    Direction = LogConstParam.Directions_In,
                //    FunctionName = "坐席主窗体_内存释放前",
                //    Level = LogConstParam.LogLevel_Error,
                //    Msg = "内存释放前,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)",
                //    Origin = LoginUser.Role.Name,
                //    OperateUserName = LoginUser.Name,
                //};
                //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                #endregion
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
                    #region 写日志
                    //LogModel log = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_Out,
                    //    FunctionName = "坐席_任务主窗体_内存释放",
                    //    Level = LogConstParam.LogLevel_Info,
                    //    Msg = "释放内存",
                    //    Origin = "汽车衡_" + LoginUser.Role.Name,
                    //    OperateUserName = LoginUser.Name
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    #region 内存日志
                    //memorySize = GetMemoryAmount();
                    //LogModel log3 = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_In,
                    //    FunctionName = "坐席主窗体_内存释放后",
                    //    Level = LogConstParam.LogLevel_Error,
                    //    Msg = "内存释放后,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)",
                    //    Origin = LoginUser.Role.Name,
                    //    OperateUserName = LoginUser.Name,
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
                    #endregion
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务主窗体_内存释放",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "释放内存时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return Math.Round((double)memorySize / (1024 * 1024), 2);
        }

        /// <summary>
        /// 任务窗体关闭后事件
        /// </summary>
        void taskHandle_Closed(object sender, EventArgs e)
        {
            try
            {
                TaskHandleViewModel vm = null;
                SendReplyModel srm = null;
                if (currentTaskHView == null)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_任务主窗体_任务处理窗体关闭",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "任务窗体对象为空",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                else
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_任务主窗体_任务处理窗体关闭",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "开始获取任务窗体中的任务结束对象",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    vm = currentTaskHView.DataContext as TaskHandleViewModel;
                    srm = InfoExchange.Clone(vm.EndTaskModel);
                    #region 写日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_任务主窗体_任务处理窗体关闭",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "得到任务结束对象",
                        Data = srm,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                }
                //FlushMemory();
                //if (srm != null)//任务窗体有可能回退任务或者转发其他坐席，这种时候关闭的窗体不用结束任务
                //{
                //    EndTask(srm);
                //    Talent.ClinetLog.SysLog.Log("----------------------------------------------------------------------------------任务处理结束----------------------------------------------------------------------------------");
                //}
                DisposeTaskViewImage();
                currentTaskHView = null;
                AfterTaskHandle();
                //计算当前程序所占内存的大小（M），大于1000M时，将重新启动程序
                double memorySize = FlushMemory();
                if (memorySize >= 1000)
                {
                    ConfirmMessageBox mb = new ConfirmMessageBox("提示", "与服务器断开连接,系统将重新启动.", true, false, "确定", "");
                    mb.IsShowClose = false;
                    mb.ShowDialog();
                    if (mb.IsOk)
                    {
                        if (srm != null)//任务窗体有可能回退任务或者转发其他坐席，这种时候关闭的窗体不用结束任务
                        {
                            EndTask(srm);
                            Talent.ClinetLog.SysLog.Log("----------------------------------------------------------------------------------任务处理结束----------------------------------------------------------------------------------");
                        }
                        #region 注销系统
                        SocketCls.Emit(ClientSendCmdEnum.logout, "");//退出
                        SocketCls.DisConnectServer();
                        SocketCls.s.Dispose();
                        SocketCls.s = null;
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "坐席主窗体_内存过大系统注销",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = "当前内存:" + memorySize + "M,内存过大系统注销",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                            OperateUserName = LoginUser.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion

                        System.Windows.Forms.Application.Restart();
                        System.Threading.Thread.Sleep(500);
                        System.Windows.Application.Current.Shutdown(0);
                        #endregion
                    }
                }
                else
                {
                    if (srm != null)//任务窗体有可能回退任务或者转发其他坐席，这种时候关闭的窗体不用结束任务
                    {
                        EndTask(srm);
                        Talent.ClinetLog.SysLog.Log("----------------------------------------------------------------------------------任务处理结束----------------------------------------------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务主窗体_任务处理窗体关闭",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "在任务处理完毕,关闭任务窗体后主页面的后续逻辑处理时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 释放图片控件的资源
        /// </summary>
        private void DisposeTaskViewImage()
        {
            long memorySize = 0;
            if (currentTaskHView != null)
            {
                #region 释放任务处理窗体中的上次计量图片集合信息
                try
                {
                    TaskHandleViewModel thvm = currentTaskHView.DataContext as TaskHandleViewModel;
                    if (thvm.Pictures != null && thvm.Pictures.Count > 0)
                    {
                        #region 内存日志
                        //memorySize = GetMemoryAmount();
                        //LogModel log = new LogModel()
                        //{
                        //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        //    FunctionName = "坐席主窗体_释放图片的流资源",
                        //    Msg = "释放图片的流资源前,线程的内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                        //};
                        //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        //********图片加载方式从图片流改为ftp地址加载了，故而以下释放无意义了*********//
                        //foreach (var pictureModel in thvm.Pictures)
                        //{
                        //    if (pictureModel.image != null)
                        //    {
                        //        pictureModel.image.StreamSource.Close();
                        //        pictureModel.image.StreamSource.Dispose();
                        //        #region 写日志
                        //        LogModel log3 = new LogModel()
                        //        {
                        //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        //            Direction = LogConstParam.Directions_In,
                        //            FunctionName = "坐席主窗体_释放图片的流资源",
                        //            Level = LogConstParam.LogLevel_Info,
                        //            Msg = "释放第" + thvm.Pictures.IndexOf(pictureModel) + "个图片的流资源成功",
                        //            Origin = "汽车衡_" + LoginUser.Role.Name,
                        //            OperateUserName = LoginUser.Name
                        //        };
                        //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
                        //        #endregion
                        //    }
                        //}
                        #region 写日志
                        //LogModel log2 = new LogModel()
                        //{
                        //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        //    Direction = LogConstParam.Directions_In,
                        //    FunctionName = "坐席主窗体_释放图片的流资源",
                        //    Level = LogConstParam.LogLevel_Info,
                        //    Msg = "释放图片的流资源成功",
                        //    Origin = "汽车衡_" + LoginUser.Role.Name,
                        //    OperateUserName = LoginUser.Name
                        //};
                        //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                        #endregion
                        thvm.Pictures.Clear();
                    }
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log3 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席主窗体_释放图片的流资源",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "释放图片的流资源异常:" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
                    #endregion
                }
                #endregion

                try
                {
                    #region 释放大小视频承载的PictureBox的资源
                    #region 内存日志
                    memorySize = GetMemoryAmount();
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "坐席任务处理窗体_释放大视频前",
                        Msg = "释放大视频前,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    //大视频
                    currentTaskHView.videoBig.Dispose();
                    currentTaskHView.videoBig = null;
                    try
                    {
                        currentTaskHView.formHostBig.Dispose();
                        currentTaskHView.formHostBig = null;
                    }
                    catch
                    {
                    }

                    #region 内存日志
                    memorySize = GetMemoryAmount();
                    LogModel log4 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "坐席任务处理窗体_释放大视频后",
                        Msg = "释放大视频后,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log4));
                    #endregion
                    #region 写日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_释放大视频图片控件的资源",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "释放大视频图片控件的资源成功",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                    //小视频
                    foreach (var host in currentTaskHView.items1.Items)
                    {
                        System.Windows.Forms.Integration.WindowsFormsHost wfh = host as System.Windows.Forms.Integration.WindowsFormsHost;
                        PictureBox pb = wfh.Child as PictureBox;
                        pb.Dispose();
                        pb = null;
                        try
                        {
                            wfh.Dispose();
                        }
                        catch
                        {

                        }

                    }
                    #region 写日志
                    LogModel log2 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_释放小视频图片控件集合的资源",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "释放小视频图片控件集合的资源成功",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                    #endregion
                    #region 内存日志
                    memorySize = GetMemoryAmount();
                    LogModel log5 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "坐席任务处理窗体_释放小视频后",
                        Msg = "释放小视频后,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log5));
                    #endregion
                    #endregion

                    #region 写日志
                    //LogModel log3 = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_In,
                    //    FunctionName = "坐席_任务处理窗体_调用Only_WindowBase的Dispose释放资源",
                    //    Level = LogConstParam.LogLevel_Error,
                    //    Msg = "调用Only_WindowBase的Dispose释放资源成功",
                    //    Origin = "汽车衡_" + LoginUser.Role.Name,
                    //    OperateUserName = LoginUser.Name
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
                    #endregion
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_释放图片控件的资源",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "释放图片控件的资源时异常:" + ex.Message + "堆栈:" + ex.StackTrace,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                }
            }
        }

        /// <summary>
        /// 尝试结束任务
        /// </summary>
        /// <param name="endTaskModel">结束任务对象</param>
        private void TryEndTask(SendReplyModel endTaskModel)
        {
            EndTask(endTaskModel);
            Task checkEndTask = new Task(CheckEndTask, endTaskModel);
            checkEndTask.Start();
        }

        /// <summary>
        /// 检查任务结束信号任务服务器有没有反馈回来
        /// </summary>
        /// <param name="endTaskObj">任务结束对象</param>
        private void CheckEndTask(object endTaskObj)
        {
            SendReplyModel endTaskModel = endTaskObj as SendReplyModel;
            while (true)
            {
                Task.Delay(500);
                if (endTaskRandomNum != taskServerRandomNum)
                {
                    EndTask(endTaskModel);
                }
                else
                {
                    endTaskRandomNum = 0;
                    taskServerRandomNum = 0;
                    break;
                }
            }
        }

        /// <summary>
        /// 结束任务
        /// </summary>
        /// <param name="endTaskModel">任务结束参数对象</param>
        public async void EndTask(SendReplyModel endTaskModel)
        {
            await System.Threading.Tasks.Task.Delay(3000);
            string extendFunctionName = "";
            //if (endTaskRandomNum == 0)
            //{
            //    endTaskRandomNum = CommonMethod.CommonMethod.GetRandom();
            //}
            //else
            //{
            //    extendFunctionName = "_重发";
            //}
            var paraObj = new
            {
                clientid = endTaskModel.clientid,
                matchid = endTaskModel.matchid,
                result = endTaskModel.result,
                data = endTaskModel.data,
                msgid = endTaskRandomNum
            };
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_任务主窗体_" + (endTaskModel.result == 0 ? "任务终止" : "任务结束") + extendFunctionName,
                Level = LogConstParam.LogLevel_Info,
                Msg = "任务处理完毕,关闭任务窗体后向服务器发送" + SeatlistenCmdEnum.endtask + "命令",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                Data = paraObj,
                IsDataValid = LogConstParam.DataValid_Ok,
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatlistenCmdEnum.endtask } },
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            SocketCls.Emit(SeatlistenCmdEnum.endtask, JsonConvert.SerializeObject(paraObj));
        }

        #endregion
        #endregion

        protected override void MessageWindowEvent(ChildWindowCommandType type)
        {
            base.MessageWindowEvent(type);
            if (type != ChildWindowCommandType.OK)
            {
                if (MessageWindow.Parameter == "AcceptTask")//取消接收任务触发
                {
                    //发送"回收任务"命令给任务服务器
                    SocketCls.Emit(SeatSendCmdEnum.backTask, SelectedTask.ClientId);
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席主窗体",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "取消接收任务,发送回收任务命令【backTask】给任务服务器",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        Data = SelectedTask,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "backTask", ParamValue = SeatSendCmdEnum.backTask }, new DataParam() { ParamName = "clientId", ParamValue = SelectedTask.ClientId } },
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    SelectedTask = null;
                    IsBellMove = false;
                    if (!PauseButtonText.Equals("继续计量"))
                    {
                        ClientStateInfo = "任务处理中";
                    }
                    else
                    {
                        IsControlEnable = false;
                    }
                }
            }
            else
            {
                if (MessageWindow.Parameter == "AcceptTask")
                {
                    HandleTask();
                }
                if (MessageWindow.Parameter == "MeasureClientRestart")
                {
                    int unm = CommonMethod.CommonMethod.GetRandom();
                    var para = new
                    {
                        clientid = selectedWeighterClient.ClientId,
                        cmd = ParamCmd.ClientRestart,
                        msg = "终端重启",
                        msgid = unm
                    };
                    SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
                    //logH.SaveLog("点击终端重启，系统提示：确定重启终端,用户选择：确认");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席主窗体",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = selectedWeighterClient.ClientName + "终端重启",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        Data = para,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                if (MessageWindow.Parameter == "MeasureWeightClear")
                {
                    int unm = CommonMethod.CommonMethod.GetRandom();
                    var para = new
                    {
                        clientid = selectedWeighterClient.ClientId,
                        cmd = ParamCmd.MeasureWeightClear,
                        msg = "表头清零",
                        msgid = unm
                    };
                    SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
                    //logH.SaveLog("点击表头清零，提示用户是否确认清零,用户选择：继续");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席主窗体",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = selectedWeighterClient.ClientName + "表头清零",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        Data = para,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                if (MessageWindow.Parameter == "MeasureClientUpdate")
                {
                    int unm = CommonMethod.CommonMethod.GetRandom();
                    var para = new
                    {
                        clientid = selectedWeighterClient.ClientId,
                        cmd = ParamCmd.ClientUpdate,
                        msg = "版本更新",
                        msgid = unm
                    };
                    SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席主窗体",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = selectedWeighterClient.ClientName + "称点版本更新",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        Data = para,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = ParamCmd.ClientUpdate } },
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                if (MessageWindow.Parameter == "LogOutSystem")
                {
                    this.FormEnableSource = 1;
                    this.IsFormEnable = false;//cs代码中为注销的实际代码
                    //logH.SaveLog("点击注销系统，系统提示：确定注销吗,用户选择:确认");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席主窗体",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "点击注销系统，系统提示：确定注销吗,用户选择:确认",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                if (MessageWindow.Parameter == "CloseSystem")
                {
                    this.FormEnableSource = 2;
                    this.IsFormEnable = false;//cs代码中为关闭系统的实际代码
                    //logH.SaveLog("点击关闭系统，系统提示：确定关闭系统吗,用户选择:确认");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席主窗体",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "点击关闭系统，系统提示：确定关闭系统吗,用户选择:确认",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }

                if (MessageWindow.Parameter == "MeasureClientStop")//终端暂停计量
                {
                    int unm = CommonMethod.CommonMethod.GetRandom();
                    var para = new
                    {
                        clientid = selectedWeighterClient.ClientId,
                        cmd = ParamCmd.ClientStop,
                        msg = "终端暂停计量",
                        msgid = unm
                    };
                    SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
                    //logH.SaveLog("点击终端暂停计量，系统提示：确定暂停计量吗,用户选择：确认");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席主窗体",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = selectedWeighterClient.ClientName + "终端暂停计量",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        Data = para,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }

        /// <summary>
        /// 上传任务处理的概要信息回调的方法
        /// </summary>
        /// <param name="ar"></param>
        private void saveTaskDoResultCallback(IAsyncResult ar)
        {
            //AfterTaskHandle();
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 处理衡器显示的状态
        /// </summary>
        private void DoClientState()
        {
            if (this.SelectedTask != null)
            {
                var ls = (from r in CarWeighterClientInfos where r.ClientId == this.SelectedTask.ClientId select r).ToList();
                if (ls != null && ls.Count > 0)
                {
                    WeighterClientModel cM = ls.First();
                    int unm = CommonMethod.CommonMethod.GetRandom();
                    var paraObj = new
                    {
                        clientid = cM.ClientId,
                        cmd = ParamCmd.ClientState,
                        msg = "正在计量",
                        msgid = unm
                    };
                    SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(paraObj));
                }
            }
        }
        /// <summary>
        /// 窗体键盘按下事件
        /// </summary>
        private void handleTaskButton_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                //logH.SaveLog("按键接收任务");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_窗体键盘按下事件",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "窗体键盘按下事件",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                TaskProcess();
            }
        }
        /// <summary>
        /// 用户自定义背景颜色
        /// </summary>
        private void DoBackGroundColor()
        {
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string url = basePath + @"/ClientConfig/UserUseColor.txt";
            if (!FileHelper.IsExistFile(url))
            {
                FileHelper.CreateFile(url);
                FileHelper.AppendText(url, "#528DAD");
            }
            string userColor = FileHelper.FileToString(url);
            if (!userColor.Equals("#528DAD"))
            {
                BColor0 = userColor;
                BColor1 = userColor;
            }
        }
        /// <summary>
        /// 获取任务弹框坐标
        /// </summary>
        /// <returns></returns>
        private Thickness GetNewTaskGridThick()
        {
            Thickness thick = new Thickness(285, -79, 287, 279);
            Rect rect = SystemParameters.WorkArea;
            thick.Left = seatMainWindow.Width / 2 - seatMainWindow.NewTaskGrid.Width / 2;
            thick.Right = seatMainWindow.Width / 2 - seatMainWindow.NewTaskGrid.Width / 2;
            thick.Top = 0 - (seatMainWindow.Height / 2 - seatMainWindow.NewTaskGrid.Height / 2) - 100;
            thick.Bottom = (seatMainWindow.Height / 2 - seatMainWindow.NewTaskGrid.Height / 2) + 100;
            return thick;
        }

        /// <summary>
        /// 保存结果
        /// </summary>
        private void SaveDoResult()
        {
            if (seatMainWindow.NewTaskGrid.Visibility == Visibility.Hidden)
            {
                return;
            }
            string serviceUrl = ConfigurationManager.AppSettings["saveTaskDoResult"].ToString();
            var param = new
            {
                opname = LoginUser.Name,
                opcode = LoginUser.Code,
                taskbegintime = DateTime.Now.AddSeconds(-10).ToString("yyyyMMddHHmmss"),//时间改为24 小时制 2016-3-7 09:03:20……
                taskendtime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                equcode = this.SelectedTask.ClientCode,
                equname = this.SelectedTask.ClientName,
                carno = this.SelectedTask.CarNumber,
                taskdoresult = "系统自动回退",
                memo = "",
                equtype = EquTypeEnum.Type_Car_Seat,
                seatid = LoginUser.Role.Code,
                seatname = LoginUser.Role.Name
            };
            var jsonStr = "[" + JsonConvert.SerializeObject(param) + "]";
            string url = string.Format(serviceUrl, System.Web.HttpUtility.UrlEncode(jsonStr, System.Text.Encoding.UTF8));
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(url, 10);
            request.BeginGetResponse(new AsyncCallback(saveTaskDoResultCallback1), request);
        }
        private void saveTaskDoResultCallback1(IAsyncResult ar)
        {

        }
        #endregion

        /// <summary>
        /// 检测任务处理窗体是否激活
        /// </summary>
        public void CheckTaskHandleViewActive()
        {
            if (currentTaskHView != null)
            {
                currentTaskHView.Activate();
            }
        }
    }
}
