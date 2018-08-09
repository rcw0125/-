using Microsoft.Expression.Interactivity.Core;
using Newtonsoft.Json;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Talent.ClientCommMethod;
using Talent.ClientCommonLib;
using Talent.ClientCommonLib.Controls;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.RemoteCarMeasure.View;
using Talent.CommonMethod;
using System.Windows.Media.Imaging;
using Talent.Audio.Controller;
using Talent.RemoteCarMeasure.Commom;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Talent.Measure.DomainModel.ServiceModel;
using System.Threading;
using Talent.Measure.WPF;
using Talent.Measure.WPF.Remote;
using Talent_LT.HelpClass;
using Talent.Measure.WPF.Log;
using System.ComponentModel;
using System.Diagnostics;

namespace Talent.RemoteCarMeasure.ViewModel
{
    /// <summary>
    /// 任务处理viewModel
    /// added by wangc on 20151020
    /// </summary>
    public class TaskHandleViewModel : Only_ViewModelBase
    {
        /// <summary>
        /// 当前任务过磅单号
        /// </summary>
        private string curMachid = "";
        private SendReplyModel endTaskModel;
        /// <summary>
        /// 任务结束时用的参数
        /// </summary>
        public SendReplyModel EndTaskModel
        {
            get { return endTaskModel; }
            set
            {
                endTaskModel = value;
            }
        }

        public AudioController audioController;

        private string voiceMsg;
        /// <summary>
        /// 语音信息
        /// </summary>
        public string VoiceMsg
        {
            get { return voiceMsg; }
            set
            {
                voiceMsg = value;
                this.RaisePropertyChanged("VoiceMsg");
            }
        }

        private string voiceTalkButtonCotent;
        /// <summary>
        /// 语音对讲按钮显示的内容
        /// </summary>
        public string VoiceTalkButtonCotent
        {
            get { return voiceTalkButtonCotent; }
            set
            {
                voiceTalkButtonCotent = value;
                this.RaisePropertyChanged("VoiceTalkButtonCotent");
            }
        }

        private bool isVoiceOpend;
        /// <summary>
        /// 语音通话是否已打开
        /// </summary>
        public bool IsVoiceOpend
        {
            get { return isVoiceOpend; }
            set
            {
                isVoiceOpend = value;
                if (value)
                {
                    VoiceMsg = "正在通话中....";
                    VoiceTalkButtonCotent = "关闭对讲";
                }
                else
                {
                    VoiceMsg = "未进行通话....";
                    VoiceTalkButtonCotent = "打开对讲";
                }
            }
        }

        #region 页面控件
        /// <summary>
        /// 页面信息
        /// </summary>
        public Grid gridReader;
        /// <summary>
        /// 供方信息
        /// </summary>
        public Grid gridSupplier;
        /// <summary>
        /// 计量信息
        /// </summary>
        public Grid gridMeasure;
        /// <summary>
        /// 称量信息
        /// </summary>
        public Grid gridMeasureWeight;
        /// <summary>
        /// 下拉框pop
        /// </summary>
        public Popup dropDownPop;
        /// <summary>
        /// 下拉表格pop
        /// </summary>
        public Popup dataViewPop;
        /// <summary>
        /// 下拉表格pop的父级grid
        /// </summary>
        public Grid popParentGrid;
        #endregion

        #region 属性

        private string curWeight;
        /// <summary>
        /// 当前重量
        /// </summary>
        public string CurWeight
        {
            get { return curWeight; }
            set
            {
                //未点击“确定”按钮。确定按钮点击时，就要锁定当前重量，不再接收实时数据反馈过来的重量
                if (!isClickSave)
                {
                    curWeight = value;
                    this.RaisePropertyChanged("CurWeight");
                    if (this.HandleTask != null && this.HandleTask.BullInfo != null)
                    {
                        if (IsGrossWeight)
                        {
                            this.HandleTask.BullInfo.gross = string.IsNullOrEmpty(value) ? 0.00M : decimal.Parse(value) * weightDot;

                        }
                        else if (IsTareWeight)
                        {
                            this.HandleTask.BullInfo.tare = string.IsNullOrEmpty(value) ? 0.00M : decimal.Parse(value) * weightDot;

                        }
                        DoShowWeightInfo(false);
                    }
                }
            }
        }

        private bool isViewEnable;
        /// <summary>
        /// 窗体是否可用
        /// </summary>
        public bool IsViewEnable
        {
            get { return isViewEnable; }
            set
            {
                isViewEnable = value;
                this.RaisePropertyChanged("IsViewEnable");
            }
        }

        private TaskModel handleTask;
        /// <summary>
        /// 当前处理的任务
        /// </summary>
        public TaskModel HandleTask
        {
            get { return handleTask; }
            set
            {
                handleTask = value;
                if (value != null)
                {
                    //处理显示的重量信息
                    DoShowWeightInfo(false);
                }
                this.RaisePropertyChanged("HandleTask");
            }
        }

        private string taskName;
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName
        {
            get { return taskName; }
            set
            {
                taskName = value;
                this.RaisePropertyChanged("TaskName");
            }
        }

        private string curWeighterText;
        /// <summary>
        /// 当前称量的重量名称
        /// </summary>
        public string CurWeighterText
        {
            get { return curWeighterText; }
            set
            {
                curWeighterText = value;
                this.RaisePropertyChanged("CurWeighterText");
            }
        }

        private ObservableCollection<VoiceModel> voiceCollections;
        /// <summary>
        /// 声音集合
        /// </summary>
        public ObservableCollection<VoiceModel> VoiceCollections
        {
            get { return voiceCollections; }
            set
            {
                voiceCollections = value;
                this.RaisePropertyChanged("VoiceCollections");
            }
        }

        private bool isGrossWeight;
        /// <summary>
        /// 是否为"毛重"(radioButton使用)
        /// </summary>
        public bool IsGrossWeight
        {
            get { return isGrossWeight; }
            set
            {
                isGrossWeight = value;
                this.RaisePropertyChanged("IsGrossWeight");
                CurWeighterText = value ? "毛重" : "皮重";
                if (value)
                {
                    if (this.HandleTask.BullInfo.operatype.Equals("10"))//调拨业务启用是否使用长皮
                    {
                        AutoTareIsShow = Visibility.Visible;
                    }
                    if (!autoTareIsCheck)
                    {
                        this.HandleTask.BullInfo.tare = this.originTare;
                    }
                    this.HandleTask.BullInfo.gross = string.IsNullOrEmpty(CurWeight) ? 0.00M : decimal.Parse(CurWeight) * weightDot;
                    this.HandleTask.BullInfo.measurestate = "G";
                    DoShowWeightInfo(false);
                    SendBusinessInfosToTaskServer();//坐席选择改变时，秤点也要改变业务信息……lt 2016-2-25 10:15:57……
                }
            }
        }
        /// <summary>
        /// 原始毛重
        /// </summary>
        private decimal originGross;
        private bool isTareWeight;
        /// <summary>
        /// 是否为"皮重"(radioButton使用)
        /// </summary>
        public bool IsTareWeight
        {
            get { return isTareWeight; }
            set
            {
                isTareWeight = value;
                this.RaisePropertyChanged("IsTareWeight");
                CurWeighterText = value ? "皮重" : "毛重";
                if (value)
                {
                    if (!this.HandleTask.BullInfo.operatype.Equals("80"))
                    {
                        DecutionIsReadOnly = false;
                    }
                    this.HandleTask.BullInfo.gross = this.originGross;
                    this.HandleTask.BullInfo.tare = string.IsNullOrEmpty(CurWeight) ? 0.00M : decimal.Parse(CurWeight) * weightDot;
                    this.HandleTask.BullInfo.measurestate = "T";
                    DoShowWeightInfo(false);
                    SendBusinessInfosToTaskServer();
                }
                else
                {
                    DecutionIsReadOnly = true;
                }
            }
        }
        /// <summary>
        /// 原始皮重
        /// </summary>
        private decimal originTare;
        /// <summary>
        /// 称点配置
        /// </summary>
        private configlist measureConfig;
        /// <summary>
        /// 配置文件名称(带路径)
        /// </summary>
        private string configPath;

        /// <summary>
        /// 配置文件名称
        /// </summary>
        public string configName;

        private string rollMsg;
        /// <summary>
        /// 滚动字信息
        /// </summary>
        public string RollMsg
        {
            get { return rollMsg; }
            set
            {
                rollMsg = value;
                this.RaisePropertyChanged("RollMsg");
            }
        }

        private Visibility leftLine;
        /// <summary>
        /// 左红外被挡时显示的"×"
        /// </summary>
        public Visibility LeftLine
        {
            get { return leftLine; }
            set
            {
                leftLine = value;
                this.RaisePropertyChanged("LeftLine");
            }
        }

        private Visibility rightLine;
        /// <summary>
        /// 右红外被挡时显示的"×"
        /// </summary>
        public Visibility RightLine
        {
            get { return rightLine; }
            set
            {
                rightLine = value;
                this.RaisePropertyChanged("RightLine");
            }
        }

        private System.Windows.Media.Brush light;
        /// <summary>
        /// 指示灯(用于红绿灯显示)
        /// </summary>
        public System.Windows.Media.Brush Light
        {
            get { return light; }
            set
            {
                light = value;
                this.RaisePropertyChanged("Light");
            }
        }

        private bool isOpenVideo;
        /// <summary>
        /// 是否打开视频
        /// </summary>
        public bool IsOpenVideo
        {
            get { return isOpenVideo; }
            set { isOpenVideo = value; this.RaisePropertyChanged("IsOpenVideo"); }
        }

        private string seatMeasureType;
        /// <summary>
        /// 坐席计量方式(自动计量;手动计量)
        /// </summary>
        public string SeatMeasureType
        {
            get { return seatMeasureType; }
            set { seatMeasureType = value; }
        }

        private int timeCount;
        /// <summary>
        /// 自动确认倒计时时间(单位:秒)
        /// </summary>
        public int TimeCount
        {
            get { return timeCount; }
            set
            {
                timeCount = value;
            }
        }
        private Visibility timeCountVisibility;
        /// <summary>
        /// 倒计时是否显示
        /// </summary>
        public Visibility TimeCountVisibility
        {
            get { return timeCountVisibility; }
            set
            {
                timeCountVisibility = value;
                this.RaisePropertyChanged("TimeCountVisibility");
            }
        }
        private string bColor0 = "#FFFFFFFF";
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
        /// <summary>
        /// 取数次数
        /// </summary>
        public int WeightTimeCount;
        /// <summary>
        /// 重量列表
        /// </summary>
        List<decimal> WeightList = new List<decimal>();
        /// <summary>
        /// 是否取数的开关
        /// </summary>
        public bool IsGetWeight { get; set; }
        /// <summary>
        /// 称重计时器
        /// </summary>
        private System.Windows.Forms.Timer timer;
        /// <summary>
        /// 称重允差(称点)
        /// </summary>
        private decimal weightAllowance;

        /// <summary>
        /// 是否为读取数据(窗体中读取数据按钮业务实现需要的开关量)
        /// </summary>
        private bool IsReadData;
        private string logMustInfo = string.Empty;
        /// <summary>
        /// 处理结果
        /// </summary>
        public string HandleResult;
        private List<PictureModel> pictures;
        /// <summary>
        /// 图片集合(上一个计量流程)
        /// </summary>
        public List<PictureModel> Pictures
        {
            get { return pictures; }
            set
            {
                pictures = value;
                this.RaisePropertyChanged("Pictures");
            }
        }

        public string lastTare = "0";
        /// <summary>
        /// 上一次皮重 重量 2016-3-15 09:01:06……
        /// </summary>
        public string LastTare
        {
            get { return lastTare; }
            set
            {
                lastTare = value;
                this.RaisePropertyChanged("LastTare");
            }
        }
        /// <summary>
        /// 车号
        /// </summary>
        private string carNo;
        /// <summary>
        /// IC卡号
        /// </summary>
        private string icId;
        /// <summary>
        /// RFID卡号
        /// </summary>
        private string rfidId;
        /// <summary>
        /// 历史皮重集合
        /// </summary>
        private List<BullInfo> historyTareLst = new List<BullInfo>();
        /// <summary>
        /// 调用服务获取历史皮重集合
        /// </summary>
        private bool isGetHistoryTare = false;
        /// <summary>
        /// 将千克转为吨 解决后台存储与前台显示一致的问题  例如后台存储吨 前台显示千克
        /// </summary>
        private decimal weightDot = Convert.ToDecimal(1);
        /// <summary>
        /// 将千克转为吨 解决后台存储与前台显示不一致的问题  例如后台存储千克 前台显示吨
        /// </summary>
        private decimal weightKg = Convert.ToDecimal(0.001);
        /// <summary>
        /// 等待打印结果计时器
        /// </summary>
        private Calculagraph waitPrintReusltTimer;
        #endregion

        #region 命令
        /// <summary>
        /// "确定重量"命令
        /// </summary>
        public ICommand ConfirmWeightCommand { get; private set; }
        /// <summary>
        /// "读取数据"命令
        /// </summary>
        public ICommand ReadDataCommand { get; private set; }
        /// <summary>
        /// "上一张图片"命令
        /// </summary>
        public ICommand PrePictureCommand { get; private set; }
        /// <summary>
        /// "下一张图片"命令
        /// </summary>
        public ICommand NextPictureCommand { get; private set; }
        /// <summary>
        /// "声音"命令
        /// </summary>
        public ICommand VoiceCommand { get; private set; }
        /// <summary>
        /// "终止计量"命令
        /// </summary>
        public ICommand StopMeasureCommand { get; private set; }
        /// <summary>
        /// "重选选择业务信息"命令
        /// </summary>
        public ICommand ReChooseBusinessCommand { get; private set; }
        /// <summary>
        /// 打开语音对讲功能
        /// </summary>
        public ICommand OpenTalkVoice { get; private set; }
        /// <summary>
        /// 显示历史皮重
        /// </summary>
        public ICommand ShowHistoryTareCommand { get; private set; }
        /// <summary>
        /// 发送通知消息
        /// </summary>
        public ICommand SendMSGCommand { get; private set; }
        #endregion
        /// <summary>
        /// 终止计量的原因 lt 2016-2-16 08:13:59……
        /// </summary>
        private string stopTaskReason = string.Empty;
        private bool _isClickSave = false;
        /// <summary>
        /// 是否点击了保存
        /// </summary>
        public bool isClickSave
        {
            get { return _isClickSave; }
            set
            {
                _isClickSave = value;
                IsConfirmUse = !value;
            }
        }

        private bool isConfirmUse;
        /// <summary>
        /// 确定按钮是否可用
        /// </summary>
        public bool IsConfirmUse
        {
            get { return isConfirmUse; }
            set
            {
                isConfirmUse = value;
                this.RaisePropertyChanged("IsConfirmUse");
            }
        }
        #region 显示的重量信息 由千克变为吨显示

        string showGross;
        /// <summary>
        /// 毛重
        /// </summary>
        public string ShowGross
        {
            get { return showGross; }
            set
            {
                showGross = value;
                this.RaisePropertyChanged("ShowGross");
            }
        }

        string showTare;
        /// <summary>
        /// 皮重
        /// </summary>
        public string ShowTare
        {
            get { return showTare; }
            set
            {
                showTare = value;
                this.RaisePropertyChanged("ShowTare");
            }
        }

        string showSuttle;
        /// <summary>
        /// 净重
        /// </summary>
        public string ShowSuttle
        {
            get { return showSuttle; }
            set
            {
                showSuttle = value;
                this.RaisePropertyChanged("ShowSuttle");
            }
        }

        string showDedution;
        /// <summary>
        /// 扣重
        /// </summary>
        public string ShowDedution
        {
            get { return showDedution; }
            set
            {
                showDedution = value;
                this.RaisePropertyChanged("ShowDedution");
            }
        }

        string showGrossb;
        /// <summary>
        /// 供方毛重
        /// </summary>
        public string ShowGrossb
        {
            get { return showGrossb; }
            set
            {
                showGrossb = value;
                this.RaisePropertyChanged("ShowGrossb");
            }
        }

        string showTareb;
        /// <summary>
        /// 供方皮重
        /// </summary>
        public string ShowTareb
        {
            get { return showTareb; }
            set
            {
                showTareb = value;
                this.RaisePropertyChanged("ShowTareb");
            }
        }

        string showSuttleb;
        /// <summary>
        /// 供方净重
        /// </summary>
        public string ShowSuttleb
        {
            get { return showSuttleb; }
            set
            {
                showSuttleb = value;
                this.RaisePropertyChanged("ShowSuttleb");
            }
        }


        bool decutionIsReadOnly = true;
        /// <summary>
        /// 扣重信息是否只读
        /// </summary>
        public bool DecutionIsReadOnly
        {
            get { return decutionIsReadOnly; }
            set
            {
                decutionIsReadOnly = value;
                this.RaisePropertyChanged("DecutionIsReadOnly");
            }
        }


        Visibility autoTareIsShow = Visibility.Hidden;
        /// <summary>
        /// 是否显示回皮
        /// </summary>
        public Visibility AutoTareIsShow
        {
            get { return autoTareIsShow; }
            set
            {
                autoTareIsShow = value;
                this.RaisePropertyChanged("AutoTareIsShow");
            }
        }
        /// <summary>
        /// 是否选择回皮
        /// </summary>
        bool autoTareIsCheck = false;
        public bool AutoTareIsCheck
        {
            get { return autoTareIsCheck; }
            set
            {
                autoTareIsCheck = value;
                if (value)//确定回皮之后
                {
                    ClearHistoryTare();
                }
                else
                {
                    ShowHistoryTare();
                }
                this.RaisePropertyChanged("AutoTareIsCheck");
            }
        }

        #endregion
        /// <summary>
        /// 日志记录
        /// </summary>
        LogsHelpClass logH = new LogsHelpClass();
        #region 构造

        public TaskHandleViewModel()
        {
            if (this.IsInDesignMode)
                return;
            this.ShowBusy = true;
            this.TimeCountVisibility = Visibility.Hidden;
            ReadDataCommand = new ActionCommand(ReadBusinessDataMethod);
            ConfirmWeightCommand = new ActionCommand(ConfirmWeightMethod);
            VoiceCommand = new ActionCommand(SendVoiceRemind);
            StopMeasureCommand = new ActionCommand(StopMeasureMehtod);
            ReChooseBusinessCommand = new ActionCommand(ReChooseBusinessMethod);
            OpenTalkVoice = new ActionCommand(OperateVoiceTalk);
            ShowHistoryTareCommand = new ActionCommand(ShowHistoryTareMethod);
            SendMSGCommand = new ActionCommand(SendMSGMethod);
            LeftLine = Visibility.Hidden;
            RightLine = Visibility.Hidden;
            WeightList.Clear();
            IsGetWeight = false;
            IsViewEnable = true;
            #region 注册记时时间
            timer = new System.Windows.Forms.Timer();
            timer.Tick += timer_Tick;
            DoBackGroundColor();
            #endregion
        }

        #endregion

        #region 方法

        private void ClosePopup(Popup curPop)
        {
            UIElementCollection children = popParentGrid.Children;
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

        /// <summary>
        /// 读取业务数据
        /// </summary>
        private void ReadBusinessDataMethod()
        {
            //logH.SaveLog("点击重新读取数据");
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "坐席_处理任务窗体_读取业务数据",
                Level = LogConstParam.LogLevel_Info,
                Msg = "点击重新读取数据",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            isClickSave = false;
            this.ShowBusy = true;
            IsReadData = true;
            //历史皮重初始化……2016-3-17 14:06:06
            historyTareLst.Clear();
            LastTare = "0";
            isGetHistoryTare = false;
            ReadDataMethod();
        }

        /// <summary>
        /// 重选选择业务信息方法
        /// </summary>
        private void ReChooseBusinessMethod()
        {
            if (this.HandleTask.BullInfos.Count > 1)
            {
                SelectedTaskBusinessView stbv = new SelectedTaskBusinessView(this.HandleTask.BullInfos);
                stbv.ShowDialog();
                this.HandleTask.BullInfo = InfoExchange.Clone<BullInfo>(stbv.SelectedBull);
            }
            else
            {
                ConfirmMessageBox mb = new ConfirmMessageBox("提示", "无多个业务信息可供选择", true, false, "确定", "取消");
                mb.ShowDialog();
            }
        }

        #region 称重计时控制
        /// <summary>
        /// 计时器开始,开始从实时数据中获取重量数据
        /// </summary>
        public void TimerStart()
        {
            WeightList.Clear();
            timer.Start();
            IsGetWeight = true;
        }
        /// <summary>
        /// 计时器停止,停止从实时数据中获取实时数据
        /// </summary>
        public void TimerStop()
        {
            try
            {
                timer.Stop();
                IsGetWeight = false;
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_计时器停止,停止从实时数据中获取实时数据",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取实时数据计时器停止时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
            }
        }
        #endregion

        /// <summary>
        /// 发送语音提示
        /// </summary>
        /// <param name="obj">选择的语音提示对象</param>
        private void SendVoiceRemind(object obj)
        {
            //logH.SaveLog("用户将语音信息：" + obj + "发送秤体与秤体：" + HandleTask.ClientName);
            VoiceRemindHelper.SendVoiceInfoToMeasure(obj, HandleTask.ClientId);
            #region 写日志
            LogModel log = new LogModel()
            {
                Origin = LoginUser.Role.Name,
                FunctionName = "坐席_任务处理窗体_发送语音提示",
                Level = LogConstParam.LogLevel_Info,
                OperateUserName = LoginUser.Name,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                Msg = "汽车衡发送语音提示命令:" + SeatSendCmdEnum.cmd2client + "给任务服务器",
                Data = new { clientid = HandleTask.ClientId, cmd = ParamCmd.Voice_Prompt, msg = obj },
                IsDataValid = LogConstParam.DataValid_Ok,
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },

            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        /// <summary>
        /// 打开/关闭语音通话
        /// </summary>
        private void OperateVoiceTalk()
        {
            if (audioController != null)
            {
                if (!IsVoiceOpend)
                {
                    try
                    {
                        audioController.Start();
                        if (this.HandleTask != null)
                        {
                            VoiceRemindHelper.SendVoiceTalkStartToMeasure(this.HandleTask.ClientId);
                            #region 写日志
                            LogModel log = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_Out,
                                FunctionName = "坐席_任务处理窗体_打开语音通话",
                                Level = LogConstParam.LogLevel_Info,
                                Msg = "发送打开语音对讲命令" + SeatSendCmdEnum.cmd2client + "给任务服务器",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                                Data = new { clientid = this.HandleTask.ClientId, cmd = ParamCmd.Voice_Prompt, msg = "语音对讲开始" },
                                IsDataValid = LogConstParam.DataValid_Ok,
                                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                                OperateUserName = LoginUser.Name
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            //FileHelpClass.WriteLog("往任务服务器发送打开对讲完成：");
                            #endregion
                        }
                        IsVoiceOpend = true;
                    }
                    catch (Exception ex)
                    {
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            FunctionName = "坐席_任务处理窗体_打开语音通话",
                            Level = LogConstParam.LogLevel_Error,
                            Msg = "打开对讲出现错误：" + ex.Message,
                            Origin = "汽车衡_" + LoginUser.Role.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "语音对讲连接失败,无法打开!", true, false, "确定", "取消");
                        mb.ShowDialog();
                    }
                }
                else
                {
                    try
                    {
                        audioController.Stop();
                        audioController.Close();
                        if (HandleTask != null)
                        {
                            VoiceRemindHelper.SendVoiceTalkEndToMeasure(HandleTask.ClientId);
                            #region 写日志
                            LogModel log = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_Out,
                                FunctionName = "坐席_任务处理窗体_关闭语音通话",
                                Level = LogConstParam.LogLevel_Info,
                                Msg = "发送关闭语音对讲命令：" + SeatSendCmdEnum.cmd2client + "给任务服务器",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                                Data = new { clientid = HandleTask.ClientId, cmd = ParamCmd.Voice_Prompt, msg = "语音对讲结束" },
                                IsDataValid = LogConstParam.DataValid_Ok,
                                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                                OperateUserName = LoginUser.Name
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            #endregion
                        }
                        IsVoiceOpend = false;
                    }
                    catch (Exception ex)
                    {
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            FunctionName = "坐席_任务处理窗体_关闭语音通话",
                            Level = LogConstParam.LogLevel_Error,
                            Msg = "关闭对讲出现错误：" + ex.Message,
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "语音对讲关闭失败!", true, false, "确定", "取消");
                        mb.ShowDialog();
                    }
                }
            }
        }

        /// <summary>
        /// 确定重量方法
        /// </summary>
        private void ConfirmWeightMethod()
        {
            if (!isClickSave)//多次点击无效
            {
                if ((this.LeftLine == Visibility.Visible || this.RightLine == Visibility.Visible))
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_确定重量方法",
                        Level = LogConstParam.LogLevel_Warning,
                        Msg = "红外对射遮挡",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    ConfirmMessageBox mb = new ConfirmMessageBox("提示", "红外对射遮挡,是否继续计量？", true, true, "确定", "取消");
                    mb.ShowDialog();
                    if (mb.IsOk)
                    {
                        isClickSave = true;
                        saveWeightServiceInfo();
                    }
                }
                else
                {
                    isClickSave = true;
                    saveWeightServiceInfo();
                }
            }
            else
            {
                //ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "已经点击过确定按钮,多次点击无效!", true, false, "确定", "取消");
                //mb.ShowDialog();
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_点击确定",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "已经点击过确定,再次点击确定按钮无效!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 终止计量
        /// </summary>
        private void StopMeasureMehtod()
        {
            ConfirmMessageBox mb = new ConfirmMessageBox("提示", "确定终止计量任务吗？", true, true, "确定", "取消");
            mb.ShowDialog();
            if (mb.IsOk)
            {
                MessageWindowEvent("StopTask");
            }
        }

        /// <summary>
        /// 读取数据方法
        /// </summary>
        private void ReadDataMethod()
        {
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["getMeasureInfo"].ToString().Replace('$', '&');
                string getUrl = string.Format(serviceUrl, carNo, icId, rfidId, 1, "", this.HandleTask.ClientId, 1, this.CurWeight);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "开始调用服务取业务数据!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = getUrl,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                request.BeginGetResponse(new AsyncCallback(getMeasureInfoCallback), request);
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "取业务数据异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "获取数据失败，原因:" + ex.Message, true, true, "确定", "取消");
                mb.ShowDialog();
            }

        }
        /// <summary>
        /// 通过服务获取业务信息的回调方法
        /// </summary>
        /// <param name="asyc"></param>
        public void getMeasureInfoCallback(IAsyncResult asyc)
        {
            try
            {
                MeasureServiceModel mServiceModel;
                string strResult = ComHelpClass.ResponseStr(asyc);
                mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_通过服务获取业务信息的回调方法",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务器读取业务数据成功!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = strResult,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.HandleTask.ServiceModel = mServiceModel;
                this.HandleTask.BullInfos = mServiceModel != null ? mServiceModel.rows : null;
                this.ShowBusy = false;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                 {
                     try
                     {
                         NextBusinessInit();
                     }
                     catch (Exception ex)
                     {
                         #region 写日志
                         LogModel log1 = new LogModel()
                         {
                             CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                             Direction = LogConstParam.Directions_In,
                             FunctionName = "坐席_处理任务窗体_窗体初始化",
                             Level = LogConstParam.LogLevel_Info,
                             Msg = "调用服务反馈结果后,继续初始化窗体时异常:" + ex.StackTrace,
                             Origin = "汽车衡_" + LoginUser.Role.Name,
                             OperateUserName = LoginUser.Name
                         };
                         Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                         #endregion
                     }
                 }));

            }
            catch (Exception ex)
            {
                this.ShowBusy = false;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                 {
                     ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "获取业务信息失败，原因:" + ex.Message, true, true, "确定", "取消");
                     mb.ShowDialog();
                 }));
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_通过服务获取业务信息的回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务读取业务数据失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        private bool openVidoTemp;
        /// <summary>
        /// 构造窗体数据
        /// </summary>
        public void InitForms(TaskModel tm, string configName, bool isOpenVideo, string measureType, int timeCount, int weightTimeGap, int weightTimeCount, decimal inWeight)
        {
            this.BusyText = "开始读取相关基础配置..";
            this.HandleTask = tm;
            openVidoTemp = isOpenVideo;
            this.configName = configName;
            this.SeatMeasureType = measureType;
            this.TimeCount = timeCount;
            this.TimeCountVisibility = Visibility.Collapsed;
            this.WeightTimeCount = weightTimeCount;
            this.CurWeight = inWeight.ToString();//解决相似重量刚开始不存在重量的问题……lt 2016-2-18 14:12:00……
            WeightList = new List<decimal>();
            TaskName = "当前任务:" + HandleTask.ClientName + "求助,操作人：" + LoginUser.Name;//增加显示当前操作人 lt2016-2-16 10:57:09……
            #region 坐席配置中读取声音集合
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ClientConfig");
            var configPath = System.IO.Path.Combine(basePath, "SystemConfig.xml");
            VoiceCollections = VoiceRemindHelper.ReadVoiceConfig(string.Empty, configPath, out this.measureConfig);
            #endregion
            #region 称点配置中称点称重允差
            string basePath1 = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            this.configPath = System.IO.Path.Combine(basePath1, tm.ClientId + ".xml");
            string WeightAllowance = ConfigurationManager.AppSettings["WeightAllowanceInfo"].ToString();
            string getWeightAllowanceInfo = XpathHelper.GetValue(this.configPath, WeightAllowance);
            if (!string.IsNullOrEmpty(getWeightAllowanceInfo))
            {
                this.weightAllowance = Convert.ToDecimal(getWeightAllowanceInfo);
            }
            #endregion
            timer.Interval = weightTimeGap * 1000;

            #region 获取等待打印结果时间
            int waitPrintResultTime = 1;
            string waitPrintResultTimeItem = ConfigurationManager.AppSettings["WaitPrintResultTime"].ToString();
            string getWaitPrintResultTimeItem = XpathHelper.GetValue(configPath, waitPrintResultTimeItem);
            if (!string.IsNullOrEmpty(getWaitPrintResultTimeItem))
            {
                waitPrintResultTime = Convert.ToInt32(getWaitPrintResultTimeItem);
            }
            #endregion

            #region 注册等待打印结果计时器
            waitPrintReusltTimer = new Calculagraph("");
            waitPrintReusltTimer.Timeout = waitPrintResultTime;
            waitPrintReusltTimer.TimeOver += new TimeoutCaller(waitPrintReusltTimer_TimeOver);
            #endregion

            #region 语音对讲初始化
            try
            {
                this.BusyText = "开始初始化语音对讲..";
                IsVoiceOpend = false;
                if (this.HandleTask == null)
                {
                    return;
                }
                configPath = System.IO.Path.Combine(basePath1, this.HandleTask.ClientId + ".xml");
                audioController = new AudioController(configPath);
                audioController.OnShowErrMsg += audioController_OnShowErrMsg;
                audioController.Open();
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体构造窗体数据",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "初始化对讲错误：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            #endregion

            isClickSave = false;
            string icCardNo = GetICCard();
            //注册socket
            SocketCls.listenEvent += SocketCls_listenEvent;
            //卡号为空,说明是卡验证异常,此时直接走后续业务处理,不再查业务信息
            //或者卡不空,任务中标记让坐席不查询业务信息,也直接走后续流程
            if (string.IsNullOrEmpty(icCardNo) || !this.HandleTask.IsBusinessInfoQuery)
            {
                NextBusinessInit();
            }
            else//卡不空，同时任务中标记让坐席查询业务信息
            {
                this.BusyText = "开始读取业务数据..";
                IsReadData = true;
                this.rfidId = this.HandleTask.ServiceModel.data.rfidId;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体构造窗体数据",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "获取到RFID卡号为：" + rfidId,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.icId = icCardNo;
                this.carNo = this.HandleTask.CarNumber;
                ReadDataMethod();
            }
        }

        /// <summary>
        /// 构造窗体数据的后半部分(由于有异步查询服务业务数据，故而产生了此种情况)
        /// </summary>
        private void NextBusinessInit()
        {
            System.GC.Collect();
            stopTaskReason = string.Empty;
            List<BullInfo> flagsList = new List<BullInfo>();
            if (this.HandleTask != null && this.HandleTask.BullInfos != null)
            {
                //判断任务的业务数据是否为多条业务信息
                if (this.HandleTask.BullInfos.Count > 1)
                {
                    SelectedTaskBusinessView stbv = new SelectedTaskBusinessView(this.HandleTask.BullInfos);
                    stbv.ShowDialog();
                    this.HandleTask.BullInfo = InfoExchange.Clone<BullInfo>(stbv.SelectedBull);
                    //this.HandleTask.ServiceModel.rows.Clear();
                    //this.HandleTask.ServiceModel.rows.Add(this.HandleTask.BullInfo);//同步终端信息
                    //logH.SaveLog("根据车号获取信息存在多条记录，用户选择：" + this.HandleTask.BullInfo.matchid);
                    #region 日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "根据车号获取信息存在多条记录，用户选择：" + this.HandleTask.BullInfo.matchid,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name,
                        Data = this.HandleTask.BullInfo,
                        IsDataValid = LogConstParam.DataValid_Ok
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    GetOneMatchidInfos(this.HandleTask.BullInfo);
                    return;
                }
                else
                {
                    this.HandleTask.BullInfo = this.HandleTask.BullInfos.Count == 0 ? new BullInfo() : InfoExchange.Clone<BullInfo>(this.HandleTask.BullInfos.First());
                }
                logMustInfo = "过磅单号：" + this.HandleTask.BullInfo.matchid + "  车号：" + this.HandleTask.BullInfo.carno;
                DoDecimalShowInfos();
                if (this.HandleTask.ServiceModel != null && this.HandleTask.ServiceModel.mores != null && this.HandleTask.ServiceModel.mores.Count > 0)
                {
                    this.BusyText = "开始加载各业务控件..";
                    RenderMainUI ui = new RenderMainUI(0, gridReader, gridSupplier, gridMeasure, gridMeasureWeight, this.HandleTask.BullInfo, this.HandleTask.ServiceModel.mores, this.dropDownPop, this.dataViewPop);
                    ui.SetRenderMainUI();
                }
                else
                {
                    //if (gridReader != null && gridReader.Children != null)
                    //{
                    //   gridReader.Children.Clear();
                    //}
                    this.carNo = this.HandleTask.CarNumber;
                    stopTaskReason = this.carNo + ":无计量业务信息系统将终止计量。";//提示信息增加车号……
                    if (!this.HandleTask.ServiceModel.success)//返回false 直接提示服务返回信息…… lt 2016-2-3 14:35:33……
                    {
                        stopTaskReason = this.HandleTask.ServiceModel.msg;
                    }
                    else
                    {
                        if (!this.handleTask.ErrorMsg.Contains("远程计量任务"))
                        {
                            stopTaskReason = stopTaskReason + this.handleTask.ErrorMsg;
                        }
                    }
                    bool isContinue = string.IsNullOrEmpty(this.carNo) ? false : true;//为空的话只能终止计量
                    this.ShowBusy = false;
                    ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", stopTaskReason, true, isContinue, isContinue ? "调拨计量" : "终止计量", "单独计皮");
                    confirmBox.ShowDialog();
                    if (confirmBox.IsOk)
                    {
                        if (confirmBox.okButton.Content.ToString().Equals("调拨计量"))
                        {
                            #region 日志
                            LogModel log = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "坐席_任务处理窗体",
                                Level = LogConstParam.LogLevel_Info,
                                Msg = "获取业务信息时系统提示：" + stopTaskReason + "用户选择调拨计量",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            #endregion
                            BullInfo newB = new BullInfo();
                            newB.carno = this.carNo;
                            newB.icid = this.icId;
                            newB.rfidid = this.rfidId;
                            newB.icid = this.icId;
                            HandleTask.BullInfo = InfoExchange.Clone<BullInfo>(newB);
                        }
                        else
                        {
                            //logH.SaveLog("获取业务信息时系统提示：" + stopTaskReason + "用户选择终止计量");
                            #region 日志
                            LogModel log = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "坐席_任务处理窗体",
                                Level = LogConstParam.LogLevel_Info,
                                Msg = "获取业务信息时系统提示：" + stopTaskReason + "用户选择终止计量",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            #endregion
                            StopTask();
                        }
                        return;
                    }
                    else//处理单独计皮的情况…… 2016-4-1 09:01:46……
                    {
                        if (confirmBox.isSystermClose)
                        {
                            return;
                        }
                        #region 日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "坐席_任务处理窗体",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = "获取业务信息时系统提示：" + stopTaskReason + "用户选择继续计量，转为无业务单独计皮",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        //处理无计量申请单的情况
                        DoNoMeasureApply();
                        return;
                    }
                }
                this.originGross = this.HandleTask.BullInfo.gross == null ? 0M : this.HandleTask.BullInfo.gross.Value;
                this.originTare = this.HandleTask.BullInfo.tare == null ? 0M : this.HandleTask.BullInfo.tare.Value;
                string rollMessage = string.Empty;
                flagsList = GetflagsList(this.HandleTask.ServiceModel, ref rollMessage); //获取到提示信息
                if (rollMessage.Contains("皮重已超期"))
                {
                    SendNoticToClient(this.carNo + "皮重已经超期");
                }
                if (this.HandleTask.ServiceModel.mfunc == 3)//禁止计量  0代表允许计量  1 代表进行提示 2 代表进行选择 3 代表终止 lt 2016-2-2 17:01:38……
                {
                    //弹出确认框，是否终止计量
                    //是:调用终止计量接口;本窗体关闭
                    //否：手动处理计量任务
                    //将终止原因提示出来……lt 2016-2-2 17:12:15……
                    this.ShowBusy = false;
                    ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", rollMessage + " 服务器禁止计量", true, false, "终止计量", "继续计量");
                    SetShowListConfirmMessageBox(confirmBox, flagsList);
                    confirmBox.ShowDialog();
                    if (confirmBox.IsOk)
                    {
                        #region 日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "坐席_任务处理窗体",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = "获取业务信息时系统提示：" + rollMessage + logMustInfo + "用户选择终止计量",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        stopTaskReason = rollMessage;
                        StopTask();
                        return;
                    }
                    else
                    {
                        if (confirmBox.isSystermClose)
                        {
                            return;
                        }
                    }
                }
                else if (this.HandleTask.ServiceModel.mfunc == 2 && this.HandleTask.ServiceModel.flags.Count > 0)//选择情况
                {
                    this.ShowBusy = false;
                    ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", rollMessage, true, true, "终止计量", "继续计量");
                    SetShowListConfirmMessageBox(confirmBox, flagsList);
                    confirmBox.ShowDialog();
                    if (confirmBox.IsOk)
                    {
                        #region 日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "坐席_任务处理窗体",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = "获取业务信息时系统提示：" + rollMessage + logMustInfo + "用户选择终止计量",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        stopTaskReason = rollMessage;
                        StopTask();
                        return;
                    }
                    else
                    {
                        if (confirmBox.isSystermClose)
                        {
                            return;
                        }
                        #region 日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "坐席_任务处理窗体",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = "获取业务信息时系统提示：" + rollMessage + logMustInfo + "用户选择继续计量",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        DoTareTimeOut(rollMessage, confirmBox.isUseTimeOutTare);
                    }
                }
                else if (this.HandleTask.ServiceModel.mfunc == 1)//1的时候进行提示……
                {
                    //this.RollMsg = rollMessage;
                    //不使用滚动 改为弹框……lt 2016-2-2 17:50:53
                    this.ShowBusy = false;
                    ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", rollMessage, true, false, "继续计量", "继续计量");
                    SetShowListConfirmMessageBox(confirmBox, flagsList);
                    confirmBox.ShowDialog();
                    if (confirmBox.isSystermClose)
                    {
                        return;
                    }
                    //logH.SaveLog("获取业务信息时系统提示：" + rollMessage + logMustInfo + "用户选择继续计量");
                    #region 日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "获取业务信息时系统提示：" + rollMessage + logMustInfo + "用户选择继续计量",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    DoTareTimeOut(rollMessage, confirmBox.isUseTimeOutTare);
                }
                if (this.HandleTask.BullInfo.measurestate == "G")
                {
                    this.IsGrossWeight = true;
                }
                else if (this.HandleTask.BullInfo.measurestate == "T")
                {
                    this.IsTareWeight = true;
                }
                if (IsViewEnable)
                {
                    if (this.SeatMeasureType.Equals("自动计量") && string.IsNullOrEmpty(rollMessage))
                    {
                        this.TimeCountVisibility = Visibility.Visible;
                    }
                    try
                    {
                        GetImages(this.HandleTask.BullInfo.matchid, this.HandleTask.ClientCode, this.HandleTask.ClientName);
                    }
                    catch (Exception ex)
                    {
                        #region 日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "坐席任务处理窗体_从服务获取图片信息",
                            Level = LogConstParam.LogLevel_Error,
                            Msg = "从服务获取图片信息时异常:" + ex.Message,
                            Origin = LoginUser.Role.Name,
                            OperateUserName = LoginUser.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                }
                SendBusinessInfosToTaskServer();
                ShowHistoryTareMethod();//获取上一次的皮重信息…… 2016-3-15 09:02:12……
            }
            OperateVoiceTalk();
            #region 日志
            long memorySize = GetMemoryAmount();
            LogModel log1 = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "坐席任务处理窗体_NextBusinessInit",
                Level = LogConstParam.LogLevel_Error,
                Msg = "当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)",
                Origin = LoginUser.Role.Name,
                OperateUserName = LoginUser.Name,
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
            #endregion
            //this.IsOpenVideo = openVidoTemp;
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<bool>(true, "OpenFirstVideo");
            this.ShowBusy = false;
        }

        /// <summary>
        /// 获取任务对象中的卡号 ic卡号
        /// </summary>
        /// <returns></returns>
        private string GetICCard()
        {
            string icCard = "";
            try
            {
                if (this.HandleTask != null && !string.IsNullOrEmpty(this.HandleTask.IcId))
                {
                    icCard = this.HandleTask.IcId;
                }
                else if (this.HandleTask.BullInfo != null && !string.IsNullOrEmpty(this.HandleTask.BullInfo.icid))
                {
                    icCard = this.HandleTask.BullInfo.icid;
                }
                else if (this.HandleTask.BullInfos != null && this.HandleTask.BullInfos.Count > 0 && !string.IsNullOrEmpty(this.HandleTask.BullInfos.First().icid))
                {
                    icCard = this.HandleTask.BullInfos[0].icid;
                }
                else if (this.HandleTask.ServiceModel != null && this.HandleTask.ServiceModel.data != null && !string.IsNullOrEmpty(this.HandleTask.ServiceModel.data.icId))
                {
                    icCard = this.HandleTask.ServiceModel.data.icId;
                }
                else if (this.HandleTask.ServiceModel != null && this.HandleTask.ServiceModel.rows != null && this.HandleTask.ServiceModel.rows.Count > 0)
                {
                    icCard = this.HandleTask.ServiceModel.rows[0].icid;
                }
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用GetICCard方法获取到卡号:" + icCard,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "调用GetICCard方法获取卡号异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return icCard;
        }

        /// <summary>
        /// 重新获取业务信息包括验证
        /// </summary>
        /// <param name="bl"></param>
        private void GetOneMatchidInfos(BullInfo bl)
        {
            try
            {
                bl.rfid = rfidId;
                string bInfoStr = JsonConvert.SerializeObject(bl);
                string serviceUrl = ConfigurationManager.AppSettings["moreMeasureInfo"].ToString();
                HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, bInfoStr, "");
                string strResult = ComHelpClass.ResponseSynStr(request);
                MeasureServiceModel mServiceModel = new MeasureServiceModel();
                mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_重新获取业务信息包括验证",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务器一车多货读取计量显示数据成功!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = strResult,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.HandleTask.ServiceModel = mServiceModel;
                this.HandleTask.BullInfos = mServiceModel != null ? mServiceModel.rows : null;
                this.HandleTask.ServiceModel.rows.Clear();
                this.HandleTask.ServiceModel.rows.Add(this.HandleTask.BullInfo);//同步终端信息
                historyTareLst.Clear();
                LastTare = "0";
                isGetHistoryTare = false;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    NextBusinessInit();
                }));
            }
            catch (Exception ex)
            {
                this.ShowBusy = false;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "获取一车多货计量显示信息失败，原因:" + ex.Message, true, false, "确定", "取消");
                    mb.ShowDialog();
                }));
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_重新获取业务信息包括验证",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务读取计量显示数据失败moreMeasureInfo！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 语音对讲异常信息
        /// </summary>
        /// <param name="msg"></param>
        void audioController_OnShowErrMsg(string msg)
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_任务处理窗体_语音对讲异常信息",
                Level = LogConstParam.LogLevel_Error,
                Msg = "语音对讲异常:" + msg,
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        /// <summary>
        /// 从服务获取图片信息
        /// </summary>
        /// <param name="matchid">过磅单号</param>
        /// <param name="equcode">称点code</param>
        /// <param name="equname">称点名称</param>
        private void GetImages(string matchid, string equcode, string equname)
        {
            this.ShowBusy = true;
            this.BusyText = "加载图片中...";
            if (!string.IsNullOrEmpty(matchid))
            {
                string serviceUrl = ConfigurationManager.AppSettings["getMeasurePhoto"].ToString();
                var param = new
                {
                    matchid = matchid,
                    photo = "",
                    measuretype = "",
                    equcode = equcode,
                    equname = ""
                };
                var jsonStr = "[" + JsonConvert.SerializeObject(param) + "]";
                string url = string.Format(serviceUrl, jsonStr);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_从服务获取图片信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "开始从服务读取图片信息!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "URL", ParamValue = url } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(url, 10);
                request.BeginGetResponse(new AsyncCallback(GetMeasurePhotoCallback), request);
            }
            else
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_从服务获取图片信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "因matchid为空，无法获取图片信息!" + "称点编号:" + equcode + " 称点名称:" + equname,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 获取图片信息回调方法
        /// </summary>
        /// <param name="asyc"></param>
        private void GetMeasurePhotoCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                List<PictureModel> pictureInfos = InfoExchange.DeConvert(typeof(List<PictureModel>), strResult) as List<PictureModel>;
                this.Pictures = null;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_从服务获取图片信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务获取图片信息成功!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    Data = pictureInfos,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                System.Windows.Application.Current.Dispatcher.Invoke(new System.Threading.ThreadStart(() =>
                {
                    GC.Collect();
                    try
                    {
                        if (pictureInfos != null && pictureInfos.Count > 0)
                        {
                            var imgs = pictureInfos.Where(r => !string.IsNullOrEmpty(r.photo));
                            if (imgs != null && imgs.Count() > 0)
                            {
                                #region 内存日志
                                long memorySize = GetMemoryAmount();
                                LogModel log1 = new LogModel()
                                {
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    FunctionName = "坐席任务处理窗体_下载图片前",
                                    Msg = "下载图片前,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                                #endregion
                                DownloadImags(pictureInfos);
                                #region 内存日志
                                memorySize = GetMemoryAmount();
                                LogModel log3 = new LogModel()
                                {
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    FunctionName = "坐席任务处理窗体_通过FTP下载图片后",
                                    Msg = "通过FTP下载图片后,当前线程使用内存大小:" + memorySize + "字节(" + Math.Round((double)memorySize / (1024 * 1024), 2) + "M)"
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
                                #endregion
                            }
                        }
                    }
                    catch
                    { }
                    this.Pictures = pictureInfos;
                }));
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_获取图片信息回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务读取图片信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //this.ShowBusy = false;
                //ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "获取图片失败,原因:" + ex.Message, true, false, "确定", "取消");
                //mb.ShowDialog();
            }
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="list"></param>
        private void DownloadImags(List<PictureModel> list)
        {
            try
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_加载图片",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "开始加载图片",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                ComHelpClass cHelp = new ComHelpClass();
                cHelp.DownloadImags(list);
                #region 日志
                LogModel log11 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_加载图片",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "加载图片成功",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log11));
                #endregion
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_加载图片",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "加载图片失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }


        /// <summary>
        /// 计时到间隔时间点触发的方法
        /// </summary>
        void timer_Tick(object sender, EventArgs e)
        {
            TimerStop();
            if (this.HandleTask != null && this.HandleTask.ServiceModel != null)
            {
                var isHwCtrlList = (from r in this.HandleTask.ServiceModel.hardwarectrl where r.name.Equals("红外对射") select r).ToList();
                if (isHwCtrlList.Count > 0 && isHwCtrlList.First().check.Equals("强制启用"))//服务指定坐席做红外对射判断
                {
                    if (LeftLine == Visibility.Visible || RightLine == Visibility.Visible)
                    {
                        TimerStart();
                        return;
                    }
                }
            }
            if (WeightList.Count > 0 && CheckWeight())
            {
                saveWeightServiceInfo();
            }
            else
            {
                this.WeightTimeCount--;
                if (this.WeightTimeCount != 0)
                {
                    TimerStart();
                }
                else
                {
                    this.RollMsg = "【取数次数】内未获得稳定重量，请手动处理!";
                }
            }
        }

        /// <summary>
        /// 发送打印票据命令给任务服务器
        /// </summary>
        private void SendPrintTicket()
        {
            PrintInfo pm = new PrintInfo()
            {
                carno = this.carNo,
                clientcode = this.HandleTask.ClientCode,
                clientname = this.HandleTask.ClientName,
                matchid = this.HandleTask.BullInfo.matchid,
                opcode = LoginUser.Code,
                opname = LoginUser.Name,
                printtype = "正常"
            };
            int unm = CommonMethod.CommonMethod.GetRandom();
            var para = new
            {
                clientid = this.HandleTask.ClientId,
                cmd = ParamCmd.Supplement,
                msg = pm,
                msgid = unm
            };
            string paraJsonStr = JsonConvert.SerializeObject(para);
            SocketCls.Emit(SeatSendCmdEnum.cmd2client, paraJsonStr);
            waitPrintReusltTimer.Start();//等待打印结果的计时器开始计时
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_任务处理窗体_发送打印票据命令给任务服务器",
                Level = LogConstParam.LogLevel_Info,
                Msg = "发送保存完成打印命令：" + SeatSendCmdEnum.cmd2client + "给任务服务器",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                Data = para,
                IsDataValid = LogConstParam.DataValid_Ok,
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            #region 写计时器开始的日志
            LogModel timerLog = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "坐席_任务处理窗体_发送打印票据命令给任务服务器",
                Level = LogConstParam.LogLevel_Info,
                Msg = "等待打印结果计时器开始计时,时间为:" + waitPrintReusltTimer.Timeout + "秒",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog));
            #endregion
            this.ShowBusy = false;
        }

        /// <summary>
        /// 检查称重是否合格
        /// </summary>
        private bool CheckWeight()
        {
            #region 写日志
            LogModel logW = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "坐席_任务处理窗体_检查称重是否合格",
                Level = LogConstParam.LogLevel_Info,
                Msg = "检查称重是否合格,IsGetWeight：" + IsGetWeight,
                Origin = "汽车衡_" + LoginUser.Role.Name,
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logW));
            #endregion
            bool isOk = false;
            if (WeightList.Count > 0 && (WeightList.Max() - WeightList.Min()) <= this.weightAllowance)
            {
                isOk = true;
            }
            return isOk;
        }

        /// <summary>
        /// 发送业务数据给任务服务器(用于坐席和称点的数据同步)
        /// </summary>
        public void SendBusinessInfosToTaskServer()
        {
            if (this.HandleTask == null)
            {
                return;
            }
            TaskModel tm = InfoExchange.Clone<TaskModel>(this.HandleTask);
            tm.mores = this.HandleTask.ServiceModel.mores;
            tm.BullInfos = null;
            tm.ServiceModel = null;
            int unm = CommonMethod.CommonMethod.GetRandom();
            var para = new
            {
                clientid = this.HandleTask.ClientId,
                cmd = ParamCmd.Update_Seat_To_Measure,
                msg = tm,
                msgid = unm
            };
            string paraJsonStr = JsonConvert.SerializeObject(para);
            SocketCls.Emit(SeatSendCmdEnum.cmd2client, paraJsonStr);
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_任务处理窗体_发送业务数据给任务服务器",
                Level = LogConstParam.LogLevel_Info,
                Msg = "发送业务数据同步命令：" + SeatSendCmdEnum.cmd2client + "给任务服务器",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                Data = para,
                IsDataValid = LogConstParam.DataValid_Ok,
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        /// <summary>
        /// 正常结束任务
        /// </summary>
        public void EndTask()
        {
            string taskJsonStr = InfoExchange.ConvertToJsonIgnoreRef(this.HandleTask.BullInfo);
            TaskModel tm = InfoExchange.Clone<TaskModel>(this.HandleTask);// new TaskModel() { BullInfo = this.HandleTask.BullInfo };
            tm.ServiceModel = null;
            tm.BullInfos = null;
            EndTaskModel = new SendReplyModel()
            {
                clientid = this.HandleTask.ClientId,
                matchid = (this.HandleTask.BullInfo == null || string.IsNullOrEmpty(this.HandleTask.BullInfo.matchid))? curMachid : this.HandleTask.BullInfo.matchid,
                data = tm,
                result = 1
            };
            this.ShowBusy = false;
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.HandleTask = null;
                HandleResult = "正常结束";
                if (this.SeatMeasureType.Equals("自动计量"))
                {
                    this.RollMsg = "自动处理完成,窗体即将关闭!";
                }
                IsViewEnable = false;
            }));
        }

        /// <summary>
        /// 终止任务
        /// </summary>
        private void StopTask()
        {
            TaskModel tm = InfoExchange.Clone<TaskModel>(this.HandleTask);
            tm.ErrorMsg = "计量失败：" + stopTaskReason.Replace("\r\n", "!") + "请拨打大厅电话";//终端显示终止计量原因  "请车下称"改为“请拨打大厅电话” 2016-3-21 10:07:33……
            tm.BullInfos = null;
            tm.ServiceModel = null;
            EndTaskModel = new SendReplyModel()
            {
                clientid = this.HandleTask.ClientId,
                matchid = this.HandleTask.BullInfo == null ? "" : this.HandleTask.BullInfo.matchid,
                data = tm,
                result = 0
            };
            try
            {
                this.HandleTask = null;
                HandleResult = "终止计量";
                IsViewEnable = false;
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_终止任务",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "执行任务终止方法时发生异常,原因:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    Data = tm,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatlistenCmdEnum.endtask } },
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        public WeighterClientModel weighterClient;

        /// <summary>
        /// 等待打印结果计时器时间到触发的事件
        /// </summary>
        /// <param name="userdata"></param>
        private void waitPrintReusltTimer_TimeOver(object userdata)
        {
            try
            {
                #region 写计时器计时结束的日志
                //LogModel timerLog = new LogModel()
                //{
                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    Msg = "等待打印结果计时器计时时间到,isShowChildFormMsg(当前信息框是否已经弹出)值为:" + isShowChildFormMsg.ToString(),
                //    Origin = LoginUser.Role.Name,
                //    OperateUserName = LoginUser.Name
                //};
                //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog));
                #endregion

                if (!isShowChildFormMsg)
                {
                    bool isRePrint = false;//是否补打
                    isShowChildFormMsg = true;
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", "保存成功,等待" + this.waitPrintReusltTimer.Timeout + "秒依然无打印结果反馈", true, true, "补打", "任务结束");
                        confirmBox.ShowDialog();
                        isRePrint = confirmBox.IsOk;
                        confirmBox.Close();
                    }));
                    #region 写计时器开始的日志
                    //LogModel timerLog1 = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Msg = "超时情况下调用了弹出信息框",
                    //    Origin = LoginUser.Role.Name,
                    //    OperateUserName = LoginUser.Name
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog1));
                    #endregion
                    if (isRePrint)
                    {
                        SendPrintTicket();
                    }
                    else
                    {
                        EndTask();
                    }
                    isShowChildFormMsg = false;
                }
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体_打印结果计时器计时时间到",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "打印结果计时器计时时间到处理时异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 弹出的信息子窗体是否已经打开
        /// </summary>
        bool isShowChildFormMsg = false;
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
                    weighterClient = JsonConvert.DeserializeObject<WeighterClientModel>(realDataJsonStr);
                    if (weighterClient != null && this.HandleTask != null && this.HandleTask.ClientId == weighterClient.ClientId)
                    {
                        if (weighterClient.EquTag == null || (!weighterClient.EquTag.Equals("P") && !weighterClient.EquTag.Equals("RFID")))
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                SetRealData(weighterClient);
                            }));
                        }
                        else if (weighterClient.EquTag != null && weighterClient.EquTag.Equals("RFID"))
                        {
                            this.rfidId = string.IsNullOrEmpty(weighterClient.RfidStrs) ? weighterClient.RfidStrs : weighterClient.RfidStrs.Substring(0, weighterClient.RfidStrs.Length - 1);
                        }
                        else if (weighterClient.EquTag != null && weighterClient.EquTag.Equals("P") && !isShowChildFormMsg)
                        {
                            #region 写打印结果的日志
                            LogModel timerLog = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "坐席_任务处理窗体_获取称点的实时数据",
                                Level = LogConstParam.LogLevel_Info,
                                Msg = "在实时数据中获取到打印结果信息。",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog));
                            #endregion
                            waitPrintReusltTimer.Stop();
                            string printError = weighterClient.PrintState;
                            if (!string.IsNullOrEmpty(printError))
                            {
                                #region 写打印结果的日志
                                //LogModel timerLog1 = new LogModel()
                                //{
                                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                //    Msg = "返回的打印结果信息:" + printError + "isShowChildFormMsg(信息子窗体是否已经打开)值为:" + isShowChildFormMsg.ToString(),
                                //    Origin = LoginUser.Role.Name,
                                //    OperateUserName = LoginUser.Name
                                //};
                                //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog1));
                                #endregion
                                bool isRePrint = false;//是否补打
                                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    isShowChildFormMsg = true;
                                    //打印失败业务处理
                                    ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", "数据保存成功,打印失败,原因:" + printError, true, true, "补打", "任务结束");
                                    confirmBox.ShowDialog();
                                    isRePrint = confirmBox.IsOk;
                                    confirmBox.Close();
                                }));
                                if (isRePrint)
                                {
                                    SendPrintTicket();
                                }
                                else
                                {
                                    EndTask();
                                }
                                isShowChildFormMsg = false;
                            }
                            else
                            {
                                #region 写打印结果的日志
                                LogModel timerLog1 = new LogModel()
                                {
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Direction = LogConstParam.Directions_In,
                                    FunctionName = "坐席_任务处理窗体_获取称点的实时数据",
                                    Level = LogConstParam.LogLevel_Info,
                                    Msg = "数据保存成功,打印成功",
                                    Origin = "汽车衡_" + LoginUser.Role.Name,
                                    OperateUserName = LoginUser.Name
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog1));
                                #endregion
                                isShowChildFormMsg = true;
                                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", "保存成功,打印成功!", true, false, "确定", "", true, 3);
                                    confirmBox.ShowDialog();
                                    confirmBox.Close();
                                }));
                                isShowChildFormMsg = false;
                                EndTask();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体_处理实时数据",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "处理实时数据时异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 设置实时数据
        /// </summary>
        /// <param name="weighterClient"></param>
        public void SetRealData(WeighterClientModel weighterClient)
        {
            #region 红绿灯赋值
            if (weighterClient.LeftLightState == LeftLightStates.Red)
            {
                Light = new SolidColorBrush(Colors.Red);
            }
            else if (weighterClient.LeftLightState == LeftLightStates.Green)
            {
                Light = new SolidColorBrush(Colors.Green);
            }
            else if (weighterClient.LeftLightState == LeftLightStates.None)
            {
                Light = new SolidColorBrush(Colors.Gray);
            }
            #endregion

            #region 左右红外对射
            this.LeftLine = weighterClient.LeftLine;
            this.RightLine = weighterClient.RightLine;
            #endregion
            if (IsGetWeight)
            {
                var wList = this.WeightList.Where(r => r == weighterClient.Weight);
                if (wList.Count() == 0)
                {
                    this.WeightList.Add(weighterClient.Weight);
                }
            }
            if (!isClickSave)//准备保存重量时 数据不再变化……
            {
                string wStr = (weighterClient.Weight * weightDot).ToString("#0.00");
                decimal dWeight = weighterClient.Weight;
                if (IsGrossWeight)
                {
                    if (this.HandleTask != null && this.HandleTask.BullInfo != null)
                    {
                        this.HandleTask.BullInfo.gross = dWeight;
                    }
                }
                else
                {
                    if (this.HandleTask != null && this.HandleTask.BullInfo != null)
                    {
                        this.HandleTask.BullInfo.gross = this.originGross;
                    }
                }
                if (IsTareWeight)
                {
                    if (this.HandleTask != null && this.HandleTask.BullInfo != null)
                    {
                        this.HandleTask.BullInfo.tare = dWeight;
                    }
                }
                else
                {
                    if (this.HandleTask != null && this.HandleTask.BullInfo != null)
                    {
                        if (!AutoTareIsCheck)
                        {
                            this.HandleTask.BullInfo.tare = this.originTare;
                        }
                    }
                }
            }
            //lt 2016-1-28 13:51:42  去掉两位格式化  取数为千克数 不存在小数问题……
            this.CurWeight = weighterClient.Weight.ToString();
        }

        #endregion
        #region Socket回调事件
        public void SocketCls_listenEvent(object sender, CallBackEventArgs e)
        {
            #region 写监听到的任务服务器的日志
            //LogModel timerLog = new LogModel()
            //{
            //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //    Msg = "得到任务服务器命令:【" + e.EventName + "】",
            //    Origin = LoginUser.Role.Name,
            //    OperateUserName = LoginUser.Name
            //};
            //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog));
            #endregion
            switch (e.EventName)
            {
                case "realData":
                    GetRealData(e.Message);
                    break;
                case "relogin"://重新登陆
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        ConfirmMessageBox mb = new ConfirmMessageBox("提示", "与服务器断开连接.", true, false, "确定", "取消");
                        mb.ShowDialog();
                        if (mb.IsOk)
                        {
                            MessageWindowEvent("DisconnectTaskServer");
                        }
                    }));
                    break;
            }
        }
        #endregion

        private void MessageWindowEvent(string Parameter)
        {
            if (Parameter == "StopTask")//终止任务触发
            {
                stopTaskReason = "坐席终止计量";
                #region 坐席终止计量的日志
                LogModel timerLog = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_终止计量",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "系统提示：是否终止计量，用户选择终止计量：" + logMustInfo,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog));
                #endregion
                StopTask();
            }
            if (Parameter == "DisconnectTaskServer")
            {
                this.HandleTask = null;
                HandleResult = "与任务服务器断开连接";
                IsViewEnable = false;
            }
        }
        #region 回调服务

        /// <summary>
        /// 保存业务信息.
        /// </summary>
        public void saveWeightServiceInfo()
        {
            string rtError = string.Empty;
            logMustInfo = "过磅单号：" + this.HandleTask.BullInfo.matchid + "  车号：" + this.HandleTask.BullInfo.carno;
            if (Convert.ToDecimal(this.CurWeight) <= 0)//当前重量变为0或者负数时 提示……2016-2-29 14:36:32
            {
                rtError = "当前秤体重量为：" + this.CurWeight;
                ConfirmMessageBox cM = new ConfirmMessageBox("系统提示", rtError, true, true, "终止计量", "继续计量");
                cM.ShowDialog();
                if (cM.IsOk)
                {
                    //logH.SaveLog("点击确认重量：" + rtError + logMustInfo + "  , 用户选择：终止计量");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_保存业务信息",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "点击确认重量：" + rtError + logMustInfo + "  , 用户选择：终止计量",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    stopTaskReason = rtError;
                    StopTask();
                    return;
                }
                //logH.SaveLog("点击确认重量：" + rtError + logMustInfo + ", 用户选择：继续计量");
                #region 写日志
                LogModel logContinue = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_保存业务信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击确认重量：" + rtError + logMustInfo + ", 用户选择：继续计量",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logContinue));
                #endregion
            }
            //判断红外是否 遮挡……lt 2016-2-18 16:03:41    
            if (CheckHongWaiDuiShe(out rtError))
            {
                //是否允许保存计量信息;1:允许保存;2:不允许保存
                string isLineMeasureMark = ConfigurationManager.AppSettings["IsLineMeasure"].ToString();
                ConfirmMessageBox cM = null;
                if (isLineMeasureMark == "1")
                {
                    cM = new ConfirmMessageBox("系统提示", rtError, true, true, "终止计量", "继续计量");
                }
                else
                {
                    cM = new ConfirmMessageBox("系统提示", rtError, true, false, "终止计量", "继续计量");
                }
                if (cM.IsOk)
                {

                    #region 写日志
                    LogModel logStop = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_保存业务信息",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "点击确认重量：" + rtError + logMustInfo + ", 用户选择：终止计量",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logStop));
                    #endregion
                    stopTaskReason = rtError;
                    StopTask();
                    return;
                }
                #region 写日志
                LogModel logContinue = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_保存业务信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击确认重量：" + rtError + logMustInfo + ", 用户选择：继续计量",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logContinue));
                #endregion
            }
            if ((this.HandleTask.BullInfo.suttle == null || this.HandleTask.BullInfo.suttle <= 200) && this.HandleTask.BullInfo.gross > 0 && this.HandleTask.BullInfo.tare > 0)//出净时,净重小于等于200时进行提示  update 20170207 by wangchao
            {
                rtError = "净重为：" + Convert.ToDecimal((this.HandleTask.BullInfo.gross - this.HandleTask.BullInfo.tare - this.HandleTask.BullInfo.deduction) * weightKg).ToString("F2");
                ConfirmMessageBox cM = new ConfirmMessageBox("系统提示", rtError, true, true, "终止计量", "继续计量");
                cM.ShowDialog();
                if (cM.IsOk)
                {
                    #region 写日志
                    LogModel logStop = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_保存业务信息",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "点击确认重量：" + rtError + logMustInfo + ", 用户选择：终止计量",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logStop));
                    #endregion
                    stopTaskReason = rtError;
                    StopTask();
                    return;
                }
                #region 写日志
                LogModel logContinue = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_保存业务信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击确认重量：" + rtError + logMustInfo + ", 用户选择：继续计量",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logContinue));
                #endregion
            }

            if (!CheckWeight())
            {
                rtError = "数据不稳，禁止取数,取数次数：" + WeightList.Count;
                ConfirmMessageBox cM = new ConfirmMessageBox("系统提示", rtError, true, true, "终止计量", "继续计量");
                cM.ShowDialog();
                if (cM.IsOk)
                {
                    #region 写日志
                    LogModel logStop = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_保存业务信息",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "点击确认重量：" + rtError + logMustInfo + ", 用户选择：终止计量",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logStop));
                    #endregion
                    stopTaskReason = rtError;
                    StopTask();
                    return;
                }
                #region 写日志
                LogModel logContinue = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_保存业务信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击确认重量：" + rtError + logMustInfo + ", 用户选择：继续计量",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(logContinue));
                #endregion
            }
            SetBullInfo();
            string bInfoStr = JsonConvert.SerializeObject(this.HandleTask.BullInfo);
            bool isSave = CheckIsAllowSave(bInfoStr, true);//首先调用验证判断是不是允许保存……lt 2016-2-24 09:11:44……
            if (isSave)
            {
                IsGetWeight = false;
                this.WeightList.Clear();
                string serviceUrl = ConfigurationManager.AppSettings["saveMeasureInfo"].ToString();
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_保存业务信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用服务存储业务信息(确定重量)!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    Data = this.HandleTask.BullInfo,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    OperateUserName = LoginUser.Name,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "Url", ParamValue = serviceUrl } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, bInfoStr, "");
                this.ShowBusy = true;
                this.BusyText = "存储业务信息中";
                request.BeginGetResponse(new AsyncCallback(saveWeightCallback), request);
            }
            else
            {
                isClickSave = false;
            }
        }
        /// <summary>
        /// 保存信息回调方法
        /// </summary>
        /// <param name="asyc"></param>
        private void saveWeightCallback(IAsyncResult asyc)
        {
            try
            {
                this.ShowBusy = false;
                this.BusyText = "";
                string strResult = ComHelpClass.ResponseStr(asyc);
                MeasureServiceModel getServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用服务存储业务信息返回：" + strResult,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    Data = this.HandleTask.BullInfo,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                if (getServiceModel != null && getServiceModel.success)
                {
                    #region 写日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_OutIn,
                        FunctionName = "坐席_任务处理窗体_记录反馈的machid",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "记录服务反馈的machid",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        Data = getServiceModel.more,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                    try
                    {
                        curMachid = getServiceModel.more == null ? "" : getServiceModel.more.ToString();
                    }
                    catch (Exception ex)
                    {
                        #region 写日志
                        LogModel log2 = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_OutIn,
                            FunctionName = "坐席_任务处理窗体_记录反馈的machid",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = "转换more得到machid时异常:" + ex.Message,
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                            Data = getServiceModel.more,
                            OperateUserName = LoginUser.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                        #endregion
                    }

                    this.ShowBusy = false;
                    var isPrintTicketCtrlList = (from r in this.HandleTask.ServiceModel.hardwarectrl where r.name.Equals("打印磅单") select r).ToList();
                    if (isPrintTicketCtrlList.Count > 0 && isPrintTicketCtrlList.First().check.Equals("强制启用"))//服务指定坐席是否打印"磅"的单据
                    {
                        SendPrintTicket();
                    }
                    else
                    {
                        //不打印票据情况下的后续业务处理
                        ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", "存储业务信息成功！", true, false, "确定", "", true, 3);
                        confirmBox.ShowDialog();
                        EndTask();
                    }
                }
                else
                {
                    isClickSave = false;
                    IsGetWeight = true;
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        #region 写日志
                        LogModel SaveSaveLog = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = "保存重量：" + logMustInfo + "毛重：" + this.handleTask.BullInfo.gross + "  皮重：" + this.handleTask.BullInfo.tare + " 扣重：" + this.handleTask.BullInfo.deduction + " 系统提示：" + "调用服务存储业务信息失败,原因:" + getServiceModel.msg,
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(SaveSaveLog));
                        #endregion
                        this.ShowBusy = false;
                        string msg = getServiceModel.msg.Contains("The operation has timed out") ? getServiceModel.msg.Replace("The operation has timed out", "操作超时") : getServiceModel.msg;
                        ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "存储业务信息失败,原因:" + msg, true, false, "确定", "取消");
                        mb.ShowDialog();
                    }));
                }
            }
            catch (Exception ex)
            {
                isClickSave = false;
                IsGetWeight = true;
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.ShowBusy = false;
                    #region 写日志
                    LogModel SaveSaveLog = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "保存重量：" + logMustInfo + "毛重：" + this.handleTask.BullInfo.gross + "  皮重：" + this.handleTask.BullInfo.tare + " 扣重：" + this.handleTask.BullInfo.deduction + " 系统提示：" + "调用服务存储业务信息失败,系统异常原因:" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(SaveSaveLog));
                    #endregion
                    ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "存储业务信息失败,原因:" + ex.Message, true, false, "确定", "取消");
                    mb.ShowDialog();
                }));
            }
        }
        #endregion
        #region 私有处理方法
        /// <summary>
        /// 处理界面显示小数点的问题
        /// </summary>
        private void DoDecimalShowInfos()
        {
            try
            {
                this.HandleTask.BullInfo.grossb = DoDecimal(this.HandleTask.BullInfo.grossb);
                this.HandleTask.BullInfo.tareb = DoDecimal(this.HandleTask.BullInfo.tareb);
                this.HandleTask.BullInfo.suttleb = DoDecimal(this.HandleTask.BullInfo.suttleb);
                this.HandleTask.BullInfo.gross = DoDecimal(this.HandleTask.BullInfo.gross);
                this.HandleTask.BullInfo.tare = DoDecimal(this.HandleTask.BullInfo.tare);
                this.HandleTask.BullInfo.suttle = DoDecimal(this.HandleTask.BullInfo.suttle);
                this.HandleTask.BullInfo.deduction = DoDecimal(this.HandleTask.BullInfo.deduction);
                DoShowWeightInfo(true);
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席任务处理窗体_DoDecimalShowInfos",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "处理界面显示小数点的问题时异常,原因:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        private decimal DoDecimal(decimal? inInfos)
        {
            decimal rtInfo = 0;
            try
            {
                if (inInfos != null)
                {
                    rtInfo = Convert.ToDecimal((Convert.ToDecimal(inInfos.ToString()).ToString("F0")));
                }
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体_DoDecimal",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "各重量转换时异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return rtInfo;
        }
        /// <summary>
        /// 查看历史皮重信息 lt 2016-2-18 09:07:37……
        /// </summary>
        private void ShowHistoryTareMethod()
        {
            if (this.HandleTask != null && this.HandleTask.BullInfo != null && !string.IsNullOrEmpty(this.HandleTask.BullInfo.carno))
            {
                this.carNo = this.HandleTask.BullInfo.carno;
            }
            else
            {
                ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "车号不允许为空", true, false, "确定", "取消");
                mb.ShowDialog();
                return;
            }
            if (historyTareLst.Count > 0)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ShowHistoryTareView view = new ShowHistoryTareView(historyTareLst);
                    view.ShowDialog();
                }));
            }
            else
            {
                if (this.HandleTask != null)
                {
                    if (!isGetHistoryTare)//第一次获取
                    {
                        string serviceUrl = ConfigurationManager.AppSettings["getCarHistoryTare"].ToString().Replace('$', '&');
                        string getUrl = string.Format(serviceUrl, carNo, this.HandleTask.BullInfo.tare);
                        HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                        request.BeginGetResponse(new AsyncCallback(GetCarHistoryTareCallback), request);
                    }
                    else
                    {
                        if (historyTareLst.Count == 0)//若第一次获取为空 再获取一次
                        {
                            string serviceUrl = ConfigurationManager.AppSettings["getCarHistoryTare"].ToString().Replace('$', '&');
                            string getUrl = string.Format(serviceUrl, carNo, this.HandleTask.BullInfo.tare);
                            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                            request.BeginGetResponse(new AsyncCallback(GetCarHistoryTareCallback), request);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 往秤体发送通知 2016-4-8 00:27:48……
        /// </summary>
        private void SendMSGMethod()
        {
            if (this.HandleTask != null && !string.IsNullOrEmpty(this.HandleTask.ClientCode))
            {
                TaskHandleSendNoticeView tSend = new TaskHandleSendNoticeView(this.HandleTask.ClientName, this.HandleTask.ClientCode);
                tSend.ShowDialog();
            }
        }
        /// <summary>
        /// 异步调用返回值
        /// </summary>
        /// <param name="asyc"></param>
        private void GetCarHistoryTareCallback(IAsyncResult asyc)
        {
            try
            {
                MeasureServiceModel mServiceModel;
                string strResult = ComHelpClass.ResponseStr(asyc);
                mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                historyTareLst.Clear();
                historyTareLst = mServiceModel.rows;
                if (historyTareLst.Count > 0)
                {
                    if (LastTare.Equals("0") && historyTareLst[0].tare != null)
                    {
                        LastTare = (Convert.ToDecimal(historyTareLst[0].tare)).ToString("F2");
                        return;
                    }
                }
                if (isGetHistoryTare)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ShowHistoryTareView view = new ShowHistoryTareView(mServiceModel.rows);
                        view.ShowDialog();
                    }));
                }
                isGetHistoryTare = true;
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体_获取历史皮重",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取历史皮重时异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 判断红外对射是不是遮挡
        /// </summary>
        /// <returns></returns>
        private bool CheckHongWaiDuiShe(out string rtInfos)
        {
            bool rtB = false;
            rtInfos = string.Empty;
            if (this.HandleTask != null && this.HandleTask.ServiceModel != null)
            {
                var isHwCtrlList = (from r in this.HandleTask.ServiceModel.hardwarectrl where r.name.Equals("红外对射") select r).ToList();
                if (isHwCtrlList.Count > 0 && isHwCtrlList.First().check.Equals("强制启用"))//服务指定坐席做红外对射判断
                {
                    if (LeftLine == Visibility.Visible)
                    {
                        rtInfos = "左红外对射被遮挡";
                    }
                    if (RightLine == Visibility.Visible)
                    {
                        rtInfos = "右红外对射被遮挡";
                    }
                    if (RightLine == Visibility.Visible && LeftLine == Visibility.Visible)
                    {
                        rtInfos = "左右红外对射都被遮挡";
                    }
                }
            }
            return rtB;
        }
        /// <summary>
        /// 调用保存之前验证……
        /// </summary>
        /// <returns></returns>
        private bool CheckIsAllowSave(string bInfoStr, bool isStopTask)
        {
            bool rtB = false;
            BullInfo bInfo = InfoExchange.DeConvert(typeof(BullInfo), bInfoStr) as BullInfo;
            if (string.IsNullOrEmpty(bInfo.matchid))
            {
                rtB = true;
            }
            else
            {
                string serviceUrl = ConfigurationManager.AppSettings["beforesaveMeasureInfo"].ToString();
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_保存前验证信息",
                    Level = LogConstParam.Directions_In,
                    Msg = "调用服务进行业务信息判断",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    Data = bInfoStr,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, bInfoStr, "");
                rtB = BeforeSaveWeightCallback(request, isStopTask);
            }
            return rtB;
        }

        /// <summary>
        /// 保存信息回调方法
        /// </summary>
        /// <param name="asyc"></param>
        private bool BeforeSaveWeightCallback(HttpWebRequest request, bool isStopTask)
        {
            bool rtB = true;
            try
            {
                string strResult = ComHelpClass.ResponseSynStr(request);
                var getServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_验证信息回调方法",
                    Level = LogConstParam.Directions_In,
                    Msg = "调用服务验证业务信息",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    Data = strResult,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                if (getServiceModel != null && getServiceModel.success)
                {
                    string rollMessage = string.Empty;
                    List<BullInfo> flagsList = GetflagsList(getServiceModel, ref rollMessage);
                    if (getServiceModel.mfunc == 3)//禁止计量  0代表允许计量  1 代表进行提示 2 代表进行选择 3 代表终止  
                    {
                        //弹出确认框，是否终止计量
                        //是:调用终止计量接口;本窗体关闭
                        //否：手动处理计量任务
                        //ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", "服务器禁止计量，是否终止任务？", true, true, "终止计量", "继续计量");
                        //将终止原因提示出来……lt 2016-2-2 17:12:15……
                        ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", rollMessage + " 服务器禁止计量", true, false, "终止计量", "继续计量");
                        SetShowListConfirmMessageBox(confirmBox, flagsList);
                        confirmBox.IsShowClose = true;
                        confirmBox.ShowDialog();
                        //logH.SaveLog("保存之前点击确认系统提示：" + rollMessage + ", 用户选择：终止计量");
                        #region 写日志
                        LogModel log1 = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                            Level = LogConstParam.Directions_In,
                            Msg = "保存之前点击确认系统提示：" + rollMessage + ", 用户选择：终止计量",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                        #endregion
                        if (confirmBox.isUserClose)
                        {
                            //logH.SaveLog("保存之前点击确认系统提示：" + rollMessage + ", 用户选择：关闭窗体");
                            #region 写日志
                            LogModel log2 = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                                Level = LogConstParam.Directions_In,
                                Msg = "保存之前点击确认系统提示：" + rollMessage + ", 用户选择：关闭窗体",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                            #endregion
                            return false;
                        }
                        else
                        {
                            if (confirmBox.IsOk)
                            {
                                stopTaskReason = rollMessage;
                                if (isStopTask)
                                {
                                    StopTask();
                                }
                                return false;
                            }
                            else
                            {
                                if (confirmBox.isSystermClose)
                                {
                                    return false;
                                }
                            }
                        }

                    }
                    else if (getServiceModel.mfunc == 2 && getServiceModel.flags.Count > 0)//选择情况
                    {
                        ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", rollMessage, true, true, "终止计量", "继续计量");
                        SetShowListConfirmMessageBox(confirmBox, flagsList);
                        confirmBox.IsShowClose = true;
                        confirmBox.ShowDialog();
                        if (confirmBox.isUserClose)
                        {
                            #region 写日志
                            LogModel log2 = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                                Level = LogConstParam.Directions_In,
                                Msg = "保存之前点击确认系统提示：" + rollMessage + ", 用户选择：关闭窗体",
                                Origin = "汽车衡_" + LoginUser.Role.Name,
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                            #endregion
                            return false;
                        }
                        else
                        {
                            if (confirmBox.IsOk)
                            {
                                stopTaskReason = rollMessage;
                                if (isStopTask)
                                {
                                    //logH.SaveLog("保存之前点击确认系统提示：" + rollMessage + ", 用户选择：终止计量");
                                    #region 写日志
                                    LogModel log2 = new LogModel()
                                    {
                                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        Direction = LogConstParam.Directions_In,
                                        FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                                        Level = LogConstParam.Directions_In,
                                        Msg = "保存之前点击确认系统提示：" + rollMessage + ", 用户选择：终止计量",
                                        Origin = "汽车衡_" + LoginUser.Role.Name,
                                    };
                                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                                    #endregion
                                    StopTask();
                                }
                                return false;
                            }
                            else
                            {
                                //logH.SaveLog("保存之前点击确认系统提示：" + rollMessage + ", 用户选择：继续计量");
                                #region 写日志
                                LogModel log2 = new LogModel()
                                {
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Direction = LogConstParam.Directions_In,
                                    FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                                    Level = LogConstParam.Directions_In,
                                    Msg = "保存之前点击确认系统提示：" + rollMessage + ", 用户选择：继续计量",
                                    Origin = "汽车衡_" + LoginUser.Role.Name,
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                                #endregion
                                if (confirmBox.isSystermClose)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else if (getServiceModel.mfunc == 1)//1的时候进行提示……
                    {
                        ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", rollMessage, true, false, "继续计量", "继续计量");
                        SetShowListConfirmMessageBox(confirmBox, flagsList);
                        confirmBox.ShowDialog();
                        if (confirmBox.isSystermClose)
                        {
                            return false;
                        }
                        #region 写日志
                        LogModel log2 = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                            Level = LogConstParam.Directions_In,
                            Msg = "保存之前点击确认系统提示：" + rollMessage + ", 用户选择：继续计量",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                        #endregion
                    }
                }
                else
                {
                    rtB = false;
                    string msg = "保存前验证业务信息时异常.";
                    if (getServiceModel != null && !string.IsNullOrEmpty(getServiceModel.msg))
                    {
                        msg = msg + ",原因:" + getServiceModel.msg;
                    }
                    #region 写日志
                    LogModel log2 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                        Level = LogConstParam.Directions_In,
                        Msg = "保存之前点击确认系统提示：" + "调用服务存储之前判断业务信息失败,原因:" + msg,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                    #endregion
                    ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", msg, true, false, "确定", "取消");
                    mb.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log2 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_任务处理窗体_保存信息回调方法",
                    Level = LogConstParam.Directions_In,
                    Msg = "保存之前点击确认系统提示：" + "调用服务存储之前判断业务信息失败,系统异常原因:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                #endregion
                string msg = ex.Message.Replace("The operation has times out", "物流服务无响应,操作超时!");
                ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "保存前判断业务信息时发生异常,原因:" + msg, true, false, "确定", "取消");
                mb.ShowDialog();
                rtB = false;
            }
            return rtB;
        }
        /// <summary>
        /// 获取提示信息
        /// </summary>
        /// <returns></returns>
        private List<BullInfo> GetflagsList(MeasureServiceModel msM, ref string rollMessage)
        {
            List<BullInfo> flagsList = new List<BullInfo>();
            rollMessage = string.Empty;
            foreach (var item in msM.flags)
            {
                if (item != null && !string.IsNullOrEmpty(item.Msg))
                {
                    if (item.flag != 0)
                    {
                        if (msM.rows.Count > 0)
                        {
                            //if (msM.rows[0].operatype.Equals("80"))
                            //{
                            //    if (item.Msg.Contains("已经计量皮重"))
                            //    {
                            //        rollMessage = "销售业务--车间未出库, " + rollMessage;
                            //    }
                            //    else
                            //    {
                            //        rollMessage = item.Msg + "\r\n " + rollMessage;
                            //    }
                            //}
                            //else
                            //{
                            rollMessage = item.Msg + "\r\n " + rollMessage;
                            //}
                        }
                        else
                        {
                            rollMessage = item.Msg + "\r\n" + rollMessage;
                        }

                    }
                    if (item.list.Count > 0)
                    {
                        foreach (var itemList in item.list)
                        {
                            flagsList.Add(itemList);//存储对应的需要展示的列表信息……
                        }
                    }
                }
            }
            //if (!string.IsNullOrEmpty(rollMessage))
            //{
            //    rollMessage = rollMessage.Substring(0, rollMessage.Length - 2);
            //}
            return flagsList;
        }
        /// <summary>
        /// 获取业务信息字符串
        /// </summary>
        /// <param name="measureType"></param>
        /// <returns></returns>
        private string GetbInfoStr(string measureType)
        {
            string rtInfos = string.Empty;
            switch (measureType)
            {
                case "G":
                    this.HandleTask.BullInfo.tare = this.originTare;
                    this.HandleTask.BullInfo.gross = string.IsNullOrEmpty(CurWeight) ? 0.00M : decimal.Parse(CurWeight);
                    this.HandleTask.BullInfo.measurestate = "G";
                    break;
                case "T":
                    this.HandleTask.BullInfo.gross = this.originGross;
                    this.HandleTask.BullInfo.tare = string.IsNullOrEmpty(CurWeight) ? 0.00M : decimal.Parse(CurWeight);
                    this.HandleTask.BullInfo.measurestate = "T";
                    break;
            }
            this.HandleTask.BullInfo.ruleflag = 1;//1读取数据
            rtInfos = JsonConvert.SerializeObject(this.HandleTask.BullInfo);
            return rtInfos;
        }

        /// <summary>
        /// 设置提示框信息
        /// </summary>
        /// <param name="inBox"></param>
        /// <param name="?"></param>
        private void SetShowListConfirmMessageBox(ConfirmMessageBox inBox, List<BullInfo> listInfos)
        {
            inBox.IsTimer = false;
            inBox.cMatchid = this.HandleTask.BullInfo.matchid;
            inBox.cMeasureType = this.HandleTask.BullInfo.measurestate;
            inBox.ShowList = listInfos;
        }
        /// <summary>
        /// 处理是不是使用超期皮重
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="isUse"></param>
        private void DoTareTimeOut(string infos, bool isUse)
        {
            if (infos.Contains("皮重已超期"))
            {
                if (!isUse)//不使用 则将给的皮重信息全部变为空……
                {
                    //logH.SaveLog("获取业务信息时用户选择继续计量,并不使用超期皮重");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "坐席_处理任务窗体_处理是不是使用超期皮重",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "获取业务信息时用户选择继续计量,并不使用超期皮重",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    this.originTare = 0;
                    this.handleTask.BullInfo.tare = 0;
                    this.HandleTask.BullInfo.taretime = string.Empty;
                    this.HandleTask.BullInfo.tareweigh = string.Empty;
                    this.HandleTask.BullInfo.tareweighid = string.Empty;
                    this.HandleTask.BullInfo.tareoperacode = string.Empty;
                    this.handleTask.BullInfo.tareoperaname = string.Empty;
                    DoShowWeightInfo(false);
                }
            }
        }
        /// <summary>
        /// 发送通知给秤点
        /// </summary>
        /// <param name="infos"></param>
        private void SendNoticToClient(string infos)
        {
            int unm = CommonMethod.CommonMethod.GetRandom();
            var para = new
            {
                clientid = this.HandleTask.ClientCode,
                cmd = ParamCmd.UserNotice,
                msg = infos,
                msgid = unm
            };
            SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_任务处理窗体_发送通知给秤点",
                Level = LogConstParam.LogLevel_Info,
                Msg = "坐席处理界面往秤体发送通知",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                Data = para,
                IsDataValid = LogConstParam.DataValid_Ok,
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        #endregion

        /// <summary>
        /// 处理显示的重量信息 例如后台存储千克 前台显示吨的问题
        /// <param name="isFirstWeightB">是否改变对应的供方重量信息</param>
        /// </summary>
        private void DoShowWeightInfo(bool isFirstWeightB)
        {
            try
            {
                if (handleTask != null && handleTask.BullInfo != null)
                {
                    if (handleTask.BullInfo.gross != null)
                    {
                        ShowGross = (Convert.ToDecimal(handleTask.BullInfo.gross) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowGross = string.Empty;
                    }
                    if (handleTask.BullInfo.tare != null)
                    {
                        ShowTare = (Convert.ToDecimal(handleTask.BullInfo.tare) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowTare = string.Empty;
                    }
                    //if (handleTask.BullInfo.deduction != null)
                    //{
                    //    ShowDedution = (Convert.ToDecimal(handleTask.BullInfo.deduction) * weightKg).ToString("F2");
                    //}
                    //else
                    //{
                    //    ShowDedution = string.Empty;
                    //}
                    if (handleTask.BullInfo.suttle != null)
                    {
                        ShowSuttle = (Convert.ToDecimal(handleTask.BullInfo.suttle) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowSuttle = string.Empty;
                    }
                    if (isFirstWeightB)
                    {
                        if (handleTask.BullInfo.grossb != null)
                        {
                            ShowGrossb = (Convert.ToDecimal(handleTask.BullInfo.grossb) * weightKg).ToString("F2");
                        }
                        else
                        {
                            ShowGrossb = string.Empty;
                        }
                        if (handleTask.BullInfo.tareb != null)
                        {
                            ShowTareb = (Convert.ToDecimal(handleTask.BullInfo.tareb) * weightKg).ToString("F2");
                        }
                        else
                        {
                            ShowTareb = string.Empty;
                        }
                        if (handleTask.BullInfo.suttleb != null)
                        {
                            ShowSuttleb = (Convert.ToDecimal(handleTask.BullInfo.suttleb) * weightKg).ToString("F2");
                        }
                        else
                        {
                            ShowSuttleb = string.Empty;
                        }
                    }

                }
                else
                {
                    ShowGross = string.Empty;
                    ShowTare = string.Empty;
                    //ShowDedution = string.Empty;
                    ShowSuttle = string.Empty;
                    ShowGrossb = string.Empty;
                    ShowTareb = string.Empty;
                    ShowSuttleb = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ShowGross = string.Empty;
                ShowTare = string.Empty;
                //ShowDedution = string.Empty;
                ShowSuttle = string.Empty;
                //ShowGrossb = string.Empty;
                //ShowTareb = string.Empty;
                //ShowSuttleb = string.Empty;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体_处理显示的重量信息",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "处理显示的重量信息时异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 无计量申请单 继续计量的情况
        /// </summary>
        private void DoNoMeasureApply()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getMeasureShowInfo"].ToString().Replace('$', '&');
            string getUrl = string.Format(serviceUrl, "0", 1);
            try
            {
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                request.BeginGetResponse(new AsyncCallback(GetMeasureShowInfoCallback), request);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_无计量申请单 继续计量的情况",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务器读取单独计皮业务界面显示信息!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = getUrl,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.LogLevel_Error,
                    FunctionName = "坐席_任务处理窗体_无计量申请单 继续计量的情况",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务器读取单独计皮业务界面显示信息!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = getUrl + "出错：" + ex.Message,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 获取单独计皮的业务信息
        /// </summary>
        /// <param name="asyc"></param>
        public void GetMeasureShowInfoCallback(IAsyncResult asyc)
        {
            try
            {
                MeasureServiceModel mServiceModel = new MeasureServiceModel();
                string strResult = ComHelpClass.ResponseStr(asyc);
                mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_获取单独计皮的业务信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务器读取计量显示数据成功!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = strResult,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                List<BullInfo> getBullInfo = mServiceModel.rows;
                BullInfo newB = new BullInfo();
                newB.carno = this.carNo;
                newB.operatype = "0";
                newB.measurestate = "T";
                newB.icid = this.icId;
                getBullInfo.Add(newB);
                this.HandleTask.ServiceModel = mServiceModel;
                this.HandleTask.BullInfos = mServiceModel != null ? mServiceModel.rows : null;
                this.ShowBusy = false;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    NextBusinessInit();
                }));
            }
            catch (Exception ex)
            {
                this.ShowBusy = false;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "获取计量信息失败，原因:" + ex.Message, true, false, "确定", "取消");
                    mb.ShowDialog();
                }));
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_获取单独计皮的业务信息",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务读取计量显示数据失败getMeasureShowInfo！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 处理业务号信息
        /// </summary>
        /// <param name="tcM"></param>
        public void DoTaskCode(TaskCodeModel tcM)
        {
            BullInfo bi = new BullInfo();
            bi.caller = "1";
            bi.carno = this.carNo;
            bi.taskcode = tcM.taskcode;
            bi.operatype = tcM.operatype;
            bi.sourcecode = tcM.sourcecode;
            bi.sourcename = tcM.sourcename;
            bi.targetname = tcM.targetname;
            bi.targetcode = tcM.targetcode;
            bi.icid = this.HandleTask.IcId;
            bi.materialcode = tcM.materialcode;
            bi.materialname = tcM.materialname;
            bi.mflag = Convert.ToInt32(tcM.mflag);
            bi.gflag = Convert.ToInt32(tcM.gflag);
            bi.deduction2 = Convert.ToDecimal(tcM.deduction2);
            bi.operaname = tcM.operaname;
            try
            {
                this.ShowBusy = false;
                string bInfoStr = JsonConvert.SerializeObject(bi);
                string serviceUrl = ConfigurationManager.AppSettings["inputPlanidInfo"].ToString();
                HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, bInfoStr, "");
                string strResult = ComHelpClass.ResponseSynStr(request);
                MeasureServiceModel mServiceModel = new MeasureServiceModel();
                mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_任务处理窗体_处理业务号信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务器调拨业务读取计量显示数据成功!",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = strResult,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.HandleTask.ServiceModel = mServiceModel;
                this.HandleTask.BullInfos = mServiceModel != null ? mServiceModel.rows : null;
                //历史皮重初始化……2016-3-17 14:06:06
                historyTareLst.Clear();
                LastTare = "0";
                isGetHistoryTare = false;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    NextBusinessInit();
                }));
            }
            catch (Exception ex)
            {
                this.ShowBusy = false;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "获取计量信息失败，原因:" + ex.Message, true, false, "确定", "取消");
                    mb.ShowDialog();
                }));
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务处理窗体_处理业务号信息",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务读取调拨业务计量显示数据失败moreMeasureInfo！原因：" + ex.Message,
                    Origin = "汽车衡" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 用户自定义背景颜色
        /// </summary>
        private void DoBackGroundColor()
        {
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string url = basePath + @"/ClientConfig/UserDoResultColor.txt";
            if (!FileHelper.IsExistFile(url))
            {
                FileHelper.CreateFile(url);
                FileHelper.AppendText(url, "#FFFFFF");
            }
            string userColor = FileHelper.FileToString(url);
            if (!userColor.Equals("#FFFFFF"))
            {
                BColor0 = userColor;
            }
        }
        /// <summary>
        /// 清除长皮信息
        /// </summary>
        private void ClearHistoryTare()
        {
            if (this.HandleTask != null)
            {
                if (this.HandleTask.BullInfo != null)
                {
                    this.HandleTask.BullInfo.tare = 0;
                    ShowTare = string.Empty;
                    SendBusinessInfosToTaskServer();
                    //logH.SaveLog(this.HandleTask.BullInfo.carno + "  , 用户选择不使用历史皮重");
                    #region 日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_任务处理窗体_清除长皮信息",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = this.HandleTask.BullInfo.carno + "  , 用户选择不使用历史皮重",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }
        /// <summary>
        /// 显示长皮信息
        /// </summary>
        private void ShowHistoryTare()
        {
            if (this.HandleTask != null)
            {
                if (this.HandleTask.BullInfo != null)
                {
                    this.HandleTask.BullInfo.tare = originTare;
                    SendBusinessInfosToTaskServer();
                    //logH.SaveLog(this.HandleTask.BullInfo.carno + "  , 用户选择使用历史皮重");
                    #region 日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_任务处理窗体_显示长皮信息",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = this.HandleTask.BullInfo.carno + "  , 用户选择使用历史皮重",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }
        /// <summary>
        /// 设置 BullInfo 给服务的值
        /// </summary>
        private void SetBullInfo()
        {
            try
            {
                if (IsGrossWeight)
                {
                    this.HandleTask.BullInfo.measurestate = "G";
                }
                if (IsTareWeight)
                {
                    this.HandleTask.BullInfo.measurestate = "T";
                }

                if (((this.HandleTask.BullInfo.gross > 0 && this.HandleTask.BullInfo.tare > 0)) && this.HandleTask.BullInfo.measurestate == "G")
                {
                    this.HandleTask.BullInfo.grossweigh = this.HandleTask.ClientName;
                    this.HandleTask.BullInfo.grossweighid = this.HandleTask.ClientId;
                    this.HandleTask.BullInfo.grossoperacode = LoginUser.LoginName;
                    this.handleTask.BullInfo.grossoperaname = LoginUser.Name;
                    this.HandleTask.BullInfo.suttleweigh = this.HandleTask.ClientName;
                    this.HandleTask.BullInfo.suttleweighid = this.HandleTask.ClientId;
                    this.HandleTask.BullInfo.suttleoperacode = LoginUser.LoginName;
                    this.handleTask.BullInfo.suttleoperaname = LoginUser.Name;
                }
                else if (((this.HandleTask.BullInfo.gross > 0 && this.HandleTask.BullInfo.tare > 0)) && this.HandleTask.BullInfo.measurestate == "T")
                {
                    this.HandleTask.BullInfo.tareweigh = this.HandleTask.ClientName;
                    this.HandleTask.BullInfo.tareweighid = this.HandleTask.ClientId;
                    this.HandleTask.BullInfo.tareoperacode = LoginUser.LoginName;
                    this.handleTask.BullInfo.tareoperaname = LoginUser.Name;
                    this.HandleTask.BullInfo.suttleweigh = this.HandleTask.ClientName;
                    this.HandleTask.BullInfo.suttleweighid = this.HandleTask.ClientId;
                    this.HandleTask.BullInfo.suttleoperacode = LoginUser.LoginName;
                    this.handleTask.BullInfo.suttleoperaname = LoginUser.Name;
                }
                else if (this.HandleTask.BullInfo.measurestate == "G")
                {
                    this.HandleTask.BullInfo.grossweigh = this.HandleTask.ClientName;
                    this.HandleTask.BullInfo.grossweighid = this.HandleTask.ClientId;
                    this.HandleTask.BullInfo.grossoperacode = LoginUser.LoginName;
                    this.handleTask.BullInfo.grossoperaname = LoginUser.Name;
                }
                else if (this.HandleTask.BullInfo.measurestate == "T")
                {
                    this.HandleTask.BullInfo.tareweigh = this.HandleTask.ClientName;
                    this.HandleTask.BullInfo.tareweighid = this.HandleTask.ClientId;
                    this.HandleTask.BullInfo.tareoperacode = LoginUser.LoginName;
                    this.handleTask.BullInfo.tareoperaname = LoginUser.Name;
                }
                if (AutoTareIsCheck)
                {
                    this.HandleTask.BullInfo.tareweigh = string.Empty;
                    this.HandleTask.BullInfo.tareweighid = string.Empty;
                    this.HandleTask.BullInfo.tareoperacode = string.Empty;
                    this.handleTask.BullInfo.tareoperaname = string.Empty;
                    this.HandleTask.BullInfo.taretime = string.Empty;
                }
                this.HandleTask.BullInfo.ruleflag = 2;
                this.HandleTask.BullInfo.rfid = this.rfidId;
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席任务处理窗体_设置业务信息(SetBullInfo)",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "设置业务信息(SetBullInfo)时异常:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
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
    }
}
