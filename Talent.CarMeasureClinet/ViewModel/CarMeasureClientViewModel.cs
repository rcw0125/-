using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Talent.ClientCommMethod;
using Talent.ClientCommonLib;
using Talent.CommonMethod;
using SocketIOClient;
using System.ComponentModel;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using System.Net;
using Talent.Weight.Interface;
using Talent.Weight.Controller;
using Talent.Ic;
using Talent.Ic.Controller;
using Talent.Rfid.Controller;
using Talent.Io.Controller;
using Talent.Measure.DomainModel.CommonModel;
using System.Windows.Threading;
using System.Windows;
using Talent.Io;
using Talent.ClientCommonLib.Controls;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;
using Talent.Measure.DomainModel;
using Talent.Video.Controller;
using Newtonsoft.Json;
using Talent.CarMeasureClient.ConfirmView;
using Talent.Keyboard.Controller;
using Talent.Keyboard.Interface;
using Talent.Printer.Controller;
using Talent.Measure.DomainModel.ServiceModel;
using Talent.CarMeasureClient.Common;
using System.Windows.Controls;
using System.Windows.Media;
using Talent.CarMeasureClient.MyUC;
using Talent_LT.HelpClass;
using System.Diagnostics;
using Talent.Measure.WPF.Remote;
using Newtonsoft.Json.Linq;
using Talent.Measure.WPF.Log;
using System.Drawing.Imaging;
using System.Collections.ObjectModel;
using Talent.Printer.Interface;
using Talent.Cheat.Controller;
using Talent.Cheat.Interface;

namespace Talent.CarMeasureClient.ViewModel
{
    public class CarMeasureClientViewModel : Only_ViewModelBase
    {
        bool _isDownLoadConfigFile = false;
        //bool _isDownLoadConfigFile = true;
        #region 公共属性
        #region 初始化配置字段

        /// <summary>
        /// 信息Id，用于和任务服务器交互过程中使用，确保交互的信息任务服务器接收到
        /// </summary>
        string MsgId = "";

        /// <summary>
        /// 任务回退是否返回值了
        /// </summary>
        bool isBackTaskReply = false;

        /// <summary>
        /// 是否允许文字转声音朗读
        /// </summary>
        bool isCanReadText = true;

        /// <summary>
        /// 记录发送任务时的时间(重发任务时使用，用于调用java服务时的matchid)
        /// </summary>
        string TaskTime = "";
        /// <summary>
        /// 所属窗体
        /// </summary>
        public MainWindow window { get; set; }

        /// <summary>
        /// 倒计时动画
        /// </summary>
        public System.Windows.Media.Animation.Storyboard std = new System.Windows.Media.Animation.Storyboard();
        private bool taskServerConnState = false;
        WindowsFormHelpClass formHClass = new WindowsFormHelpClass();//窗体帮助类
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

        private bool isFullScreenEnable;
        /// <summary>
        /// 是否全屏(用于接收到坐席发过来的全屏指令后，设置窗体全屏)
        /// </summary>
        public bool IsFullScreenEnable
        {
            get { return isFullScreenEnable; }
            set
            {
                isFullScreenEnable = value;
                this.RaisePropertyChanged("IsFullScreenEnable");
            }
        }

        /// <summary>
        /// 是否保存所有重量曲线
        /// </summary>
        bool IsSaveAllWeightData = false;

        /// <summary>
        /// 是否取数的开关，重量启动方式下使用
        /// </summary>
        public bool IsGetWeight { get; set; }
        /// <summary>
        /// 称重计时器
        /// </summary>
        private Calculagraph timer;
        /// <summary>
        /// 重发任务计时器
        /// </summary>
        private Calculagraph resendTaskTimer;
        /// <summary>
        /// 任务回退计时器
        /// </summary>
        private Calculagraph backTaskTimer;
        /// <summary>
        /// 检测是否全屏计时器
        /// </summary>
        private Calculagraph fullScreenTimer;
        /// <summary>
        /// 称重读数列表，重量启动方式下使用，当前启动方式不使用
        /// </summary>
        List<decimal> listWeights = new List<decimal>();
        /// <summary>
        /// 记录表头记录
        /// </summary>
        WeightRealData wRealData = new WeightRealData();
        /// <summary>
        /// 记录重量曲线数据
        /// </summary>
        List<WeightRecordData> recorddatalist = new List<WeightRecordData>();
        private string _leftLed;
        /// <summary>
        /// 绑定左红绿灯
        /// </summary>
        public string leftLed
        {
            get { return _leftLed; }
            set
            {
                _leftLed = value;
                this.RaisePropertyChanged("leftLed");
            }

        }
        /// <summary>
        /// 终端自动重启时间（分）
        /// </summary>
        private int _autoRunTime = 60;
        public int AutoRunTime
        {
            get { return _autoRunTime; }
            set
            {
                _autoRunTime = value;
            }
        }

        private string _rightLed;
        /// <summary>
        /// 绑定右红绿灯
        /// </summary>
        public string rightLed
        {
            get { return _rightLed; }
            set
            {
                _rightLed = value;
                this.RaisePropertyChanged("rightLed");
            }

        }
        /// <summary>
        /// 称重计算周期
        /// </summary>
        private int weightTimePeriod;
        /// <summary>
        /// 服务送达周期
        /// </summary>
        private int measureTimePeriod;
        /// <summary>
        /// 称重允差
        /// </summary>
        private decimal weightAllowance;
        /// <summary>
        /// 最大尝试次数
        /// </summary>
        private int weightTimeCount;
        /// <summary>
        /// 临时记录
        /// </summary>
        private int temWeightTimeCount;
        /// <summary>
        /// 图片保存路径
        /// </summary>
        private string picPath = string.Empty;

        private string _sysCompany;
        /// <summary>
        /// 软件开发公司
        /// </summary>
        public string sysCompany
        {
            get { return _sysCompany; }
            set
            {
                _sysCompany = value;
                this.RaisePropertyChanged("sysCompany");
            }
        }
        private string _measurementUnitName;
        /// <summary>
        ///计量单位名称
        /// </summary>
        public string measurementUnitName
        {
            get { return _measurementUnitName; }
            set
            {
                _measurementUnitName = value;
                this.RaisePropertyChanged("measurementUnitName");
            }
        }
        private string _softName;
        /// <summary>
        /// 软件名称
        /// </summary>
        public string softName
        {
            get { return _softName; }
            set
            {
                _softName = value;
                this.RaisePropertyChanged("softName");
            }
        }
        private decimal _BeginWeight;
        /// <summary>
        /// 启动重量
        /// </summary>
        public decimal BeginWeight
        {
            get { return _BeginWeight; }
            set
            {
                _BeginWeight = value;
                this.RaisePropertyChanged("BeginWeight");
            }
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
        private string _Msg1 = string.Empty;
        /// <summary>
        /// 显示上一条计量信息
        /// </summary>
        public string Msg1
        {
            get { return _Msg1; }
            set
            {
                _Msg1 = value;
                this.RaisePropertyChanged("Msg1");
            }
        }
        private string _Msg2 = string.Empty;
        /// <summary>
        /// 显示计量信息
        /// </summary>
        public string Msg2
        {
            get { return _Msg2; }
            set
            {
                _Msg2 = value;
                this.RaisePropertyChanged("Msg2");
            }
        }

        private string _Msg3;
        /// <summary>
        /// 通知信息
        /// </summary>
        public string Msg3
        {
            get { return _Msg3; }
            set
            {
                _Msg3 = value;
                this.RaisePropertyChanged("Msg3");
            }
        }
        private string _MeasureTypeInfo;
        /// <summary>
        /// 计量方式
        /// </summary>
        public string MeasureTypeInfo
        {
            get { return _MeasureTypeInfo; }
            set
            {
                _MeasureTypeInfo = value;
                this.RaisePropertyChanged("MeasureTypeInfo");
            }
        }
        private string _configUrl;
        /// <summary>
        /// 文件绑定路径
        /// </summary>
        public string ConfigUrl
        {
            get { return _configUrl; }
            set
            {
                _configUrl = value;
            }
        }
        private int _StartupInfo;
        /// <summary>
        /// 计量业务启动方式
        /// </summary>
        public int StartupInfo
        {
            get { return _StartupInfo; }
            set
            {
                _StartupInfo = value;
                this.RaisePropertyChanged("StartupInfo");
            }
        }
        private bool isRedLedOpend;
        /// <summary>
        /// 红灯是否已经打开(打开红灯时的开关)
        /// </summary>
        public bool IsRedLedOpend
        {
            get { return isRedLedOpend; }
            set { isRedLedOpend = value; }
        }
        /// <summary>
        /// 配置文件Url
        /// </summary>
        private string configUrl;
        /// <summary>
        /// 是否确认重量
        /// </summary>
        private bool IsConfirmWeight;
        /// <summary>
        /// 是否功能开始
        /// </summary>
        private bool IsFounctionStart = true;
        /// <summary>
        /// 空称显示信息
        /// </summary>
        private string KCStr;
        /// <summary>
        /// 上称显示信息
        /// </summary>
        private string SCStr;
        /// <summary>
        /// 任务发送后显示信息
        /// </summary>
        private string RWFSStr;
        /// <summary>
        /// 计量中显示信息
        /// </summary>
        private string JLZStr;
        /// <summary>
        /// 计量完成显示信息
        /// </summary>
        private string JLWCStr;
        /// <summary>
        /// 下称显示信息
        /// </summary>
        private string XCStr;
        /// <summary>
        /// 刷卡显示信息
        /// </summary>
        private string SKStr;
        /// <summary>
        /// 终止计量
        /// </summary>
        private string ZZStr;
        /// <summary>
        /// 发送远程
        /// </summary>
        private string YCStr;
        /// <summary>
        /// 系统提示
        /// </summary>
        private string XTTSStr;
        /// <summary>
        /// 系统异常
        /// </summary>
        private string XTYCStr;
        /// <summary>
        /// 是否标准键盘
        /// </summary>
        private bool isStandardBoard;
        /// <summary>
        /// 最后一条显示信息
        /// </summary>
        private string lastMsg1;
        private string lastMsg2;
        public string measureServiceResult = string.Empty;
        /// <summary>
        /// 远程出票对象(坐席端远程出票发起时,传递过来的参数对象)
        /// </summary>
        private PrintInfo printModel;
        /// <summary>
        /// 是否摁下 OK 键 2016-3-31 15:31:46……
        /// </summary>
        public bool isEnterOK = false;
        /// <summary>
        /// 是否摁下 取消键 
        /// </summary>
        public bool isEnterCancel = false;
        /// <summary>
        /// 是否摁下 求助键
        /// </summary>
        public bool isEnterHelp = false;
        /// <summary>
        /// 是否允许恩下 OK 
        /// </summary>
        public bool isAllowEnterOk = false;
        /// <summary>
        /// 是否只能摁一次
        /// </summary>
        public bool isEnterOnly = false;
        /// <summary>
        /// 表头为0情况下发送实时数据的次数
        /// </summary>
        private decimal SendRealDataCountInZero = 0;
        /// <summary>
        /// 最后一次获取重量的时间
        /// </summary>
        public DateTime lastGetWeightDate = Convert.ToDateTime("1799-01-01 00:00:00");
        /// <summary>
        /// 是否清空通知
        /// </summary>
        public bool isClearInfos = false;
        /// <summary>
        /// 重量日志车号
        /// </summary>
        private string saveCarNo = string.Empty;
        /// <summary>
        /// 重量日志过磅单号
        /// </summary>
        private string saveMatchid = string.Empty;
        /// <summary>
        /// 获取打印数据失败次数
        /// </summary>
        private int getPrintDataCount = 0;
        /// <summary>
        /// 称点状态信息
        /// </summary>
        private string WeightMsg = string.Empty;
        /// <summary>
        /// 临时自动重启
        /// </summary>
        public bool isTempRestart = false;


        #endregion
        #region 业务属性
        /// <summary>
        /// 输入业务号窗体界面
        /// </summary>
        public ConfirmInputPlanCodeView inputPlanCodeView;
        private string _CarNumber = string.Empty;
        public string CarNumber
        {
            get { return _CarNumber; }
            set
            {
                _CarNumber = value;
            }
        }
        /// <summary>
        /// 是否保存重量
        /// </summary>
        bool saveWeight = false;

        /// <summary>
        /// 标记车下称
        /// </summary>
        bool carLeave = true;

        int globTimerState = 0;//保存重量

        private decimal _Weight;
        /// <summary>
        /// 获取重量(用于界面显示)
        /// </summary>
        public decimal Weight
        {
            get { return _Weight; }
            set
            {
                try
                {
                    _Weight = value;
                    this.RaisePropertyChanged("Weight");
                    //处理ic卡移走期间，重量最大值最小值变化记录
                    DoMoveCardWeight();//处理移卡时的重量……
                    //将当前重量发送给任务服务器
                    SendRealDataToServer("W");//修改为 采到数据就发送 不管是负数还是 0 还是正常数据 lt 2016-2-1 08:46:24……
                    //判断重量是否超量程
                    CheckIsShowWeightControl();//判断是不是超过量程 之后不再显示当前重量信息
                    //向视频中写入当前重量 
                    SaveWeightToVideo();
                    if (value > 0)
                    {
                        //在重量启动方式下，才会执行
                        if (IsGetWeight)
                        {
                            listWeights.Add(Weight);
                        }
                        //要搞清楚mServiceModel是什么
                        if (BullState != eBullTag.free && mServiceModel != null && mServiceModel.rows != null && mServiceModel.rows.Count == 1 && globTimerState != 5)
                        {
                            if (mServiceModel.rows[0].measurestate == "G")//G计毛
                            {
                                //加载业务数据值
                                DoGrossTareWeightValue("G");
                            }
                            else if (mServiceModel.rows[0].measurestate == "T")//T计皮
                            {
                                DoGrossTareWeightValue("T");
                            }
                        }
                    }
                    #region 测试用
                    else if (_Weight == 0 && BullState == eBullTag.free)
                    {
                        iwc.IsFinish = false;
                    }
                    #endregion
                    //计量业务状态，specification状态需要满足，车上称且当前状态大于起始重量
                    if ((BullState == eBullTag.weight) && WeightCompare())
                    {
                        BullState = eBullTag.specification;
                    }
                    else
                    {
                        if (WeightCompare() && BullState == eBullTag.free)
                        {
                            recorddatalist.Clear();
                            #region 日志
                            LogModel log1 = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "称点主窗体_车上秤前清空重量记录",
                                Origin = ClientInfo.Name,
                                Level = LogConstParam.LogLevel_Info,
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                            #endregion
                            BullState = eBullTag.weight;//状态置为车上称
                            if (IsSaveAllWeightData)
                            {
                                saveWeight = true; //车上秤后允许最后保存记录
                            }
                            wRealData.begintime = System.DateTime.Now.ToLocalTime().ToString();//车上称 开始时间……
                            receiveBullInfo = new BullInfo();
                        }
                        else if ((value <= BeginWeight && (BullState == eBullTag.end || BullState == eBullTag.stop)) || (value <= BeginWeight && BullState != eBullTag.init && BullState != eBullTag.free))
                        {
                            try
                            {
                                #region 日志
                                LogModel log1 = new LogModel()
                                {
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Direction = LogConstParam.Directions_In,
                                    FunctionName = "称点主窗体_触发车下秤",
                                    Origin = ClientInfo.Name,
                                    Level = LogConstParam.LogLevel_Info,
                                    Msg = "当前重量:" + value + " BeginWeight:" + BeginWeight + " carLeave:" + carLeave
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                                #endregion

                                if (carLeave)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        window.CloseChildWindow();  //关闭所有子窗体
                                    });
                                    carLeave = false;
                                    #region 日志
                                    LogModel log = new LogModel()
                                    {
                                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        Direction = LogConstParam.Directions_In,
                                        FunctionName = "称点主窗体_进入车下秤逻辑",
                                        Origin = ClientInfo.Name,
                                        Level = LogConstParam.LogLevel_Info,
                                        Msg = "当前重量:" + value + " BeginWeight:" + BeginWeight
                                    };
                                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                                    #endregion
                                    CleatNoticeInfos();
                                    StopIc();//暂停IC读卡设备……2016-2-29 09:52:12… …
                                    //StopRFID();
                                    //1.记量完成2.在计量中重量出现0情况(表头有问题或车直接开下称)
                                    CloseRFID();//关闭RFID
                                    RFIDCardNo.Clear();
                                    IsMeasuring = false;//远程计量关闭 2016-3-1 11:05:09……                               
                                    isEnterOnly = false;
                                    #region 写日志
                                    LogModel log3 = new LogModel()
                                    {
                                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        Direction = LogConstParam.Directions_In,
                                        FunctionName = "秤点主窗体_车下称",
                                        Level = LogConstParam.LogLevel_Info,
                                        Msg = "准备向任务服务器发送任务清除命令",
                                        IsDataValid = LogConstParam.DataValid_Ok,
                                        Origin = "汽车衡_" + ClientInfo.Name,
                                        OperateUserName = IcCardNo
                                    };
                                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
                                    #endregion
                                    SendBackTaskInfo();//车下称 告诉任务服务器……2016-3-8 10:50:25
                                    this.IcCardNo = "";
                                    mServiceModel = null;
                                    #region 称点计量记录信息
                                    if (saveWeight)
                                    {
                                        wRealData.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//24小时制
                                        if (mServiceModel != null && mServiceModel.rows != null && mServiceModel.rows.Count > 0)
                                        {
                                            wRealData.matchid = mServiceModel.rows[0].matchid;
                                        }
                                        else
                                        {
                                            wRealData.matchid = " ";
                                        }
                                        maxWeight = 0;
                                        //保存重量曲线记录
                                        saveWeightRecordServiceInfo();
                                        //recorddatalist.Clear();
                                    }
                                    #region 日志
                                    LogModel log2 = new LogModel()
                                    {
                                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        Direction = LogConstParam.Directions_In,
                                        FunctionName = "称点主窗体_车下秤将终端状态设为Free, saveWeight=" + saveWeight,
                                        Origin = ClientInfo.Name,
                                        Level = LogConstParam.LogLevel_Info,
                                    };
                                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                                    #endregion
                                    BullState = eBullTag.free;
                                }
                                    #endregion

                                //isTempRestart = true;
                            }
                            catch (Exception ex)
                            {
                                #region 写日志
                                LogModel log = new LogModel()
                                {
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Msg = "车辆下称时，秤点错误：" + ex.Message + "堆栈：" + ex.StackTrace,
                                    FunctionName = "称点主窗体_车下称",
                                    Origin = "汽车衡_" + ClientInfo.Name,
                                    Level = LogConstParam.LogLevel_Error
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                                #endregion
                            }
                        }
                    }
                    //车在称上时，记录重量--时间点
                    if (BullState != eBullTag.free && _BullState != eBullTag.error && carLeave)
                    {

                        WeightRecordData wrD = new WeightRecordData { recorddata = value, recordtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                        //判断是否允许保存重量，同一时刻同一重量不记录
                        if (CheckIsSaveWeightList(wrD))
                        {
                            recorddatalist.Add(wrD);//记录表头数据
                        }
                        if (maxWeight < value)
                        {
                            maxWeight = value;
                        }


                    }
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = "获取到重量数据处理时错误：" + ex.Message + "堆栈：" + ex.StackTrace,
                        FunctionName = "称点主窗体_界面获取到重量",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Error,
                        Data = "Weight=" + Weight
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }

        private decimal realWeight;
        /// <summary>
        /// 实际重量
        /// </summary>
        public decimal RealWeight
        {
            get { return realWeight; }
            set { realWeight = value; }
        }

        private string _showClientName;
        /// <summary>
        /// 当前终端名称 lt 2016-1-29 15:19:07……
        /// </summary>
        public string ShowClientName
        {
            get { return _showClientName; }
            set
            {
                _showClientName = value;
                this.RaisePropertyChanged("ShowClientName");
            }
        }
        private string _IcCardNo;
        /// <summary>
        /// Ic卡号
        /// </summary>
        public string IcCardNo
        {
            get { return _IcCardNo; }
            set
            {
                _IcCardNo = value;
                this.RaisePropertyChanged("IcCardNo");
            }
        }
        /// <summary>
        /// 当前的rfidId号……2016-3-17 14:45:12
        /// </summary>
        string rfidId = string.Empty;
        private List<string> _RFICCardNo;
        /// <summary>
        /// Rfid卡号
        /// </summary>
        public List<string> RFIDCardNo
        {
            get
            {
                //if (string.IsNullOrEmpty(_RFICCardNo))
                //{
                //    return "0311934F";
                //}
                //else
                //{
                return _RFICCardNo;
                //}
            }
            set
            {
                _RFICCardNo = value;
                this.RaisePropertyChanged("RFICCardNo");
                if (RFIDCardNo != null)
                {
                    if (RFIDCardNo.Count > 0)
                    {
                        rfidId = RFIDCardNo[0];
                        #region 写日志
                        //LogModel log = new LogModel()
                        //{
                        //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        //    Msg = "当前的Rfid卡号(rfidId):" + rfidId,
                        //    FunctionName = "称点主窗体_Rfid卡号赋值",
                        //    Origin = "汽车衡_" +ClientInfo.Name,
                        //    Level = LogConstParam.LogLevel_Info
                        //};
                        //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                }
            }
        }

        private string _BuTypeId;
        /// <summary>
        /// 业务类型ID
        /// </summary>
        public string BuTypeId
        {
            get { return _BuTypeId; }
            set
            {
                _BuTypeId = value;
            }
        }
        private string _BuTypeName;
        /// <summary>
        /// 业务类型
        /// </summary>
        public string BuTypeName
        {
            get { return _BuTypeName; }
            set
            {
                _BuTypeName = value;
            }
        }
        private MeasureServiceModel _mServiceModel;
        public MeasureServiceModel mServiceModel
        {
            get { return _mServiceModel; }
            set
            {
                _mServiceModel = value;
                if (value != null && value.rows != null && value.rows.Count == 1)//绑定页面数据
                {
                    CarTaskModel.BullInfo = value.rows.First();
                }
                else//为空的话 
                {
                    try
                    {
                        if (CarTaskModel == null)
                        {
                            CarTaskModel = new TaskModel();
                        }
                        CarTaskModel.BullInfo = new BullInfo(); ;
                    }
                    catch //(Exception ex)
                    {
                        CarTaskModel = new TaskModel();
                    }
                }
                DoShowWeightInfo();
            }
        }
        private TaskModel carTaskModel;
        /// <summary>
        /// 任务对象
        /// </summary>
        public TaskModel CarTaskModel
        {
            get { return carTaskModel; }
            set
            {
                carTaskModel = value;
                //显示重量
                DoShowWeightInfo();
                this.RaisePropertyChanged("CarTaskModel");
            }
        }
        private string sendTaskStr;
        /// <summary>
        /// 任务对象Json
        /// </summary>
        public string SendTaskStr
        {
            get { return sendTaskStr; }
            set
            {
                sendTaskStr = value;
            }
        }

        #endregion
        #region 动画控制

        private int _countdown;
        /// <summary>
        /// 倒计时
        /// </summary>
        public int countdown
        {
            get { return _countdown; }
            set
            {
                _countdown = value;
                this.RaisePropertyChanged("countdown");
            }
        }
        private bool _IsRedLineLeft;
        /// <summary>
        /// 开启左红外动画
        /// </summary>
        public bool IsRedLineLeft
        {
            get { return _IsRedLineLeft; }
            set
            {
                _IsRedLineLeft = value;
                this.RaisePropertyChanged("IsRedLineLeft");
                if (value)
                {
                    SetShowInfoMsg("业务处理中", "左红外被遮挡......", true);
                }
                else
                {
                    SetShowInfoMsg(lastMsg1, lastMsg2, false);
                }
            }
        }
        private bool _IsRedLineRight;
        /// <summary>
        /// 开启右红外动画
        /// </summary>
        public bool IsRedLineRight
        {
            get { return _IsRedLineRight; }
            set
            {
                _IsRedLineRight = value;
                this.RaisePropertyChanged("IsRedLineRight");
                if (value)
                {
                    SetShowInfoMsg("业务处理中", "右红外被遮挡......", true);
                }
                else
                {
                    SetShowInfoMsg(lastMsg1, lastMsg2, false);
                }
            }
        }
        private bool _IsTalking;
        /// <summary>
        /// 开启通话动画
        /// </summary>
        public bool IsTalking
        {
            get { return _IsTalking; }
            set
            {
                _IsTalking = value;
                this.RaisePropertyChanged("IsTalking");
            }
        }
        private bool _IsMeasuring;
        /// <summary>
        /// 远程处理中
        /// </summary>
        public bool IsMeasuring
        {
            get { return _IsMeasuring; }
            set
            {
                _IsMeasuring = value;
                this.RaisePropertyChanged("IsMeasuring");
            }
        }
        #endregion
        #region 硬件类注册
        private IoController ioc;//IO
        public WeightManager iwc;//表头
        private IcCardsController iic;//ic卡
        private RfidController rfid;//rfid卡
        public KeyBoardController ikb;//键盘注册
        //private PrinterController pcl;//打印
        private CheatManager cmc;//安国防作弊
        //VideoController tVideoController;//视频抓拍对象
        /// <summary>
        /// 写视频的摄像头
        /// </summary>
        public VideoController writeVideoController;
        #endregion
        #region 业务控制
        private eBullTag _BullState;
        /// <summary>
        /// 业务控制
        /// </summary>
        public eBullTag BullState
        {
            get { return _BullState; }
            set
            {
                try
                {
                    _BullState = value;
                    if (value == eBullTag.free)
                    {
                        //初始化数据
                        InitBusiness();
                    }
                    else if (value == eBullTag.weight)
                    {
                        //打开信号灯
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Msg = "计量业务:车上称" + LogConstParam.Draw_Line,
                            FunctionName = "称点主窗体_BullState业务控制",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Level = LogConstParam.LogLevel_Info
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        if (!IsRedLedOpend)
                        {
                            OpenRedLed();
                        }
                    }
                    else if (value == eBullTag.specification)
                    {
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Msg = "计量业务:验证称量规则" + LogConstParam.Draw_Line,
                            FunctionName = "称点主窗体_BullState业务控制",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Level = LogConstParam.LogLevel_Info
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        //启动ic卡和RFID卡
                        CheckBusinessRule();
                    }
                    else if (value == eBullTag.end)//和硬件连接时，没有下面这些代码
                    {
                        //测试临时使用
                        //isSend = false;
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Msg = "计量业务结束" + LogConstParam.Draw_Line,
                            FunctionName = "称点主窗体_BullState业务控制",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Level = LogConstParam.LogLevel_Info
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                    else if (value == eBullTag.error)
                    {
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Msg = "计量业务:错误" + LogConstParam.Draw_Line,
                            FunctionName = "称点主窗体_BullState业务控制",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Level = LogConstParam.LogLevel_Warning
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = "业务控制时错误：" + ex.Message + "堆栈：" + ex.StackTrace,
                        FunctionName = "称点主窗体_BullState业务控制",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Error,
                        Data = "BullState=" + BullState.ToString()
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }

            }
        }
        private bool _IsCheckRedLine;
        /// <summary>
        /// 是否检查红外
        /// </summary>
        public bool IsCheckRedLine
        {
            get { return _IsCheckRedLine; }
            set
            {
                _IsCheckRedLine = value;
            }
        }
        private bool _IsPrintPoundList;
        /// <summary>
        /// 是否打印 
        /// </summary>
        public bool IsPrintPoundList
        {
            get { return _IsPrintPoundList; }
            set
            {
                _IsPrintPoundList = value;
            }
        }
        private bool _IsOpenInputView;
        /// <summary>
        /// 是否打开输入窗体 
        /// </summary>
        public bool IsOpenInputView
        {
            get { return _IsOpenInputView; }
            set
            {
                _IsOpenInputView = value;
            }
        }
        private int _RandomData;
        /// <summary>
        /// 随机数(计量任务)
        /// </summary>
        public int RandomData
        {
            get { return _RandomData; }
            set
            {
                _RandomData = value;
            }
        }
        /// <summary>
        /// 服务返回随机数(监听消息)
        /// </summary>
        private Calculagraph randomDataTimer;
        private bool _IsNoReply;
        /// <summary>
        /// 检测任务服务器是否回复随机数
        /// </summary>
        private bool IsNoReply
        {
            get { return _IsNoReply; }
            set
            {
                _IsNoReply = value;
            }
        }

        private int sendTime;
        /// <summary>
        /// 重复发送表头清零的时间
        /// </summary>
        public int SendTime
        {
            get { return sendTime; }
            set
            {
                sendTime = value;
            }
        }
        private bool isSoftClear;
        /// <summary>
        /// 是否硬件清零
        /// </summary>
        public bool IsSoftClear
        {
            get { return isSoftClear; }
            set
            {
                isSoftClear = value;
            }
        }
        #endregion
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
        /// 用户自定义显示内容
        /// </summary>
        private string _UserDefineInfos;
        public string UserDefineInfos
        {
            get { return _UserDefineInfos; }
            set
            {
                _UserDefineInfos = value;
                this.RaisePropertyChanged("UserDefineInfos");
            }
        }

        /// <summary>
        /// 是否显示重量信息
        /// </summary>
        private Visibility _IsShowWeightInfo;
        public Visibility IsShowWeightInfo
        {
            get { return _IsShowWeightInfo; }
            set
            {
                _IsShowWeightInfo = value;
                this.RaisePropertyChanged("IsShowWeightInfo");
            }
        }
        /// <summary>
        /// 滚动显示内容
        /// </summary>
        public ScrollingTextControl userNotic;

        /// <summary>
        /// 滚动显示内容防LED
        /// </summary>
        //public LEDForm LEDShowForm;
        /// <summary>
        /// 是否显示滚动内容
        /// </summary>
        public bool IsUserNotice;
        #endregion
        #region 界面颜色
        /// <summary>
        /// 默认颜色
        /// </summary>
        private string _TopColor = "#FF0C215F";
        public string TopColor
        {
            get { return _TopColor; }
            set
            {
                _TopColor = value;
                this.RaisePropertyChanged("TopColor");
            }
        }
        private string _TopColor1 = "#FF09236E";
        public string TopColor1
        {
            get { return _TopColor1; }
            set
            {
                _TopColor1 = value;
                this.RaisePropertyChanged("TopColor1");
            }
        }
        private string _TopColor2 = "#FF072C93";
        public string TopColor2
        {
            get { return _TopColor2; }
            set
            {
                _TopColor2 = value;
                this.RaisePropertyChanged("TopColor2");
            }
        }
        /// <summary>
        /// 左部分颜色
        /// </summary>
        private string _LeftColor1 = "#FF060628";
        public string LeftColor1
        {
            get { return _LeftColor1; }
            set
            {
                _LeftColor1 = value;
                this.RaisePropertyChanged("LeftColor1");
            }
        }

        private string _LeftColor2 = "#FF0B3D4A";
        public string LeftColor2
        {
            get { return _LeftColor2; }
            set
            {
                _LeftColor2 = value;
                this.RaisePropertyChanged("LeftColor2");
            }
        }
        /// <summary>
        /// 右部分颜色
        /// </summary>
        private string _RightColor1 = "#FF060628";
        public string RightColor1
        {
            get { return _RightColor1; }
            set
            {
                _RightColor1 = value;
                this.RaisePropertyChanged("RightColor1");
            }
        }
        private string _RightColor2 = "#FF0B3D4A";
        public string RightColor2
        {
            get { return _RightColor2; }
            set
            {
                _RightColor2 = value;
                this.RaisePropertyChanged("RightColor2");
            }
        }
        /// <summary>
        /// IC颜色变换
        /// </summary>
        private string _IcImage = "Image/IC_Good.png";
        public string IcImage
        {
            get { return _IcImage; }
            set
            {
                _IcImage = value;
                this.RaisePropertyChanged("IcImage");
            }
        }
        #endregion

        private bool isFirstLogin = false;//是否已经登录，如果已经登录，即使接收到服务器relogin也不再发送登录……
        public bool isTest = false;//是否测试模式 2016-3-4 16:32:30……
        /// <summary>
        /// 移卡时的重量2016-3-22 15:09:10……
        /// </summary>
        decimal moveCardWeight = 0;
        /// <summary>
        /// 移卡之后的最小重量
        /// </summary>
        decimal moveCardMinWeight = 0;
        /// <summary>
        /// 移卡之后的最大重量
        /// </summary>
        decimal moveCardMaxWeight = 0;
        /// <summary>
        /// 磅差 5000固定值
        /// </summary>
        decimal moreOrLessWeight = 5000;
        /// <summary>
        /// 上磅之后的最大重量
        /// </summary>
        decimal maxWeight = 0;
        /// <summary>
        /// 打印机状态
        /// </summary>
        string printError = string.Empty;
        /// <summary>
        /// 是否移卡终止任务
        /// </summary>
        bool isMoveCardStopTask = false;
        /// <summary>
        /// 接收到坐席的保存重量或者终止时的信息
        /// </summary>
        BullInfo receiveBullInfo = new BullInfo();
        /// <summary>
        /// 将千克转为吨 解决后台存储与前台显示不一致的问题  例如后台存储千克 前台显示吨
        /// </summary>
        private decimal weightKg = Convert.ToDecimal(0.001);
        /// <summary>
        /// 上一次记录海康重量 
        /// </summary>
        decimal lastSaveHKWeight = -99999999;
        /// <summary>
        /// 最大允许千克
        /// </summary>
        decimal maxAllowWeight = 10000;
        /// <summary>
        /// 是不是打开IC卡时间，处理IC卡打开慢 影响判断取数是否稳定
        /// </summary>
        public bool isICOpen = false;
        /// <summary>
        /// 是不是打开RFID卡时间，处理IC卡打开慢 影响判断取数是否稳定
        /// </summary>
        //public bool isRFIDOpen = false;
        /// <summary>
        /// 操作日志记录类
        /// </summary>
        //LogsHelpClass logH = new LogsHelpClass();
        #endregion
        #region 显示的重量信息 由千克变为吨显示
        /// <summary>
        /// 毛重
        /// </summary>
        string showGross;

        public string ShowGross
        {
            get { return showGross; }
            set
            {
                showGross = value;
                this.RaisePropertyChanged("ShowGross");
            }
        }
        /// <summary>
        /// 皮重
        /// </summary>
        string showTare;

        public string ShowTare
        {
            get { return showTare; }
            set
            {
                showTare = value;
                this.RaisePropertyChanged("ShowTare");
            }
        }
        /// <summary>
        /// 净重
        /// </summary>
        string showSuttle;

        public string ShowSuttle
        {
            get { return showSuttle; }
            set
            {
                showSuttle = value;
                this.RaisePropertyChanged("ShowSuttle");
            }
        }
        /// <summary>
        /// 扣重
        /// </summary>
        string showDedution;

        public string ShowDedution
        {
            get { return showDedution; }
            set
            {
                showDedution = value;
                this.RaisePropertyChanged("ShowDedution");
            }
        }
        /// <summary>
        /// 供方毛重
        /// </summary>
        string showGrossb;

        public string ShowGrossb
        {
            get { return showGrossb; }
            set
            {
                showGrossb = value;
                this.RaisePropertyChanged("ShowGrossb");
            }
        }
        /// <summary>
        /// 供方皮重
        /// </summary>
        string showTareb;

        public string ShowTareb
        {
            get { return showTareb; }
            set
            {
                showTareb = value;
                this.RaisePropertyChanged("ShowTareb");
            }
        }
        /// <summary>
        /// 供方净重
        /// </summary>
        string showSuttleb;

        public string ShowSuttleb
        {
            get { return showSuttleb; }
            set
            {
                showSuttleb = value;
                this.RaisePropertyChanged("ShowSuttleb");
            }
        }
        #endregion
        #region 构造函数
        /// <summary>
        /// 页面构造方法
        /// </summary>
        public CarMeasureClientViewModel()
        {
            if (IsInDesignMode)
            {
                return;
            }
            try
            {
                ClearOldProcess();
                ClearPross("Talent.FIleSync");//先清除进程……
                ClearPross("Talent.CarMeasureConfig");//先清除进程……
                TaskServerConnState = false;
                CarNumber = string.Empty;
                stdClose();
                #region 日志初始化(日志初始化转移至App.xaml.cs中了,那里是启动页，也要写日志)
                //#region 系统日志初始化
                ////string logConfigPath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\Log4Net.config";
                //string logConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Log4Net.config";
                //Talent.ClinetLog.SysLog.CreateSysLog(logConfigPath);
                //#endregion
                //#region 实时数据日志初始化
                //Talent.ClinetLog.IwdLog.CreateIwdLog(logConfigPath);
                //#endregion
                //#region 称重量日志初始化
                //Talent.ClinetLog.WeightLog.CreateWeightLog(logConfigPath);
                //#endregion
                #endregion
                BullState = eBullTag.init;
                /// 获取配置信息Url
                GetConfigUrl();
                //下载配置文件
                if (_isDownLoadConfigFile)
                {
                    DownLoadConfigFile();
                }
                //初始化界面信息
                Thread t1 = new Thread(new ThreadStart(initCarMeasureInfo));
                t1.IsBackground = true;
                t1.Start();
            }
            catch (Exception ex)
            {
                SetShowInfoMsg(XTYCStr, "系统初始化信息失败！原因：" + ex.Message, true);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_VM中窗体初始化",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "系统初始化信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 获取配置信息Url
        /// </summary>
        private void GetConfigUrl()
        {
            string configSet = ConfigurationManager.AppSettings["SysConfigFileName"].ToString();
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string configUrl = basePath + configSet;
            this.configUrl = configUrl;
        }
        #endregion
        #region 页面初始化注册各种信息
        private void initCarMeasureInfo()
        {
            try
            {
                CarTaskModel = new TaskModel();
                TaskServerConnState = false;
                stdClose();
                #region 配置信息管理
                #region 系统初始化

                //SetShowInfoMsg("系统初始化中", "读取配置信息中......", false);
                if (FileHelper.IsExistFile(configUrl))//读取配置文件
                {
                    #region 初始化XPathl类
                    //XpathHelper xPathCls = new XpathHelper(configUrl);
                    #region 读取公司名称
                    string CompanyItem = ConfigurationManager.AppSettings["SysInfo"].ToString();
                    string getSysCompany = XpathHelper.GetValue(configUrl, CompanyItem);
                    if (!string.IsNullOrEmpty(getSysCompany))
                    {
                        sysCompany = getSysCompany;
                    }
                    #endregion
                    #region 启动重量
                    string BeginWeightItem = ConfigurationManager.AppSettings["BeginWeightInfo"].ToString();
                    string getBeginWeight = XpathHelper.GetValue(configUrl, BeginWeightItem);
                    if (!string.IsNullOrEmpty(getBeginWeight))
                    {
                        BeginWeight = CommonTranslationHelper.ToDecimal(getBeginWeight);
                    }
                    #endregion
                    #region 软件名称
                    string SoftItem = ConfigurationManager.AppSettings["SoftInfo"].ToString();
                    string getSoftItem = XpathHelper.GetValue(configUrl, SoftItem);
                    if (!string.IsNullOrEmpty(getSoftItem))
                    {
                        softName = getSoftItem;
                    }
                    #endregion
                    #region 计量单位
                    string WeightUnitItem = ConfigurationManager.AppSettings["WeightUnitInfo"].ToString();
                    string getWeightUnitItem = XpathHelper.GetValue(configUrl, WeightUnitItem);
                    if (!string.IsNullOrEmpty(getWeightUnitItem))
                    {
                        measurementUnitName = getWeightUnitItem;
                    }
                    #endregion
                    #region 初始化终端的ID,名称和终端IP等
                    string ClientId = ConfigurationManager.AppSettings["ClientIdInfo"].ToString();
                    string getClientIdItem = XpathHelper.GetValue(configUrl, ClientId);
                    if (!string.IsNullOrEmpty(getClientIdItem))
                    {
                        ClientInfo.ClientId = getClientIdItem;
                    }
                    string ClientCode = ConfigurationManager.AppSettings["ClientCodeInfo"].ToString();
                    string getClientCodeItem = XpathHelper.GetValue(configUrl, ClientCode);
                    if (!string.IsNullOrEmpty(getClientCodeItem))
                    {
                        ClientInfo.ClientCode = getClientCodeItem;
                    }
                    picPath = XpathHelper.GetValue(configUrl, @"/configlist/module[@name='系统设置']/submodule[@name='Ftp文件设置']/param[@name='PictureSavePath']/@value");//图片存放路径

                    string ClientName = ConfigurationManager.AppSettings["ClientNameInfo"].ToString();
                    string getClientNameItem = XpathHelper.GetValue(configUrl, ClientName);
                    if (!string.IsNullOrEmpty(getClientNameItem))
                    {
                        ClientInfo.Name = getClientNameItem;
                    }
                    //显示终端名称……lt2016-1-29 15:20:57
                    ShowClientName = getClientNameItem;
                    string ClientIp = ConfigurationManager.AppSettings["ClientIpInfo"].ToString();
                    string getClientIpItem = XpathHelper.GetValue(configUrl, ClientIp);
                    if (!string.IsNullOrEmpty(getClientIpItem))
                    {
                        ClientInfo.Ip = getClientIpItem;
                    }
                    LoadLoginUser();
                    #endregion
                    #region 计量业务启动方式
                    string Startup = ConfigurationManager.AppSettings["StartupInfo"].ToString();
                    string getStartupItem = XpathHelper.GetValue(configUrl, Startup);
                    if (!string.IsNullOrEmpty(getStartupItem))
                    {
                        StartupInfo = Convert.ToInt32(Enum.Parse(typeof(eStartup), getStartupItem.Replace("+", "")));
                    }
                    #endregion
                    #region 计量方式
                    string MeasureType = ConfigurationManager.AppSettings["MeasureTypeInfo"].ToString();
                    string getMeasureTypeItem = XpathHelper.GetValue(configUrl, MeasureType);
                    if (!string.IsNullOrEmpty(getMeasureTypeItem))
                    {
                        Convert.ToInt32(Enum.Parse(typeof(eMeasureType), getMeasureTypeItem));
                        MeasureTypeInfo = getMeasureTypeItem;
                    }
                    #endregion
                    #region 计量规则参数
                    //取数周期
                    string WeightTimePeriod = ConfigurationManager.AppSettings["WeightTimePeriodInfo"].ToString();
                    string getWeightTimePeriodInfo = XpathHelper.GetValue(configUrl, WeightTimePeriod);
                    if (!string.IsNullOrEmpty(getWeightTimePeriodInfo))
                    {
                        weightTimePeriod = Convert.ToInt32(getWeightTimePeriodInfo);
                    }
                    string MeasureTimePeriod = ConfigurationManager.AppSettings["MeasureTimePeriodInfo"].ToString();
                    string getMeasureTimePeriodInfo = XpathHelper.GetValue(configUrl, MeasureTimePeriod);
                    if (!string.IsNullOrEmpty(getMeasureTimePeriodInfo))
                    {
                        measureTimePeriod = Convert.ToInt32(getMeasureTimePeriodInfo);
                    }
                    //取数次数(S)
                    string WeightTimeCount = ConfigurationManager.AppSettings["WeightTimeCountInfo"].ToString();
                    string getWeightTimeCountInfo = XpathHelper.GetValue(configUrl, WeightTimeCount);
                    if (!string.IsNullOrEmpty(getWeightTimeCountInfo))
                    {
                        weightTimeCount = Convert.ToInt32(getWeightTimeCountInfo);
                    }
                    //允差
                    string WeightAllowance = ConfigurationManager.AppSettings["WeightAllowanceInfo"].ToString();
                    string getWeightAllowanceInfo = XpathHelper.GetValue(configUrl, WeightAllowance);
                    if (!string.IsNullOrEmpty(getWeightAllowanceInfo))
                    {
                        weightAllowance = Convert.ToDecimal(getWeightAllowanceInfo);
                    }
                    #endregion 计量规则参数
                    #region 注册记时时间
                    timer = new Calculagraph(StartupInfo);
                    timer.Timeout = weightTimePeriod;
                    timer.TimeOver += new TimeoutCaller(timer_TimeOver);
                    timer.timeTrigger += new EventHandler(TimeTriggerMethod);
                    #endregion
                    #region 监听任务送达时间
                    randomDataTimer = new Calculagraph(StartupInfo);
                    randomDataTimer.Timeout = measureTimePeriod;
                    randomDataTimer.TimeOver += new TimeoutCaller(randomtimer_TimeOver);
                    randomDataTimer.timeTrigger += new EventHandler(RandomTimeTriggerMethod);
                    #endregion

                    #region 注册重发任务的计时时间

                    //重发任务时间(S)
                    int resendTimer = 3;
                    string resendTimerItem = ConfigurationManager.AppSettings["ResendTaskTimer"].ToString();
                    string resendTimerStr = XpathHelper.GetValue(configUrl, resendTimerItem);
                    if (!string.IsNullOrEmpty(resendTimerStr))
                    {
                        resendTimer = Convert.ToInt32(resendTimerStr);
                    }
                    resendTaskTimer = new Calculagraph(StartupInfo);
                    resendTaskTimer.Timeout = resendTimer;
                    resendTaskTimer.TimeOver += new TimeoutCaller(resendTaskTimer_TimeOver);
                    #endregion

                    #region 任务回退计时器
                    backTaskTimer = new Calculagraph(StartupInfo);
                    backTaskTimer.Timeout = resendTimer;
                    backTaskTimer.TimeOver += new TimeoutCaller(backTaskTimer_TimeOver);
                    #endregion
                    #region 检测是否全屏
                    fullScreenTimer = new Calculagraph(null);
                    fullScreenTimer.Timeout = resendTimer;
                    fullScreenTimer.TimeOver += new TimeoutCaller(fullScreenTimer_TimeOver);
                    fullScreenTimer.Start();
                    #endregion

                    #region 自动确认倒计时时间
                    string confirmTime = ConfigurationManager.AppSettings["ConfirmTime"].ToString();
                    string confirmTimeItem = XpathHelper.GetValue(this.configUrl, confirmTime);
                    if (!string.IsNullOrEmpty(confirmTimeItem))
                    {
                        this.TimeCount = Int32.Parse(confirmTimeItem);
                    }
                    #endregion
                    #region 是否确定重量
                    string confirmWeight = ConfigurationManager.AppSettings["ConfirmWeight"].ToString();
                    string confirmWeightItem = XpathHelper.GetValue(configUrl, confirmWeight);
                    if (!string.IsNullOrEmpty(confirmWeightItem))
                    {
                        this.IsConfirmWeight = confirmWeightItem.Equals("是") ? true : false;
                    }
                    #endregion
                    #region 空秤文字
                    string kc = ConfigurationManager.AppSettings["KCStr"].ToString();
                    this.KCStr = XpathHelper.GetValue(configUrl, kc);
                    #endregion
                    #region 上秤文字
                    string sc = ConfigurationManager.AppSettings["SCStr"].ToString();
                    this.SCStr = XpathHelper.GetValue(configUrl, sc);
                    #endregion
                    #region 任务发送后
                    string rwfs = ConfigurationManager.AppSettings["RWFSStr"].ToString();
                    this.RWFSStr = XpathHelper.GetValue(configUrl, rwfs);
                    #endregion
                    #region 计量过程中
                    string jlz = ConfigurationManager.AppSettings["JLZStr"].ToString();
                    this.JLZStr = XpathHelper.GetValue(configUrl, jlz);
                    #endregion
                    #region 提示刷卡 SKStr
                    string jlsk = ConfigurationManager.AppSettings["SKStr"].ToString();
                    this.SKStr = XpathHelper.GetValue(configUrl, jlsk);
                    #endregion
                    #region 计量完成提示
                    string jlwc = ConfigurationManager.AppSettings["JLWCStr"].ToString();
                    this.JLWCStr = XpathHelper.GetValue(configUrl, jlwc);
                    #endregion
                    #region 下秤文字
                    string xc = ConfigurationManager.AppSettings["XCStr"].ToString();
                    this.XCStr = XpathHelper.GetValue(configUrl, xc);
                    #endregion
                    #region 远程处理文字
                    string yc = ConfigurationManager.AppSettings["YCStr"].ToString();
                    this.YCStr = XpathHelper.GetValue(configUrl, yc);
                    #endregion
                    #region 终止计量
                    string zz = ConfigurationManager.AppSettings["ZZStr"].ToString();
                    this.ZZStr = XpathHelper.GetValue(configUrl, zz);
                    #endregion
                    #region 系统提示
                    string xtts = ConfigurationManager.AppSettings["XTTSStr"].ToString();
                    this.XTTSStr = XpathHelper.GetValue(configUrl, xtts);
                    #endregion
                    #region 系统异常
                    string xtyc = ConfigurationManager.AppSettings["XTYCStr"].ToString();
                    this.XTYCStr = XpathHelper.GetValue(configUrl, xtyc);
                    #endregion
                    #region 是否标准键盘
                    string isStandardParam = ConfigurationManager.AppSettings["IsStandardBoard"].ToString();
                    string isStandardStr = XpathHelper.GetValue(configUrl, isStandardParam);
                    this.isStandardBoard = isStandardStr.Equals("是") ? true : false;
                    #endregion
                    #region 用户自定义显示内容 例如大厅电话……
                    string userDefineInfo = ConfigurationManager.AppSettings["USERDEFINEStr"].ToString();
                    string tempTel = XpathHelper.GetValue(configUrl, userDefineInfo);
                    string[] temp = tempTel.Split(new char[] { ' ' });
                    tempTel = temp[0];
                    if (temp.Length >= 2)
                    {
                        for (int i = 1; i < temp.Length; i++)
                        {
                            if (temp[i].Length > 0)
                            {
                                temp[i] = temp[i].PadLeft(temp[i].Length + 17);
                                tempTel += Environment.NewLine;
                                tempTel += temp[i];
                            }
                        }
                    }
                    this.UserDefineInfos = tempTel;
                    #endregion
                    #region 是否显示坐席发送终端的通知……
                    string isUserNotice = ConfigurationManager.AppSettings["IsUserNotice"].ToString();
                    this.IsUserNotice = (XpathHelper.GetValue(configUrl, isUserNotice)).Equals("是") ? true : false;
                    ShowUserNotic();
                    #endregion
                    #region 获取表头发送的时间
                    string SoftClearSendTime = ConfigurationManager.AppSettings["SoftClearSendTime"].ToString();
                    string SoftClearSendTimeItem = XpathHelper.GetValue(configUrl, SoftClearSendTime);
                    if (!string.IsNullOrEmpty(SoftClearSendTimeItem))
                    {
                        this.SendTime = CommonMethod.CommonTranslationHelper.ToInt(SoftClearSendTimeItem);
                    }
                    #endregion
                    #region 是否软件清零
                    string IsSoftClear = ConfigurationManager.AppSettings["IsSoftClear"].ToString();
                    string IsSoftClearItem = XpathHelper.GetValue(configUrl, IsSoftClear);
                    if (!string.IsNullOrEmpty(IsSoftClearItem))
                    {
                        this.IsSoftClear = IsSoftClearItem.Equals("是") ? true : false;
                    }
                    #endregion
                    #region 自动重启的时间
                    string getAutoRunTime = ConfigurationManager.AppSettings["AutoRunTime"].ToString();
                    string getAutoRunTimeItem = XpathHelper.GetValue(configUrl, getAutoRunTime);
                    if (!string.IsNullOrEmpty(getAutoRunTimeItem))
                    {
                        this.AutoRunTime = Int32.Parse(getAutoRunTimeItem);
                    }
                    #endregion
                    #region 是否保存所有重量曲线
                    string isSaveAllWeightData = ConfigurationManager.AppSettings["IsSaveAllWeightData"].ToString();
                    string isSaveAllWeightDataItem = XpathHelper.GetValue(configUrl, isSaveAllWeightData);
                    if (!string.IsNullOrEmpty(isSaveAllWeightDataItem))
                    {
                        this.IsSaveAllWeightData = isSaveAllWeightDataItem.Equals("是") ? true : false;
                    }
                    #endregion
                    #endregion
                    // SetShowInfoMsg("系统初始化中", "读取配置信息完成......", false);
                }
                else
                {
                    #region 日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "称点主窗体_读取配置文件配置",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "系统初始化信息失败！原因：配置文件缺失",
                        Origin = "汽车衡_" + ClientInfo.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    throw new Exception("配置文件缺失!");
                }
                //SetShowInfoMsg("系统初始化中", "准备初始化硬盘录像机......", false);
                //SyncDvrTime();
                //SetShowInfoMsg("系统初始化中", "初始化硬盘录像机完成......", false);
                //写视频……
                //SetShowInfoMsg("系统初始化中", "准备初始化写入重量视频......", false);
                SaveWeightVideo();//方法已被注释
                //SetShowInfoMsg("系统初始化中", "初始化写入重量视频完成......", false);
                //获取秤体最大量程                
                GetMaxAllowWeight();
                //MessageBox.Show("获取最大量程完成");
                //this.initRfidController(this.configUrl);
                #endregion
                #endregion
                #region 称量衡器的注册
                //SetShowInfoMsg("系统初始化中", "准备初始化衡器......", false);
                if (!initWeightController(configUrl))
                {
                    //BullState = eBullTag.error;
                    //return;
                }
                //SetShowInfoMsg("系统初始化中", "初始化衡器完成......", false);
                #endregion
                #region 注册硬件
                //注册打印机
                //initPrinterController(this.configUrl);
                //注册视频抓拍对象
                //InitVideoCapturePicture(this.configUrl);
                //SetShowInfoMsg("系统初始化中", "准备初始化IO控制设备......", false);
                this.initIocController(this.configUrl);
                //SetShowInfoMsg("系统初始化中", "初始化IO控制设备完成......", false);
                //SetShowInfoMsg("系统初始化中", "准备初始化键盘......", false);
                initKeyBoardController(this.configUrl);
                //SetShowInfoMsg("系统初始化中", "初始化键盘完成......", false);
                initCheatController(this.configUrl);//初始化安国防作弊
                #endregion
                #region 注册Socket
                //SetShowInfoMsg("系统初始化中", "客户端注册中...", false);
                initSocket(configUrl);
                //SetShowInfoMsg("系统初始化中", "客户端注册完成...", false);
                #endregion
            }
            catch (Exception ex)
            {
                SetShowInfoMsg(XTYCStr, ex.Message, true);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_初始化注册信息",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "系统初始化信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));

                #endregion
            }
        }
        #endregion
        #region Socket初始化
        private bool initSocket(string configUrl)
        {
            #region 读取任务服务器IP
            string TaskIpItem = ConfigurationManager.AppSettings["TaskIpInfo"].ToString();
            string getTaskIp = XpathHelper.GetValue(configUrl, TaskIpItem).ToString();
            #endregion
            #region 读取端口
            string SeatTaskItem = ConfigurationManager.AppSettings["TaskPortInfo"].ToString();
            int getTaskPort = CommonTranslationHelper.ToInt(XpathHelper.GetValue(configUrl, SeatTaskItem));
            #endregion
            #region 注册事件
            List<string> eventNameList = new List<string>();
            eventNameList.Add(ClientlistenCmdEnum.sendReply);
            eventNameList.Add(ClientlistenCmdEnum.sendCMD);
            eventNameList.Add(ClientlistenCmdEnum.sendMSG);
            eventNameList.Add(ClientlistenCmdEnum.relogin);
            eventNameList.Add(ClientlistenCmdEnum.reconn);
            eventNameList.Add(ClientlistenCmdEnum.loginok);
            eventNameList.Add(ClientSendCmdEnum.logout);
            eventNameList.Add(ClientlistenCmdEnum.reply);
            eventNameList.Add("listtask");
            SocketCls.ConnectServer(getTaskIp, getTaskPort, eventNameList, true);
            SocketCls.listenEvent += SocketCls_listenEvent;
            SocketCls.CallConnect += SocketCls_scoketClose;
            SocketCls.scoketError += SocketCls_scoketError;
            return SocketCls.s.IsConnected;
            #endregion
        }
        #endregion
        #region 任务服务器部分
        #region 重新连接任务服务器部分
        /// <summary>
        /// 重新连接任务服务器
        /// </summary>
        private void reconn()
        {
            //if (!isFirstLogin)
            {
                initSocket(ConfigUrl);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_InOut,
                    FunctionName = "称点主窗体_监听到socket命令reconn",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "收到任务服务器命令reconn,重新与任务服务器建立连接。",
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        #endregion
        #region Socket断开连接监听
        void SocketCls_scoketClose(object sender, EventArgs e)
        {
            SetShowInfoMsg(XTYCStr, "网络异常,请检查网络情况!", false);
            TaskServerConnState = false;
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体",
                Level = LogConstParam.LogLevel_Error,
                Msg = "任务服务器断开。",
                Origin = "汽车衡_" + ClientInfo.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        void SocketCls_scoketError(object sender, EventArgs e)
        {
            string errInfo = "Socket异常";
            try
            {
                SocketIOClient.ErrorEventArgs eea = e as SocketIOClient.ErrorEventArgs;
                errInfo = errInfo + ",原因:" + eea.Message;
            }
            catch { }

            if (TaskServerConnState)
            {
                SetShowInfoMsg(XTYCStr, "网络异常,请检查网络情况!", false);
                TaskServerConnState = false;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_SocketCls_scoketError",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = errInfo,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        #endregion
        #region Socket回调事件
        void SocketCls_listenEvent(object sender, CallBackEventArgs e)
        {
            #region 写日志
            //LogModel log = new LogModel()
            //{
            //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //    Direction = LogConstParam.Directions_In,
            //    FunctionName = "称点主窗体_SocketCls_listenEvent",
            //    Level = LogConstParam.LogLevel_Warning,
            //    Msg = "---------------得到任务服务器命令:" + e.EventName + "__" + e.Message,
            //    Origin = "汽车衡_" + ClientInfo.Name
            //};
            //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            //Console.WriteLine("得到任务服务器命令:" + e.EventName + "__" + e.Message + "_" + DateTime.Now.ToString());
            Console.WriteLine("得到命令:" + e.EventName + "__" + e.Message + "_" + DateTime.Now.ToString());
            switch (e.EventName)
            {
                case "sendCMD"://发命令
                    sendCMD(e.Message);
                    break;
                case "sendMSG"://发消息
                    sendMSG(e.Message);
                    break;
                case "relogin"://重新登陆
                    relogin();
                    break;
                case "reconn"://重新连接
                    reconn();
                    break;
                case "loginok"://登陆成功
                    loginok();
                    break;
                case "logout"://退出成功
                    TaskServerConnState = false;
                    break;
                    //当坐席发送结束任务命令时，任务服务器接收到，发送给称终端
                case "sendReply":
                    EndTask(e.Message);
                    break;
                case "reply":
                    EndReply(e.Message);
                    break;
                case "listtask"://获取等待的任务的各称点id集合
                    ReceiveListTask(e.Message);
                    break;
            }
        }

        /// <summary>
        /// 接收到任务服务器反馈的listtask命令的处理方法
        /// </summary>
        /// <param name="msg"></param>
        private void ReceiveListTask(string msg)
        {
            resendTaskTimer.Stop();
            isLastGetListTask = true;
            System.Threading.Thread.Sleep(1000);
            if ((msg.Contains("[") && msg.Contains("]")) || msg.Equals("0"))
            {
                CheckClientIdExist(msg);
            }
            else
            {
                isLastGetListTask = false;
                resendTaskTimer.Start();
            }
        }

        /// <summary>
        /// 检查称点ID是否存在
        /// </summary>
        /// <param name="jsonArrayStr"></param>
        private void CheckClientIdExist(string jsonArrayStr)
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_OutIn,
                FunctionName = "称点主窗体_检查称点ID是否存在_获得等待的任务对应的称点ID集合",
                Level = LogConstParam.LogLevel_Info,
                Msg = "收到服务器命令【listtask】,获取等待的任务对应的各称点id集合。",
                Origin = "汽车衡_" + ClientInfo.Name,
                Data = jsonArrayStr,
                IsDataValid = LogConstParam.DataValid_Ok
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            if (!string.IsNullOrEmpty(jsonArrayStr))
            {
                if (jsonArrayStr.Equals("[]") || jsonArrayStr.Equals("0"))
                {
                    #region 写日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_OutIn,
                        FunctionName = "称点主窗体_称点防止任务丢失_检测称点ID在任务列表中是否存在",
                        Level = LogConstParam.LogLevel_Warning,
                        Msg = "接收到等待的任务信息为空",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Data = jsonArrayStr,
                        IsDataValid = LogConstParam.DataValid_No
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                    //GetTaskStatues(TaskTime);
                    ResendTask();
                    return;
                }
                try
                {
                    var clientIds = JsonConvert.DeserializeObject<List<string>>(jsonArrayStr);
                    if (clientIds != null && clientIds.Count() > 0)
                    {
                        //arraying:排队中;处理中;
                        var status = clientIds.Where(r => r.Contains("处理中")).ToList();//判断是否为处理中,是：停止重发计时器（方法体外已经停止，这里不再启动即可）
                        if (status == null || status.Count == 0)
                        {
                            var arraying = clientIds.Where(r => r.Contains("arraying")).ToList();//判断是否为排队中,不是排队中,停止重发计时器（方法体外已经停止，这里不再启动即可）
                            if (arraying != null && arraying.Count > 0)
                            {
                                //排队中的情况下，再判断给的称点id集合里是否含有当前称点ID,若不含，说明任务在排队过程中丢失了,则重发
                                var curClientIdList = clientIds.Where(r => r.Contains(ClientInfo.ClientId)).ToList();
                                if (curClientIdList == null || curClientIdList.Count == 0)//当前称点ID在任务服务器给的集合里不存在,调用java服务查看任务状态
                                {
                                    //GetTaskStatues(TaskTime);
                                    ResendTask();
                                }
                                else
                                {
                                    isLastGetListTask = false;
                                    resendTaskTimer.Start();
                                }
                            }
                        }
                    }
                    else
                    {
                        isLastGetListTask = false;
                        resendTaskTimer.Start();
                    }
                }
                catch (Exception ex)
                {
                    isLastGetListTask = false;
                    resendTaskTimer.Start();
                    #region 写日志
                    LogModel log3 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_OutIn,
                        FunctionName = "称点主窗体_获得等待的任务对应的称点ID集合",
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "处理获取到的等待处理的任务对应的称点id集合异常,原因" + ex.Message,
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Data = jsonArrayStr,
                        IsDataValid = LogConstParam.DataValid_Ok
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
                    #endregion
                }
            }
        }

        /// <summary>
        /// 重发任务
        /// </summary>
        private void ResendTask()
        {
            if (this.Weight > BeginWeight)
            {
                TaskModel tm = JsonConvert.DeserializeObject<TaskModel>(SendTaskStr);
                //RandomData = CommonMethod.CommonMethod.GetRandom();//获取随机数字
                tm.msgid = RandomData;
                tm.CreateTime = DateTime.Parse(TaskTime);
                string paraJsonStr = JsonConvert.SerializeObject(tm);
                SendTaskStr = paraJsonStr;//重新改变msgid以后修改全局任务对象转化字符串(2016-09-24）                          
                SocketCls.Emit(ClientSendCmdEnum.measureData, SendTaskStr);
                #region 写日志
                LogModel log2 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "称点主窗体_重发任务",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "再次发送任务,Msgid=" + RandomData + ",matchid=" + TaskTime,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = tm,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                #endregion
            }
            isLastGetListTask = false;
            resendTaskTimer.Start();
        }

        /// <summary>
        /// 结束任务  保存图片
        /// </summary>
        private void EndTask(string msg)
        {
            if (Weight < BeginWeight || string.IsNullOrEmpty(this.IcCardNo))//小于启动重量，则不执行接收到的消息 2016-2-29 10:11:07……
            {
                return;
            }
            #region 写日志
            LogModel log2 = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "秤点主窗体_收到任务结束信号",
                Level = LogConstParam.LogLevel_Info,
                Msg = "准备向任务服务器发送任务清除命令",
                IsDataValid = LogConstParam.DataValid_Ok,
                Origin = "汽车衡_" + ClientInfo.Name,
                OperateUserName = IcCardNo
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
            #endregion
            isMoveCardStopTask = false;//收到坐席终止或者计量完成 不再判断是否移卡
            IsMeasuring = false;
            IsTalking = false;
            SendReplyModel srm = (SendReplyModel)InfoExchange.DeConvert(typeof(SendReplyModel), msg);
            receiveBullInfo = ((TaskModel)InfoExchange.DeConvert(typeof(TaskModel), srm.data.ToString())).BullInfo;
            saveMatchid = receiveBullInfo.matchid;
            if (string.IsNullOrEmpty(saveMatchid))
            {
                saveMatchid = srm.matchid;
            }
            //StopIc();
            saveWeight = true;
            if (srm.result == 1)
            {
                globTimerState = 5;//保存重量
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "秤点主窗体_收到任务结束信号",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "收到任务服务器命令【sendReply】,称量任务正常结束",
                    Data = srm,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    OperateUserName = IcCardNo
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                BullState = eBullTag.end;
                iwc.IsFinish = true;
                #region 变换界面颜色
                TopColor = "#FF0C215F";
                TopColor1 = "#FF09236E";
                TopColor2 = "#FF072C93";
                LeftColor1 = "#FF060628";
                LeftColor2 = "#FF5FEE4A";
                RightColor1 = "#FF060628";
                RightColor2 = "#FF5FEE4A";//ARGB，其中当A为FF时表示不透明，相反A为00时对象会完全透明，变得不可见
                #endregion
                //CloseRFID();
                SetShowInfoMsg(XTTSStr, JLWCStr, false);
                //StopIc();
                #region 保存图片
                try
                {
                    #region 准备抓图日志
                    LogModel pLog = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "称点主窗体_抓拍照片",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "开始抓拍照片"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(pLog));
                    #endregion
                    Thread thread = new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            #region wangchao 20171128注释并修改
                            //if (mServiceModel==null || mServiceModel.rows == null)
                            //{
                            //    SaveVideoPic("YC", "YC");
                            //}
                            //else
                            //{
                            //    SaveVideoPic(mServiceModel.rows[0].matchid ?? "", mServiceModel.rows[0].measurestate ?? "");
                            //}
                            if (receiveBullInfo == null)
                            {
                                SaveVideoPic(string.IsNullOrEmpty(saveMatchid) ? "YC" : saveMatchid, "YC");
                            }
                            else
                            {
                                SaveVideoPic(saveMatchid, receiveBullInfo.measurestate ?? "");
                            }
                            #endregion
                            #region 日志
                            LogModel log1 = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "称点主窗体_抓拍照片",
                                Origin = "汽车衡_" + ClientInfo.Name,
                                Data = saveMatchid + receiveBullInfo.measurestate,
                                Level = LogConstParam.LogLevel_Info,
                                Msg = "抓拍照片结束"
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            #region 日志
                            LogModel log1 = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "称点主窗体_抓拍照片",
                                Origin = "汽车衡_" + ClientInfo.Name,
                                Data = mServiceModel,
                                Level = LogConstParam.LogLevel_Error,
                                Msg = "抓拍照片异常。原因：" + ex.Message
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                            #endregion
                        }


                    }));
                    thread.IsBackground = true;
                    thread.Start();
                }
                catch (Exception ex)
                {
                    #region 准备抓图日志
                    LogModel pLog = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "称点主窗体_抓拍照片",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Error,
                        Msg = "抓拍照片异常,原因:" + ex.Message + "堆栈信息:" + ex.StackTrace
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(pLog));
                    #endregion
                }
                #endregion
            }
            else if (srm.result == 0)
            {
                SendBackTaskInfo();
                BullState = eBullTag.stop;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_获得任务结束信号",
                    Level = LogConstParam.LogLevel_Warning,
                    Msg = "收到任务服务器命令【sendReply】,坐席将任务终止",
                    Data = srm,
                    IsDataValid = LogConstParam.DataValid_No,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    OperateUserName = IcCardNo
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                TaskModel tm = (TaskModel)InfoExchange.DeConvert(typeof(TaskModel), srm.data.ToString());
                #region 变换界面颜色
                TopColor = "#FF0C215F";
                TopColor1 = "#FF09236E";
                TopColor2 = "#FF072C93";
                LeftColor1 = "#FF060628";
                LeftColor2 = "#FFFF3838";
                RightColor1 = "#FF060628";
                RightColor2 = "#FFFF3838";//ARGB，其中当A为FF时表示不透明，相反A为00时对象会完全透明，变得不可见
                #endregion
                string errmsg = tm.ErrorMsg.Split(new Char[] { '，', ',' })[0];
                SetShowInfoMsg(XTTSStr, errmsg ?? "", false);
                //StopIc();
            }

        }
        /// <summary>
        /// 核对随机数,结束监听
        /// </summary>
        private void EndReply(string msg)
        {
            if (!string.IsNullOrEmpty(msg) && msg.Length > 2 && msg.Substring(0, 2).Equals("b_"))
            {
                isBackTaskReply = true;
                this.backTaskTimer.Stop();
                System.Threading.Thread.Sleep(1000);
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "称点主窗体_任务回退随机数核对",
                    Level = LogConstParam.LogLevel_Warning,
                    Msg = "收到任务服务器返回的回退任务随机数:" + msg + "称点当前回退任务随机数为:" + MsgId,
                    Data = msg,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
                return;
            }
            if (msg == RandomData.ToString())
            {
                randomDataTimer.Stop();
                IsNoReply = false;
            }
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_OutIn,
                FunctionName = "称点主窗体_随机数核对",
                Level = LogConstParam.LogLevel_Warning,
                Msg = "收到任务服务器返回的任务随机数:" + msg + "称点当前随机数为:" + RandomData.ToString(),
                Data = msg,
                Origin = "汽车衡_" + ClientInfo.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        /// <summary>
        /// 设置窗体中显示的提示信息
        /// </summary>
        /// <param name="msg1"></param>
        /// <param name="msg2"></param>
        private void SetShowInfoMsg(string msg1, string msg2, bool IsErrorMsg)
        {
            try
            {
                if (_Msg2.Equals(msg2) && _Msg1.Equals(msg1))
                {
                    return;
                }
                if (_Msg2.Contains("系统已停用") || msg2.Contains("IC读卡器操作失败!"))//系统停用之后 不再滚动显示 其它的内容……2016-3-4 17:24:10
                {
                    return;
                }
                Msg1 = msg1;
                Msg2 = msg2;
                if (!IsErrorMsg)
                {
                    lastMsg1 = msg1;
                    lastMsg2 = msg2;
                }
                if (msg2.Contains("天利和"))
                {
                    return;
                }
                PlayVoice(msg2);
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "称点主窗体_设置窗体中显示的提示信息",
                    Level = LogConstParam.LogLevel_Warning,
                    Msg = "设置窗体显示信息或者语音播放出错：" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        #region 任务服务器返回信息
        private void sendCMD(string msg)
        {
            CommandParam cp = (CommandParam)InfoExchange.DeConvert(typeof(CommandParam), msg);
            LogModel log;
            switch (cp.msg.cmd)
            {
                //当坐席通过服务获取业务信息后，会向任务服务器发送UpdateInfo业务数据同步命令，使坐席和称点数据保持一致
                case "UpdateInfo"://业务数据同步

                    RefreshMeasureInfo(cp.msg.msg.ToString());
                    break;
                case "VoicePrompt"://语音提示
                    //if (Weight < BeginWeight)//小于启动重量，则不执行接收到的消息 2016-2-29 10:11:07……
                    //{
                    //    break;
                    //}
                    VoiceModel vm = InfoExchange.DeConvert(typeof(VoiceModel), cp.msg.msg.ToString()) as VoiceModel;
                    PlayVoice(vm.Content);
                    break;
                case "VoiceTalkStart":
                    IsTalking = true;
                    #region 写日志
                    log = new LogModel()
                    {
                        Origin = "汽车衡_" + ClientInfo.Name,
                        FunctionName = "称点主窗体_语音对讲",
                        Level = LogConstParam.LogLevel_Info,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        Msg = "接收到服务器命令【sendCMD】,业务类型【VoiceTalkStart】,语音对讲开始!",
                        Data = cp,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        OperateUserName = IcCardNo
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    break;
                case "VoiceTalkEnd":
                    IsTalking = false;
                    #region 写日志
                    log = new LogModel()
                    {
                        Origin = "汽车衡_" + ClientInfo.Name,
                        FunctionName = "称点主窗体_语音对讲",
                        Level = LogConstParam.LogLevel_Info,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        Msg = "接收到服务器命令【sendCMD】,业务类型【VoiceTalkEnd】,语音对讲结束!",
                        Data = cp,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        OperateUserName = IcCardNo
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    break;
                case "MeasureWeightClear":
                    BullState = eBullTag.free;
                    if (IsSoftClear)
                    {
                        iwc.ClearZero();
                    }
                    else
                    {
                        if (ClearWeightCloseIo())
                        {
                            #region 写日志
                            log = new LogModel()
                            {
                                Origin = "汽车衡_" + ClientInfo.Name,
                                FunctionName = "称点主窗体_表头清零",
                                Level = LogConstParam.LogLevel_Info,
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                Msg = "接收到服务器命令【sendCMD】,业务类型【MeasureWeightClear】,IO断电方式断电成功!",
                                Data = 1,
                                IsDataValid = LogConstParam.DataValid_Ok,
                                OperateUserName = IcCardNo
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            #endregion
                            System.Threading.Thread.Sleep(SendTime * 1000);
                            if (ClearWeightOpenIo())
                            {
                                #region 写日志
                                log = new LogModel()
                                {
                                    Origin = "汽车衡_" + ClientInfo.Name,
                                    FunctionName = "称点主窗体_表头清零",
                                    Level = LogConstParam.LogLevel_Info,
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Direction = LogConstParam.Directions_In,
                                    Msg = "接收到服务器命令【sendCMD】,业务类型【MeasureWeightClear】,IO断电方式供电成功!",
                                    Data = 1,
                                    IsDataValid = LogConstParam.DataValid_Ok,
                                    OperateUserName = IcCardNo
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                                #endregion
                                RestartClient(cp);
                                #region 写日志
                                log = new LogModel()
                                {
                                    Origin = "汽车衡_" + ClientInfo.Name,
                                    FunctionName = "称点主窗体_表头清零",
                                    Level = LogConstParam.LogLevel_Info,
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Direction = LogConstParam.Directions_In,
                                    Msg = "接收到服务器命令【sendCMD】,业务类型【MeasureWeightClear】IO断电方式供电成功,系统重启!",
                                    Data = 1,
                                    IsDataValid = LogConstParam.DataValid_Ok,
                                    OperateUserName = IcCardNo
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                                #endregion
                            }
                            else
                            {
                                #region 写日志
                                log = new LogModel()
                                {
                                    Origin = "汽车衡_" + ClientInfo.Name,
                                    FunctionName = "称点主窗体_表头清零",
                                    Level = LogConstParam.LogLevel_Info,
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Direction = LogConstParam.Directions_In,
                                    Msg = "接收到服务器命令【sendCMD】,业务类型【MeasureWeightClear】,IO断电方式供电失败!",
                                    Data = 1,
                                    IsDataValid = LogConstParam.DataValid_Ok,
                                    OperateUserName = IcCardNo
                                };
                                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                                #endregion
                            }
                        }
                        else
                        {
                            #region 写日志
                            log = new LogModel()
                            {
                                Origin = "汽车衡_" + ClientInfo.Name,
                                FunctionName = "称点主窗体_表头清零",
                                Level = LogConstParam.LogLevel_Info,
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                Msg = "接收到服务器命令【sendCMD】,业务类型【MeasureWeightClear】,IO断电方式断电失败!",
                                Data = 1,
                                IsDataValid = LogConstParam.DataValid_Ok,
                                OperateUserName = IcCardNo
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            #endregion
                        }
                    }
                    break;
                case "ClientRestart":
                    BullState = eBullTag.free;
                    string newMsgId = "b_" + CommonMethod.CommonMethod.GetRandom();
                    SocketCls.Emit(ClientSendCmdEnum.backtask, newMsgId);
                    //initCarMeasureInfo();
                    #region 写日志
                    //log = new LogModel()
                    //{
                    //    Origin = "汽车衡_" + ClientInfo.Name,
                    //    FunctionName = "称点主窗体_衡器客户端重启",
                    //    Level = LogConstParam.LogLevel_Info,
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_In,
                    //    Msg = "接收到服务器命令【sendCMD】,业务类型【ClientRestart】,终端重启!",
                    //    Data = cp,
                    //    IsDataValid = LogConstParam.DataValid_Ok,
                    //    OperateUserName = IcCardNo
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));

                    RestartClient(cp);

                    #endregion


                    break;
                //当坐席保存重量完成后，会向任务服务器发送打印命令（正常打印）
                //2.坐席可以在查询节点发送补打命令，称点进行补打
                #region 接收到坐席的数据
                //{"clientid":"104","cmd":"Supplement",
                //"msg":{"matchid":"8018080100255","opname":"樊军","opcode":"14610","clientcode":"104","clientname":"原料80t","carno":"冀EF0579","printtype":"正常","TicketType":0},"msgid":643},
                //"ParamList":[{"ParamName":"cmd","ParamValue":"cmd2client"}],"IsDataValid":"有效"}
                #endregion:
                case "Supplement":
                    SupplementTicket(cp.msg.msg.ToString());
                    break;
                case "ClientUpdate"://方法已被注释，不再使用
                    AutoUpdateVersion();
                    #region 写日志
                    log = new LogModel()
                    {
                        Origin = "汽车衡_" + ClientInfo.Name,
                        FunctionName = "称点主窗体_衡器客户端升级",
                        Level = LogConstParam.LogLevel_Info,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        Msg = "接收到服务器命令【sendCMD】,业务类型【ClientUpdate】," + ClientInfo.Name + "版本更新!",
                        Data = cp,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        OperateUserName = IcCardNo
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    break;
                //暂停计量
                case "ClientStop":
                    ClientStop(cp);
                    break;
                //接收到通知消息（中间的滚动信息）
                case "UserNotice":
                    GetUserNotice(cp);
                    break;
                case "ClientState":
                    SetClientState(cp);
                    break;
                case "FullScreen":
                    SetFullScreen();
                    break;
            }
        }



        /// <summary>
        /// 设置称点全屏
        /// </summary>
        private void SetFullScreen()
        {
            IsFullScreenEnable = true;//cs中有可用为真情况下，设置窗体全屏的代码...
        }

        /// <summary>
        /// 称点版本更新方法实现（当前不用）
        /// </summary>
        private void AutoUpdateVersion()
        {
            //string FtpAddress = ConfigurationManager.AppSettings["FtpAddress"].ToString();
            ////string FtpPort = ConfigurationManager.AppSettings["FtpPort"].ToString();
            //string FtpUserName = ConfigurationManager.AppSettings["FtpUserName"].ToString();
            //string FtpPassWord = ConfigurationManager.AppSettings["FtpPassWord"].ToString();

            //Updater.PreClosed += new EventHandler(PreUpdate);

            //Updater.SetDownLoadParam(FtpAddress, FtpUserName, FtpPassWord);
            //Updater.CheckUpdateStatus();
        }
        private void PreUpdate(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                SocketCls.Emit(ClientSendCmdEnum.logout, "");//退出
                if (SocketCls.s != null)
                {
                    SocketCls.s.Dispose();
                }
                Application.Current.Shutdown();
            });
        }
        private void sendMSG(string msg)
        {
            SetShowInfoMsg("", msg, false);
            if (msg.Contains("处理中"))
            {
                isLastGetListTask = true;
                resendTaskTimer.Stop();
            }
        }
        /// <summary>
        /// 登录任务服务器 向任务服务器发送客户端信息
        /// </summary>
        private void relogin()
        {
            //if (!isFirstLogin)
            {
                var Clientobj = new { clientid = ClientInfo.ClientId, name = ClientInfo.Name };
                string loginCmd = InfoExchange.ToJson(Clientobj);
                //System.Threading.Thread.Sleep(3000);
                SocketCls.Emit(ClientSendCmdEnum.login, loginCmd);
                isFirstLogin = true;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_InOut,
                    FunctionName = "称点主窗体_登录任务服务器",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "收到任务服务器命令【relogin】,请求重新登录任务服务器。",
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }

        }
        /// <summary>
        /// 登录成功
        /// </summary>
        private void loginok()
        {
            TaskServerConnState = true;
            SendRealDataCountInZero = 0;//重新登陆后连续发实时数据.防止称点出现的断开现象
            //SetShowInfoMsg("", "任务服务器登录成功", false);
            if (BullState == eBullTag.init)
            {
                BullState = eBullTag.free;
            }
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_OutIn,
                FunctionName = "称点主窗体_登录任务服务器成功",
                Level = LogConstParam.LogLevel_Info,
                Msg = "收到任务服务器命令【loginok】,登录任务服务器成功。",
                Origin = "汽车衡_" + ClientInfo.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion

        }
        /// <summary>
        /// 服务送达完毕事件（randomtimer没有start方法，当前没有使用）
        /// </summary>
        /// <param name="userdata"></param>
        void randomtimer_TimeOver(object userdata)
        {//默认监听时间完毕，服务未发送随机数,再次启动服务发送
            if (IsNoReply)
            {
                SocketCls.Emit(ClientSendCmdEnum.measureData, SendTaskStr);
                SaveSendTask("未监听到服务器返回又发送：" + "msgid:" + RandomData);
                MeasureTimerStart();
            }
        }

        /// <summary>
        /// 上次请求的listtask是否有响应
        /// </summary>
        private bool isLastGetListTask = false;

        /// <summary>
        /// 重发任务计时器事件(检测送达的任务是否存在)
        /// 仔细研究
        /// </summary>
        /// <param name="userdata"></param>
        void resendTaskTimer_TimeOver(object userdata)
        {
            if (!isLastGetListTask)
            {
                SocketCls.Emit("listtask", "");
                resendTaskTimer.Start();
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "称点主窗体_重发任务计时器计时时间到",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "向任务服务器发送了【listtask】请求,计时器重新启动",
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
            }
            //else
            //{
            //    resendTaskTimer.Stop();
            //}

            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "称点主窗体_重发任务计时器_resendTaskTimer_TimeOver", //ShowClientName,
                Level = LogConstParam.LogLevel_Info,
                Msg = "向任务服务器发送请求等待处理任务的命令:listtask",
                Origin = "汽车衡_" + ClientInfo.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        /// <summary>
        /// 任务回退计时器计时时间到
        /// </summary>
        /// <param name="userdata"></param>
        void backTaskTimer_TimeOver(object userdata)
        {
            if (!isBackTaskReply)
            {
                MsgId = "b_" + CommonMethod.CommonMethod.GetRandom();
                Console.WriteLine("任务回退计时器重发任务回退命令" + "_msgid:" + MsgId + "_" + DateTime.Now.ToString());
                //内部约定，回退任务时，参数为msgid，且magid是以"b_"开头,用以在redis监听的endreply中区分任务发送后的msgid和回退任务msgid
                SocketCls.Emit(ClientSendCmdEnum.backtask, MsgId);
                backTaskTimer.Start();
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "称点主窗体_向任务服务器发送任务清除命令",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "任务回退计时器调用命令【" + ClientSendCmdEnum.backtask + "】向任务服务器发送回退任务信号。",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = ClientInfo.ClientId,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }

        }

        /// <summary>
        /// 是否全屏检测时间到
        /// （超时，计时器停止，HasStarted为false，这时重新启动计时器）
        /// </summary>
        /// <param name="userdata"></param>
        void fullScreenTimer_TimeOver(object userdata)
        {
            //if (!IsFullScreenEnable)
            //{
            //    IsFullScreenEnable = true;//cs中有可用为真情况下，设置窗体全屏的代码...
            //}
            IsFullScreenEnable = true;
            /*System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if(!window.isActiveWindow())
                {
                    foreach(Window w in window.OwnedWindows)
                    {
                        w.Activate();
                        return;
                    }
                    window.Activate();
                }

            }));*/
            if (!fullScreenTimer.HasStarted)
            {
                fullScreenTimer.Start();
            }
        }

        #endregion
        #endregion
        /// <summary>
        /// 发送计量数据给任务服务器
        /// </summary>
        /// <param name="EquTag">设备标识</param>
        private void SendRealDataToServer(string EquTag)
        {
            try
            {
                WeighterStates WeighterState = default(WeighterStates);
                if (BullState == eBullTag.free)
                {
                    WeighterState = WeighterStates.Free;
                }
                else if (BullState == eBullTag.metering)//正在计量
                {
                    WeighterState = WeighterStates.Working;
                }
                else if (BullState == eBullTag.end)//计量完成
                {
                    WeighterState = WeighterStates.EndTask;
                }
                else if (BullState == eBullTag.stop)//终止计量
                {
                    WeighterState = WeighterStates.StopTask;
                }
                else
                {
                    if (string.IsNullOrEmpty(CarNumber))
                    {
                        WeighterState = WeighterStates.NoReaderCard;
                    }
                    else
                    {
                        WeighterState = WeighterStates.Wait;//Working 改为 wait lt 2016-2-18 16:42:29……
                    }

                }
                //if (Weight == 0)
                //{
                //    if (SendRealDataCountInZero<10)
                //    {
                //        SendRealDataCountInZero += 1;
                //        return;
                //    }
                //    else
                //    {
                //        SendRealDataCountInZero =0;
                //    }
                //}
                //else
                //{
                //    SendRealDataCountInZero = 0;
                //}

                WeighterClientModel paramObj = new WeighterClientModel()
                {
                    ClientId = ClientInfo.ClientId,
                    ClientName = ClientInfo.Name,
                    LeftLightState = IsRedLedOpend ? LeftLightStates.Red : LeftLightStates.Green,
                    RightLightState = IsRedLedOpend ? RightLightStates.Red : RightLightStates.Green,
                    ClientState = WeighterState,
                    Weight = this.Weight,
                    EquTag = EquTag,
                    WeightMsg = WeightMsg,
                    LeftLine = IsRedLineLeft ? Visibility.Visible : Visibility.Hidden,
                    RightLine = IsRedLineRight ? Visibility.Visible : Visibility.Hidden,
                    PrintState = printError,
                    RfidStrs = GetRfidStr()
                };
                
                string paraJsonStr = InfoExchange.ToJson(paramObj);
                //发送称点状态数据给任务服务器
                SocketCls.Emit(ClientSendCmdEnum.realData, paraJsonStr);
                //if (EquTag.Equals("RLL") || EquTag.Equals("RLR"))
                //{
                #region 写日志
                //LogModel log = new LogModel()
                //{
                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    Direction = LogConstParam.Directions_Out,
                //    FunctionName = "称点主窗体_发送实时数据",
                //    Level = LogConstParam.LogLevel_Info,
                //    Msg = "调用命令【" + ClientSendCmdEnum.realData + "】发送实时数据给任务服务器",
                //    Origin = "汽车衡_" + ClientInfo.Name,
                //    Data = paramObj,
                //    IsDataValid = LogConstParam.DataValid_Ok,
                //    ParamList = new List<DataParam>() { new DataParam() { ParamName = "clientId", ParamValue = ClientInfo.ClientId } },
                //    OperateUserName = this.IcCardNo
                //};
                //Talent.ClinetLog.IwdLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //}
            }
            catch (Exception ex)
            {
                //SetShowInfoMsg(XTYCStr, "向任务服务器发送实时称量信息失败", false);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "称点主窗体_发送实时数据",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "调用命令【" + ClientSendCmdEnum.realData + "】发送实时数据给任务服务器失败,原因:" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    OperateUserName = IcCardNo,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "clientId", ParamValue = ClientInfo.ClientId } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        //private bool isSend = false;
        /// <summary>
        ///  发送任务给任务服务器
        /// 自动计量时，发送计量数据给服务，服务异常，发送给坐席。
        /// 手动计量时，发送计量数据给坐席，由坐席调用服务处理
        /// </summary>
        /// <param name="ErrorMsg">提示信息</param>
        /// <param name="ServiceResult">服务结果</param>
        /// <param name="IsBusinessInfoQuery">是否调用服务</param>
        /// <param name="isHelpCmd">是否求助</param>
        public void SendTaskToServer(string ErrorMsg, string ServiceResult, bool IsBusinessInfoQuery, bool isHelpCmd)
        {
            IsMeasuring = true;
            isEnterHelp = true;
            //isEnterOnly = false;
            RandomData = CommonMethod.CommonMethod.GetRandom();//获取随机数字
            try
            {
                MeasureServiceModel msm = InfoExchange.DeConvert(typeof(MeasureServiceModel), ServiceResult) as MeasureServiceModel;
                if (!string.IsNullOrEmpty(msm.data.carNo) && !string.IsNullOrEmpty(CarNumber) && !msm.data.carNo.Equals(CarNumber))//当前刷卡车号与发送车号不一致
                {
                    SetShowInfoMsg(XTYCStr, "发送任务车号：" + msm.data.carNo + "与当前车号：" + CarNumber + "不一致", true);
                    return;
                }
                msm.data.rfidId = GetRfidStr();
                TaskModel carTaskModel = new TaskModel()
                {
                    ClientId = ClientInfo.ClientId,
                    ClientCode = ClientInfo.ClientCode,
                    ClientName = ClientInfo.Name,
                    CreateTime = DateTime.Now,
                    MeasureType = MeasureTypeInfo,
                    ErrorMsg = ErrorMsg,
                    Weight = RealWeight,
                    IsBusinessInfoQuery = IsBusinessInfoQuery,
                    ServiceModel = msm,
                    msgid = RandomData,
                    IcId = IcCardNo,
                    CarNumber = this.CarNumber,
                    IsHelpCmd = true
                };
                this.TaskTime = carTaskModel.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string paraJsonStr = JsonConvert.SerializeObject(carTaskModel);
                SendTaskStr = paraJsonStr;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "称点主窗体_发送任务给任务服务器",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "RandomData:" + RandomData + "," + paraJsonStr,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = ServiceResult,
                    IsDataValid = LogConstParam.DataValid_No,
                    OperateUserName = IcCardNo
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                SocketCls.Emit(ClientSendCmdEnum.measureData, paraJsonStr);
                SaveSendTask("首次发送：" + "msgid:" + RandomData + "发送来源：" + ErrorMsg);
                IsNoReply = true;
                isLastGetListTask = false;
                resendTaskTimer.Start();//任务发送计时器开始
                MeasureTimerStart();
                #region 写日志
                LogModel log2 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "称点主窗体",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用命令~" + ClientSendCmdEnum.measureData + "~向任务服务器发送任务成功。",
                    Origin = ClientInfo.Name,
                    Data = carTaskModel,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    OperateUserName = IcCardNo
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                #endregion
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "称点主窗体_发送任务给任务服务器",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "发送任务异常：" + ex.Message + ";堆栈:" + ex.StackTrace,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = ServiceResult,
                    IsDataValid = LogConstParam.DataValid_No,
                    OperateUserName = IcCardNo
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 计时器开始
        /// </summary>
        public void MeasureTimerStart()
        {
            //randomDataTimer.Start();
        }
        /// <summary>
        /// 任务服务返回随机数,监听停止
        /// </summary>
        private void MeasureTimerStop()
        {
            try
            {
                randomDataTimer.Stop();
            }
            catch //(Exception ex)
            {

            }
        }
        #endregion
        #region 注册硬件信息
        #region 称量衡量注册
        private bool initWeightController(string configUrl)
        {
            try
            {
                //MessageBox.Show("准备注册衡器事件");
                iwc = new WeightManager(configUrl);
                iwc.OnReceivedWeightData += new ReceivedWeightData(iwc_OnReceivedWeightData);
                iwc.OnShowErrorMsg += iwc_OnShowErrorMsg;
                if (iwc.Open())
                {
                    //MessageBox.Show("秤体打开");
                    iwc.Start();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "称量衡量注册时错误：" + ex.Message + "堆栈：" + ex.StackTrace,
                    FunctionName = "称点主窗体_称量衡量注册",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                return false;
            }
        }
        /// <summary>
        /// 称量衡器报错
        /// </summary>
        /// <param name="msg"></param>
        void iwc_OnShowErrorMsg(ErrorType pErrorType, string pMsg)
        {
            try
            {
                WeightMsg = pMsg;
                SetShowInfoMsg(XTYCStr, "衡器异常,原因：" + pMsg, true);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_称点衡器_iwc_OnShowErrorMsg",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "称量衡器异常:" + pMsg,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    OperateUserName = IcCardNo
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
                    FunctionName = "称点主窗体_称量衡器_iwc_OnShowErrorMsg",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "处理称量衡器返回的异常信息时异常,原因:" + ex.Message + "堆栈：" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    OperateUserName = IcCardNo
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }

        }
        /// <summary>
        /// 衡器重量
        /// </summary>
        /// <param name="pTag">错误标识</param>
        /// <param name="pWeight">重量</param>
        /// <param name="pHData">重量16定制字符串</param>
        void iwc_OnReceivedWeightData(string pTag, string pWeight, string pHData)
        {
            try
            {
                lastGetWeightDate = DateTime.Now;
                Weight = CommonTranslationHelper.ToDecimal(pWeight);
                WeightMsg = "";
                #region 衡器重量日志
                Talent.ClinetLog.WeightLog.Log(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  获得衡器重量:" + Weight);
                #endregion

                if (pTag != "1")
                {
                    WeightMsg = "超重!";
                    Talent.ClinetLog.WeightLog.Log(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  衡器异常,错误原因:" + WeightMsg);
                    SetShowInfoMsg(XTYCStr, "衡器异常,原因：" + WeightMsg + "请下秤!", true);
                    SendRealDataToServer("W");
                }
            }
            catch (Exception ex)
            {
                Talent.ClinetLog.WeightLog.Log("处理iwc_OnReceivedWeightData接收到的衡器重量数据时异常,重量:" + pWeight + ";异常信息：" + ex.Message + ";堆栈：" + ex.StackTrace);
            }

        }
        #endregion
        #region IC卡注册
        /// <summary>
        /// 初始化IC卡，打开IC卡  ，打开后调用RFID卡的初始化方法
        /// </summary>
        /// <param name="configUrl"></param>
        /// <returns></returns>
        private bool initIcController(string configUrl)
        {
            LogModel log;
            try
            {
                if (!isICOpen)//IC未打开标记
                {
                    iic = new IcCardsController(configUrl, true);
                    iic.OnReadCardNo += iic_OnReadCardNo;
                    iic.OnShowErrorMsg += iic_OnShowErrorMsg;
                    #region 准备打开IC卡log
                    log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "称点主窗体_IC卡注册_initIcController",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Msg = "Ic卡注册完毕,准备打开IC卡" + LogConstParam.Draw_Line
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion

                    isICOpen = iic.Open();

                    #region 打开IC卡后日志
                    log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "称点主窗体_IC卡注册_initIcController",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Msg = "打开IC卡返回：" + isICOpen + LogConstParam.Draw_Line
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
                if (isICOpen)
                {
                    IcImage = "Image/IC_Good.png";
                    #region 日志
                    log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "称点主窗体_IC卡注册_initIcController",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Msg = "启动IC卡寻卡,iic.Start()"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    iic.Start();
                }
                else
                {
                    IcImage = "Image/IC_Bad.png";
                    SetShowInfoMsg(XTYCStr, "IC读卡器操作失败!打开读卡器失败", true);
                    #region 日志
                    log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FunctionName = "称点主窗体_IC卡注册_initIcController",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Msg = "IC读卡器操作失败!打开读卡器失败"
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
            catch (Exception ex)
            {
                IcImage = "Image/IC_Bad.png";
                SetShowInfoMsg(XTYCStr, "IC读卡器操作失败!打开读卡器失败:" + ex.Message, true);
                #region 日志
                log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "称点主窗体_IC卡注册_initIcController",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Msg = "IC卡注册、打开、启动过程中异常,原因:" + ex.Message + " 堆栈：" + ex.StackTrace
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            //Thread th = new Thread(initRfidControllerNoParam);
            //th.Start();
            initRfidControllerNoParam(configUrl);
            return isICOpen;
        }
        /// <summary>
        /// IC报错
        /// </summary>
        /// <param name="msg"></param>
        void iic_OnShowErrorMsg(string msg)
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体_IC卡异常_iic_OnShowErrorMsg",
                Level = LogConstParam.LogLevel_Error,
                Msg = "IC读写失败！原因：" + msg,
                Origin = "汽车衡_" + ClientInfo.Name,
                OperateUserName = IcCardNo
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion

            if (msg.Contains("临时刷卡读取不到卡号信息重启"))
            {
                //isTempRestart = true;
                return;
            }
            SetShowInfoMsg(XTYCStr, "IC读卡器操作失败！原因：" + msg, true);
        }
        /// <summary>
        /// ic读卡
        /// </summary>
        /// <param name="pComPortNo"></param>
        /// <param name="pCardNo"></param>
        public void iic_OnReadCardNo(string pComPortNo, string pCardNo)
        {
            ReceiveIcNo(pCardNo);
        }

        /// <summary>
        /// 接收到卡号后的处理方法
        /// IC卡接收到卡号后，关闭RFID接收
        /// </summary>
        public void ReceiveIcNo(string icCardNo, string pComPortNo = "")
        {
            //IcCardNo = "B35CADE2";// pCardNo;
            bool isAllowSendTask = false;
            try
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "界面收到IC卡卡号为【：" + icCardNo + "】",
                    FunctionName = "称点主窗体_IC读卡",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion

                if (string.IsNullOrEmpty(icCardNo))
                {
                    SetShowInfoMsg("异常提示", "IC读卡异常,请再次刷卡!", false);
                    return;
                }
                StopIc();
                //StopRFID();
                CloseRFID();
                //if (!isTest)// 测试 刷卡时 2016-3-4 16:34:42……
                {
                    icCardNo = DoMHCardId(icCardNo);//转为16进制 2016-3-3 11:58:51……
                }
                //pCardNo = Convert.ToInt64(pCardNo).ToString("X8");//转为16进制 2016-3-3 11:58:51……
                IcCardNo = icCardNo;

                isEnterHelp = false;
                isEnterCancel = false;
                isAllowSendTask = CheckIsAllowSendTask();//判断是不是允许发任务
            }
            catch (Exception ex)
            {
                SetShowInfoMsg(XTYCStr, "卡信息出现错误：请按自助求助远程", true);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "称点主窗体_IC读卡",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "处理反馈的IC卡信息时异常,原因:" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            if (isAllowSendTask)
            {
                isMoveCardStopTask = false;
                // iic.Stop();
                //CloseIc();
                getCardServiceInfo(icCardNo, "0");//判断IC卡是否有效
                //getBllRulesServiceInfo("鲁GM8925", "", "", 0, "", ClientInfo.ClientId, 1);
            }
        }

        /// <summary>
        /// 处理明华读卡器
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        private string DoMHCardId(string cardId)
        {
            string cardStr = string.Empty;
            try
            {
                string str = Convert.ToInt64(cardId).ToString("X8");
                cardStr = str.Substring(6, 2) + str.Substring(4, 2) + str.Substring(2, 2) + str.Substring(0, 2);
            }
            catch //(Exception ex)
            {
                cardStr = cardId;
            }

            return cardStr;
        }

        /// <summary>
        /// iic移卡操作 2016-3-22 13:43:12
        /// </summary>
        /// <param name="pComPortNo"></param>
        public void iic_OnRemoveCard(string pComPortNo)
        {
            //SendBackTaskInfo();
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                FunctionName = "称点主窗体_IC卡移卡",
                Level = LogConstParam.LogLevel_Info,
                Msg = "检测到卡被移动  车号：" + CarNumber + "   IC卡号：" + IcCardNo + " ",
                Origin = "汽车衡_" + ClientInfo.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion

            if (iic != null)
            {
                #region 写日志
                LogModel cLog = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "称点主窗体_IC卡移卡",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "司机移卡：车号：" + CarNumber + "   IC卡号：" + IcCardNo + " ",
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(cLog));
                #endregion

                SetShowInfoMsg("系统提示", "请插入IC卡!", false);
                isMoveCardStopTask = true;
            }
            CarNumber = string.Empty;
            measureServiceResult = string.Empty;
            mServiceModel = new MeasureServiceModel();
            ClearGridReaderInfos();
            DoGridInitColors();
            moveCardWeight = Weight;
            moveCardMinWeight = Weight;
            moveCardMaxWeight = Weight;
            if (iic != null)
            {
                iic.Start();
            }
            isEnterOnly = false;
        }

        #endregion
        #region 称重计时控制
        /// <summary>
        /// 计时器开始,系统开始采集数据
        /// </summary>
        public void TimerStart()
        {
            if (BullState == eBullTag.specification)
            {
                listWeights.Clear();
                IsGetWeight = true;
                timer.Start();
            }
        }
        /// <summary>
        /// 计时器停止,系统不采集数据
        /// </summary>
        private void TimerStop()
        {
            try
            {
                timer.Stop();
                IsGetWeight = false;
            }
            catch //(Exception ex)
            {

            }
        }
        #endregion


        #region RFID卡注册
        /// <summary>
        /// 初始化RFID卡  （其实启动方式为IC卡，该方法并未被调用，initRfidControllerNoParam被调用）
        /// </summary>
        /// <param name="configUrl"></param>
        public void initRfidController(string configUrl)
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Msg = "准备打开RFID",
                FunctionName = "称点主窗体_RFID注册",
                Origin = "汽车衡_" + ClientInfo.Name,
                Level = LogConstParam.LogLevel_Info
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion

            try
            {
                rfid = new RfidController(configUrl);
                if (rfid != null && rfid.Open())
                {
                    rfid.Start();
                }
                ////读取配置
                //ConfigReader cfgReader = new ConfigReader(configUrl);
                //List<RfidCfg> curCfgList = ConfigReader.ReadListRfidConfig();
                //for (int i = 0; i < curCfgList.Count; i++)
                //{
                //    RfidCfg rf = curCfgList[i];
                //    if (i == 0)
                //    {
                //        if (rfid == null)
                //        {
                //            rfid = new RfidController(rf);
                //            rfid.OnReceivedData += rfid_onReceivedData;
                //            rfid.OnShowErrorMsg += rfid_OnShowErrorMsg;
                //        }
                //        rfid.Open();
                //        rfid.Start();
                //    }
                //    if (i == 1)
                //    {
                //        if (rfid1 == null)
                //        {
                //            rfid1 = new RfidController(rf);
                //            rfid1.OnReceivedData += rfid1_onReceivedData;
                //            rfid1.OnShowErrorMsg += rfid1_OnShowErrorMsg;
                //        }
                //        rfid1.Open();
                //        rfid1.Start();
                //    }

                //}
                // if (rfid == null)
                // {
                //     rfid = new RfidController(configUrl, true);
                //     FileHelpClass.WriteLog("实例化RfidController：", "RFID");
                //     rfid.OnReceivedData += rfid_onReceivedData;
                //     rfid.OnShowErrorMsg += rfid_OnShowErrorMsg;
                // }
                //bool rtO= rfid.Open();
                //FileHelpClass.WriteLog("OpenRfidController：", "RFID");
                //bool rtS= rfid.Start();
                //FileHelpClass.WriteLog("StartRfidController：", "RFID");
                //FileHelpClass.WriteLog("打开RFID设备rtO：" + rtO + "rtS:" + rtS, "RFID");
            }
            catch (Exception ex)
            {
                #region 写日志
                log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "打开RFID设备出错:" + ex.Message,
                    FunctionName = "称点主窗体_RFID注册",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 初始化RFID卡 （当前使用）
        /// </summary>
        /// <param name="pConfig"></param>
        public void initRfidControllerNoParam(string pConfig)
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Msg = "准备打开RFID",
                FunctionName = "称点主窗体_RFID注册",
                Origin = "汽车衡_" + ClientInfo.Name,
                Level = LogConstParam.LogLevel_Info
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            //System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
            try
            {
                if (rfid == null)
                {
                    rfid = new RfidController(pConfig);
                    rfid.OnReceivedData += rfid_onReceivedData;
                    rfid.OnShowErrorMsg += rfid_OnShowErrorMsg;
                }

                if (rfid.Open())
                {
                    if (!rfid.Start())
                    {
                        log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            FunctionName = "称点主窗体_RFID卡注册_initRFIDController",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Msg = "RFID读卡器启动失败：" + LogConstParam.Draw_Line
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    }
                }
                #region 打开RFID卡后日志

                #endregion
                //SetShowInfoMsg(XTYCStr, "RFID读卡器操作失败!打开读卡器失败", true);
            }
            catch (Exception ex)
            {
                #region 写日志
                log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "打开RFID设备异常：" + ex.Message,
                    FunctionName = "称点主窗体_RFID注册",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            //}));

        }
        /// <summary>
        /// FIID卡报错
        /// </summary>
        /// <param name="msg"></param>
        void rfid_OnShowErrorMsg(string msg)
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Msg = "RFID读卡器内部错误：" + msg,
                FunctionName = "称点主窗体_收到rfid错误信息_rfid_OnShowErrorMsg",
                Origin = "汽车衡_" + ClientInfo.Name,
                Level = LogConstParam.LogLevel_Warning
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion

            //SetShowInfoMsg(XTYCStr, "RFID卡读取失败！原因：" + msg, false);
            //#region 写日志
            //LogModel log = new LogModel()
            //{
            //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //    Direction = LogConstParam.Directions_In,
            //    FunctionName = "称点主窗体",
            //    Level = LogConstParam.LogLevel_Error,
            //    Msg = "RFID卡读取失败！原因:" + msg,
            //    Origin = "汽车衡_" +ClientInfo.Name,
            //    OperateUserName = IcCardNo
            //};
            //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            //#endregion
        }
        void rfid_onReceivedData(List<string> pCardNoList)
        {
            //RFIDCardNo = pCardNoList;
            for (int i = 0; i < pCardNoList.Count; i++)
            {
                string rfidNos = pCardNoList[i];
                if (!string.IsNullOrEmpty(rfidNos))
                {
                    if (!RFIDCardNo.Contains(rfidNos))
                    {
                        RFIDCardNo.Add(rfidNos);
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Msg = "RFID0检测到标签：" + rfidNos,
                            FunctionName = "称点主窗体_收到rfid信息_rfid_onReceivedData",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Level = LogConstParam.LogLevel_Info
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                }
            }
            SendRealDataToServer("RFID");  //发送实时数据

        }
        #endregion
        #region io 红外控制
        private bool initIocController(string configUrl)
        {
            try
            {
                ioc = new IoController(configUrl);
                ioc.OnShowErrMsg += ioc_OnShowErrMsg;
                ioc.OnReceiveAlarmSignal += ioc_OnReceiveAlarmSignal;
                if (ioc != null)
                {
                    // OpenGreenLed();//注册完打开红灯
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "秤体：" + this.ShowClientName + "注册IO设备失败：" + ex.Message,
                    FunctionName = "称点主窗体_IO控制注册",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return false;
        }
        /// <summary>
        /// io报错
        /// </summary>
        /// <param name="msg"></param>
        void ioc_OnShowErrMsg(string msg)
        {
            //SetShowInfoMsg(XTYCStr, "IO控制异常!" + msg, true);
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体_获得IO返回的错误信息",
                Level = LogConstParam.LogLevel_Error,
                Msg = "IO控制异常！原因:" + msg,
                Origin = "汽车衡_" + ClientInfo.Name,
                OperateUserName = IcCardNo
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        void ioc_OnReceiveAlarmSignal(string pDeviceCode, string pValue)
        {
            string var = pDeviceCode + pValue;
            //System.Diagnostics.Debug.WriteLine(var);
            if (pDeviceCode.Equals(DeviceConst.Left_Infrared_Correlation))
            {
                bool TempValue = pValue == "1" ? true : false;
                #region 写日志
                //LogModel log = new LogModel()
                //{
                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    Msg = "返回的数据pValue:" + pValue + ";this.IsRedLineLeft:" + IsRedLineLeft,
                //    FunctionName = "称点主窗体_获取到左红外对射",
                //    Origin = "汽车衡_" + ClientInfo.Name,
                //    Data = this.Weight,
                //    Level = LogConstParam.LogLevel_Info
                //};
                //Talent.ClinetLog.IwdLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                if (this.IsRedLineLeft != TempValue)
                {
                    this.IsRedLineLeft = TempValue;
                    SendRealDataToServer("RLL");
                    System.Threading.Thread.Sleep(100);
                    SendRealDataToServer("RLL");
                }
            }
            if (pDeviceCode.Equals(DeviceConst.Right_Infrared_Correlation))
            {
                bool TempValue = pValue == "1" ? true : false;
                #region 写日志
                //LogModel log = new LogModel()
                //{
                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    Msg = "返回的数据pValue:" + pValue + ";this.IsRedLineRight:" + IsRedLineRight,
                //    FunctionName = "称点主窗体_获取到右红外对射",
                //    Origin = "汽车衡_" + ClientInfo.Name,
                //    Data = this.Weight,
                //    Level = LogConstParam.LogLevel_Info
                //};
                //Talent.ClinetLog.IwdLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                if (this.IsRedLineRight != TempValue)
                {
                    this.IsRedLineRight = TempValue;
                    SendRealDataToServer("RLR");
                    System.Threading.Thread.Sleep(100);
                    SendRealDataToServer("RLR");
                }
            }
        }
        #endregion
        #region 键盘注册
        private void initKeyBoardController(string configUrl)
        {
            ikb = new KeyBoardController(configUrl);
            ikb.OnReceivedKeyData += ikb_OnReceivedKeyData;
            ikb.Open();

        }
        /// <summary>
        /// 目前此段代码 不执行  执行 
        /// </summary>
        /// <param name="pType"></param>
        /// <param name="pData"></param>
        /// <param name="pCommand"></param>
        void ikb_OnReceivedKeyData(KeyDataType pType, string pData, KeyCommand pCommand)
        {
            if (pType == KeyDataType.COMMAND && !IsOpenInputView)//表示命令
            {
                ikb.Stop();
                if (pCommand == KeyCommand.OK && KeyUseRange.NoRange == pData)//按了确定键(无限制范围)
                {
                    saveWeightServiceInfo();
                }
                else if (pCommand == KeyCommand.OK && KeyUseRange.Metering == pData && BullState != eBullTag.free)
                {
                    saveWeightServiceInfo();
                }
                else if (pCommand == KeyCommand.CANCEL && KeyUseRange.NoRange == pData)
                {
                    tipMsg();//提示状态
                    stdClose();//关闭时间动画
                }
                else if (pCommand == KeyCommand.CANCEL && KeyUseRange.Metering == pData && BullState != eBullTag.free)
                {
                    tipMsg();//提示状态
                    stdClose();//关闭时间动画
                }
                else if (pCommand == KeyCommand.HELP && KeyUseRange.NoRange == pData)//去掉同时判断 && IsMeasuring 2016-2-29 11:29:48……
                {
                    SendTaskToServer("按键远程求助", measureServiceResult, true, true);
                }
                else if (pCommand == KeyCommand.HELP && KeyUseRange.Metering == pData && BullState != eBullTag.free)//&& IsMeasuring
                {
                    SendTaskToServer("按键远程求助1", measureServiceResult, true, true);
                }
            }
        }
        #endregion
        #region 打印机注册
        //private void initPrinterController(string configUrl)
        //{
        //    pcl = new PrinterController(configUrl);
        //    pcl.OnShowErrMsg += pc_OnShowErrMsg;
        //}

        #endregion
        #region 视频抓拍注册
        //private void InitVideoCapturePicture(string configUrl)
        //{
        //    tVideoController = new VideoController(configUrl);
        //    tVideoController.Open();
        //}
        #endregion
        #region 安国防作弊
        private bool initCheatController(string configUrl)
        {
            try
            {
                cmc = new CheatManager(configUrl);
                cmc.OnReceivedCheatData += new ReceivedCheatData(cmc_OnReceivedCheatData);
                cmc.OnShowErrorMsg += cmc_OnShowErrorMsg;
                if (cmc.Open())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "安国防作弊注册时错误：" + ex.Message + "堆栈：" + ex.StackTrace,
                    FunctionName = "称点主窗体_安国防作弊注册",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                return false;
            }
        }
        /// <summary>
        /// 安国防作弊接收数据
        /// </summary>
        /// <param name="pRawData"></param>
        void cmc_OnReceivedCheatData(string pRawData)
        {
            if (!string.IsNullOrEmpty(pRawData))
                SaveCheatData(pRawData);
        }

        /// <summary>
        /// 安国防作弊报错
        /// </summary>
        /// <param name="msg"></param>
        void cmc_OnShowErrorMsg(string eMsg)
        {
            try
            {
                //SetShowInfoMsg(XTYCStr, "安国防作弊异常,原因：" + eMsg, true);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_安国防作弊_cmc_OnShowErrorMsg",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "安国防作弊异常:" + eMsg,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    OperateUserName = IcCardNo
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
                    FunctionName = "称点主窗体_安国防作弊_cmc_OnShowErrorMsg",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "处理安国防作弊返回的异常信息时异常,原因:" + ex.Message + "堆栈：" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    OperateUserName = IcCardNo
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }

        }
        /// <summary>
        /// 调用保存安国防作弊消息服务
        /// </summary>
        /// <param name="msg"></param>
        void SaveCheatData(string msg)
        {
            LogModel log;
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["SaveCheatMsg"].ToString().Replace('$', '&');
                string getUrl = string.Format(serviceUrl, msg, ClientInfo.Name, CarNumber, IcCardNo);
                #region 日志
                log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_安国防作弊_SaveCheatData",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = msg,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用保存安国防作弊消息服务",
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "URL", ParamValue = serviceUrl } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                string strResultSResult = ComHelpClass.ResponseSynStr(request);
                MeasureServiceModel getServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResultSResult) as MeasureServiceModel;
                if (getServiceModel.success)
                {
                    #region 日志
                    log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "称点主窗体_安国防作弊_SaveCheatData",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "调用保存安国防作弊消息服务成功",
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
            catch (Exception e)
            {
                #region 写日志
                log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_安国防作弊_SaveCheatData",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "保存安国防作弊返回的信息时异常,原因:" + e.Message,
                    Origin = "汽车衡_" + ClientInfo.Name,
                    OperateUserName = IcCardNo
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 关闭安国防作弊
        /// </summary>
        public void CloseCheat()
        {
            try
            {
                if (cmc != null)
                {
                    #region 写日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = "准备关闭安国防作弊。",
                        FunctionName = "称点主窗体_关闭安国防作弊",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Info
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                    cmc.Close();
                    #region 写日志
                    LogModel Log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = "安国防作弊关闭完成。",
                        FunctionName = "称点主窗体_关闭安国防作弊",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Info
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(Log));
                    #endregion
                    cmc = null;
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel eLog = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "安国防作弊异常:" + ex.Message,
                    FunctionName = "称点主窗体_关闭安国防作弊",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(eLog));
                #endregion
            }
        }
        #endregion
        #endregion
        #region 计量规则判断方法和服务调用
        #region 计量相关方法
        /// <summary>
        /// 判断称量前的业务规则是否满足
        /// 根据计量启动方式，启动相应的设备进行校验。（如重量+IC卡）
        /// </summary>
        private void CheckBusinessRule()
        {
            switch (StartupInfo)
            {
                case 0://重量启动
                    TimerStart();
                    break;
                case 1://IC卡启动  重量+IC卡 （现场用的这种启动方式）
                    if (this.initIcController(this.configUrl))
                    {
                        //SetShowInfoMsg("", this.SKStr, false);//提示刷卡信息                       
                    }
                    break;
                case 2://Rfid启动  重量+RFID卡
                    this.initRfidController(this.configUrl);
                    SetShowInfoMsg("", this.SKStr, false);//提示刷卡信息
                    break;
                case 3://Ic+Rfid启动  重量+IC卡+RFID卡
                    this.initIcController(this.configUrl);
                    this.initRfidController(this.configUrl);
                    SetShowInfoMsg("", this.SKStr, false);//提示刷卡信息
                    break;
                case 4://Ic或Rfid启动  （没有这种情况）
                    this.initIcController(this.configUrl);
                    this.initRfidController(this.configUrl);
                    SetShowInfoMsg("", this.SKStr, false);//提示刷卡信息
                    break;
                default:
                    SetShowInfoMsg(XTYCStr, "无法识别的计量业务~启动方式~,请核查配置文件!", true);
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        FunctionName = "称点主窗体_判断业务规则",
                        Level = LogConstParam.LogLevel_Warning,
                        Msg = "无法识别的计量业务【启动方式】,请核查配置文件!",
                        Origin = "汽车衡_" + ClientInfo.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    break;
            }
        }
        /// <summary>
        ///按秒执行事件实现的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeTriggerMethod(object sender, EventArgs e)
        {
            if ((this.IsRedLineLeft || this.IsRedLineRight) && IsCheckRedLine)
            {
                listWeights.Clear();
                timer.Reset();
                IsGetWeight = true;
            }
        }
        private void RandomTimeTriggerMethod(object sender, EventArgs e)
        {


        }
        /// <summary>
        /// 重量启动方式，才会执行，当前不执行
        /// 取数完毕事件(重量取数完毕后启动第一个服务)
        /// 取数计时器，取数计时器结束后，即在一个取数周期内，是否取到了稳定的重量，如果没有，则开始下一个取数周期
        /// 若多个取数周期达到规定次数weightTimeCount依旧没有取到稳定重量，则转坐席
        /// 取到稳定总量后，则，调用服务
        /// </summary>
        /// <param name="userdata"></param>
        void timer_TimeOver(object userdata)
        {
            temWeightTimeCount++;
            TimerStop();
            if (CheckWeight())//重量稳定
            {
                globTimerState = 5;//标记开始保存重量
                ///调用第三个服务
                saveWeightServiceInfo();
            }
            else
            {
                if (temWeightTimeCount > weightTimeCount)//取数据超过规定次数,转发坐席
                {
                    SetShowInfoMsg(XTYCStr, "未取到稳定重量已转远程", false);
                    //转坐席
                    SendTaskToServer("未取到稳定重量", measureServiceResult, false, false);
                }
                else
                {
                    TimerStart();//从新取数据
                }
            }
        }
        /// <summary>
        /// 倒计时动画结束后调用的方法
        /// </summary>
        public void AfterAnimation()
        {
            TimerStart();//计量时间启动
        }
        /// <summary>
        /// 检查称重是否合格
        /// </summary>
        private bool CheckWeight()
        {
            if (listWeights.Count > 0 && (listWeights.Max() - listWeights.Min()) <= this.weightAllowance)
            {
                var Weight = listWeights.Sum() / listWeights.Count;
                RealWeight = Decimal.Parse(Weight.ToString("#0.000"));
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 重量是否大于起始重量
        /// </summary>
        /// <returns></returns>
        private bool WeightCompare()
        {
            bool getResult = Weight > BeginWeight ? true : false;
            return getResult;
        }
        /// <summary>
        /// 业务开始前做的初始化工作
        /// </summary>
        private void InitBusiness()
        {
            try
            {
                #region 写日志
                LogModel log1 = new LogModel()
                {
                    Origin = ClientInfo.Name,
                    FunctionName = "称点主窗体_业务初始开始前",
                    Level = LogConstParam.LogLevel_Info,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    Msg = "carLeave=" + carLeave + " saveWeight=" + saveWeight,
                    IsDataValid = LogConstParam.DataValid_Ok,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
                //SendBackTaskInfo();
                carLeave = true;//标记车是否下称
                saveWeight = false;//标记是否保存重量记录
                countdown = 0;
                globTimerState = 0;
                listWeights.Clear();
                MeasureTimerStop();
                TimerStop();
                IsNoReply = false;
                RealWeight = 0M;
                // iwc.IsFinish = false;
                IcCardNo = string.Empty;
                CarNumber = string.Empty;
                RFIDCardNo = new List<string>();
                mServiceModel = new MeasureServiceModel();
                OpenGreenLed();
                DoGridInitColors();
                isAllowEnterOk = false;
                ClearGridReaderInfos();
                receiveBullInfo = new BullInfo();//接收到的坐席保存或者终止时业务信息……
                SetShowInfoMsg(XTTSStr, this.KCStr, false);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "计量业务开始" + LogConstParam.Draw_Line,
                    FunctionName = "称点主窗体_业务开始前的初始化",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
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
                    Msg = "初始化信息错误：" + ex.Message,
                    FunctionName = "称点主窗体_业务开始前的初始化",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 刷新称点信息
        /// </summary>
        /// <param name="msg"></param>
        private void RefreshMeasureInfo(string msg)
        {
            try
            {
                if (Weight < BeginWeight)//小于启动重量，则不执行接收到的消息 2016-2-29 10:11:07……
                {
                    return;
                }
                if (!string.IsNullOrEmpty(msg))
                {
                    CarTaskModel = InfoExchange.DeConvert(typeof(TaskModel), msg) as TaskModel;
                    if (CarTaskModel != null)
                    {
                        mServiceModel = new MeasureServiceModel() { rows = new List<BullInfo>() { CarTaskModel.BullInfo } };
                        mServiceModel.mores = CarTaskModel.mores;
                    }
                    #region 根据业务字段初始化界面
                    //ShowMessage("", "收到接收的信息：" + mServiceModel.mores.Count+"matchid:"+mServiceModel.rows[0].matchid, false, false);
                    RenderUI(mServiceModel);
                    DoDecimalShowInfos();//处理小数点显示的问题……lt 2016-2-15 11:07:12……       
                    #endregion
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        Origin = "汽车衡_" + ClientInfo.Name,
                        FunctionName = "称点主窗体_刷新称点信息",
                        Level = LogConstParam.LogLevel_Info,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        Msg = "接收到服务器命令【sendCMD】,业务类型【UpdateInfo】,刷新称点业务信息",
                        Data = CarTaskModel,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        OperateUserName = IcCardNo
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "刷新称点信息错误：" + ex.Message,
                    FunctionName = "称点主窗体_刷新称点信息",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        #endregion
        #region 动画控制
        private void stdOpen()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                           {
                               std.Begin();
                           }));
        }
        private void stdClose()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                std.Stop();
            }));
        }
        #endregion
        #region 服务调用

        /// <summary>
        /// 获取发送的任务状态
        /// </summary>
        /// <param name="taskTime">发送任务的时间</param>
        private void GetTaskStatues(string taskTime)
        {
            string serviceUrl = ConfigurationManager.AppSettings["getTaskStatus"].ToString().Replace('$', '&');
            string getUrl = string.Format(serviceUrl, taskTime, ClientInfo.ClientCode);
            #region 日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体_获取任务状态(getTaskStatus方法)",
                Level = LogConstParam.LogLevel_Info,
                Msg = getUrl,
                Origin = "汽车衡_" + ClientInfo.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            string resultStr = WebHttpHelpClass.HttpGet(getUrl, string.Empty);
            #region 日志
            LogModel log1 = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体_反馈的任务状态信息",
                Level = LogConstParam.LogLevel_Info,
                Msg = resultStr,
                Origin = "汽车衡_" + ClientInfo.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
            #endregion
            GetTaskStatuesCallback(resultStr);
        }

        /// <summary>
        /// 获取任务状态单步的方法
        /// </summary>
        /// <param name="asyc"></param>
        private void GetTaskStatuesCallback(string taskStatusJsonStr)
        {
            try
            {
                JavaServiceModel<TaskStatusModel> serviceModel = InfoExchange.DeConvert(typeof(JavaServiceModel<TaskStatusModel>), taskStatusJsonStr) as JavaServiceModel<TaskStatusModel>;
                //没有任务状态信息时，重发任务
                if (serviceModel.data == null || string.IsNullOrEmpty(serviceModel.data.taskstatus))
                {
                    ResendTask();
                }
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_获取任务状态",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取任务状态回调函数(GetTaskStatuesCallback)处理时失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 判断卡有效性验证，获取卡的信息
        /// </summary>
        /// <param name="RecordType">（0 表示IC卡、1表示RFID卡、2表示车号）</param>
        /// <param name="No">号</param>
        private void getCardServiceInfo(string CardNo, string No)
        {
            try
            {
                //SendBackTaskInfo();
                //SetShowInfoMsg("业务处理中", "验证卡信息中...", false);
                string serviceUrl = ConfigurationManager.AppSettings["getCardInfo"].ToString().Replace('$', '&');
                string getUrl = string.Format(serviceUrl, CardNo, No, StartupInfo, ClientInfo.ClientCode);
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                request.BeginGetResponse(new AsyncCallback(ReadCallback), request);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_判断卡有效性验证_getCardInfo",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = getUrl,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用卡验证服务"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            catch (Exception ex)
            {
                SetShowInfoMsg(XTYCStr, "验证卡信息出现错误：请按自助求助远程", true);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_判断卡有效性验证_getCardInfo",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "卡有效性验证信息失败！原因：" + ex.Message + "堆栈信息：" + ex.StackTrace,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        private void ReadCallback(IAsyncResult asyc)
        {
            try
            {
                MeasureServiceModel mServiceModel;
                string strResultSResult = ComHelpClass.ResponseStr(asyc);
                measureServiceResult = strResultSResult;               
                mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResultSResult) as MeasureServiceModel;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_卡有效性验证(getCardInfo)回调方法",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = mServiceModel,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用卡验证服务返回值"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                ////////////////////
                //// 1.判断卡状态(异常转坐席)
                //// 2.判断业务逻辑
                if (mServiceModel != null && mServiceModel.success)//调用服务成功
                {
                    CarNumber = mServiceModel.data.carNo;//车号
                    saveCarNo = CarNumber;
                    #region 写日志
                    log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = "车号:" + CarNumber + " IC卡号：" + IcCardNo + "刷卡成功" + "卡类型为：" + mServiceModel.mfunc,
                        FunctionName = "称点主窗体_卡有效性验证(getCardInfo)回调方法",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Info
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    if (mServiceModel.mfunc == 2)//2代表操作员卡
                    {
                        #region 写日志
                        log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Msg = " IC卡号：" + IcCardNo + "为操作员卡,不再发送任务",
                            FunctionName = "称点主窗体_卡有效性验证(getCardInfo)回调方法",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Level = LogConstParam.LogLevel_Info
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        SetShowInfoMsg("系统提示", "监秤卡刷卡成功", false);
                        measureServiceResult = string.Empty;
                        mServiceModel = null;
                        return;
                    }
                    if (mServiceModel.data.flag == 1)//卡正常
                    {
                        //StartupInfo为1 ic启动 启动方式为现场自助
                        if (StartupInfo == 1 || StartupInfo == 2)//IC启动或FRID卡 
                        {
                            if (MeasureTypeInfo == eMeasureType.远程计量.ToString())
                            {
                                //转坐席
                                SendTaskToServer("远程计量任务", strResultSResult, false, false);
                            }
                            else  //现场自助的计量方式
                            {
                                string getRfidId = GetRfidStr();
                                ////调用第二个服务
                                getBllRulesServiceInfo(CarNumber, IcCardNo, getRfidId, 0, "", ClientInfo.ClientId, 1);
                            }

                        }
                        else if (StartupInfo == 3)//双卡启动 
                        {
                            if ((!string.IsNullOrEmpty(mServiceModel.data.rfidId) && (mServiceModel.data.rfidId == rfidId)) || (string.IsNullOrEmpty(mServiceModel.data.rfidId)))
                            {
                                ////需要验证RFIDCard卡，且RFIDCard卡是正确的 或不需要验证 RFIDCard
                                if (MeasureTypeInfo == eMeasureType.远程计量.ToString())
                                {
                                    //转坐席
                                    SendTaskToServer("双卡启动远程计量任务", strResultSResult, false, false);
                                }
                                else
                                {
                                    ////调用第二个服务 
                                    string getRfidId = GetRfidStr();
                                    getBllRulesServiceInfo(CarNumber, IcCardNo, getRfidId, 0, "", ClientInfo.ClientId, 1);
                                }
                            }
                            else //缺FRID卡 //转坐席
                            {
                                SendTaskToServer(mServiceModel.data.msg, strResultSResult, false, false);
                            }
                        }
                    }
                    else
                    {
                        //转坐席(卡有问题)
                        SendTaskToServer(mServiceModel.data.msg, strResultSResult, false, false);
                    }
                }
                else
                {
                    SetShowInfoMsg(XTYCStr, "卡异常已转坐席,请耐心等待!", true);
                    //转坐席
                    SendTaskToServer("卡验证异常", strResultSResult, true, false);
                }
            }
            catch (Exception ex)
            {
                //SetShowInfoMsg(XTYCStr, "", true);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_卡有效性验证(getCardInfo)回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "判断卡有效性验证信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 判断业务规则
        /// </summary>
        /// <param name="carno">车号</param>
        /// <param name="icid">IC卡号</param>
        /// <param name="rfid">RFID卡号</param>
        /// <param name="caller">0终端1远程</param>
        /// <param name="planid">业务号</param>
        /// <param name="weightno">称点ID</param>
        /// <param name="carflag">业务查询标记(1:业务号不是录入的(条件中业务号可以为空);2:业务号录入的(业务号不能为空))</param>
        private void getBllRulesServiceInfo(string carno, string icid, string rfid, int type, string planid, string weightno, int carflag)
        {
            if (IsShowWeightInfo == Visibility.Hidden)
            {
                isEnterOnly = true;
                SendTaskToServer("超过量程自动发送任务", measureServiceResult, true, true);
                return;
            }
            //SetShowInfoMsg("业务处理中", "验证业务信息中...", false);
            string serviceUrl = ConfigurationManager.AppSettings["getMeasureInfo"].ToString().Replace('$', '&');
            string getUrl = string.Format(serviceUrl, carno, icid, rfid, type, planid, weightno, carflag);
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(bllRulesCallback), request);
            #region 日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体_判断业务规则_getMeasureInfo",
                Origin = "汽车衡_" + ClientInfo.Name,
                Data = getUrl,
                Level = LogConstParam.LogLevel_Info,
                Msg = "验证业务服务调用"
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        private void bllRulesCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                //初始化一下
                mServiceModel = new MeasureServiceModel();
                // mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResultSResult) as MeasureServiceModel;
                //根据IC卡号获取车号信息与根据车号获取业务信息 的返回结果一样  都是MeasureServiceModel
                //结果数据在数据.TXT中
                mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "称点主窗体_判断业务规则_getMeasureInfo回调方法",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = mServiceModel,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "验证业务服务返回值"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                ////////////////////
                ////1.判断业务逻辑(异常转坐席)
                //// 2.提交保存
                ////GetCarTask();//获取业务信息
                if (mServiceModel != null && mServiceModel.success)
                {
                    //mtype= "远程计量 以下不执行
                    if (mServiceModel.mtype != "远程计量" && mServiceModel.mtype != "强制远程计量")
                    {
                        if (mServiceModel.mfunc == 0 && mServiceModel.total == 0)//输入业务号
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                inputPlanCodeView = new ConfirmInputPlanCodeView(this.Weight.ToString("#0.00"), measurementUnitName, configUrl, isStandardBoard, CarNumber);
                                inputPlanCodeView.Owner = window;
                                inputPlanCodeView.Closed += inputPlanCodeView_Closed;
                                inputPlanCodeView.ShowDialog();
                            }));
                        }
                        //mfunc == 0 &&total == 1
                        else if (mServiceModel.mfunc == 0 && mServiceModel.total == 1)//自动业务,数据正常
                        {
                            saveMatchid = mServiceModel.rows[0].matchid;
                            #region 根据业务字段初始化界面
                            RenderUI(mServiceModel);
                            #endregion
                            this.Weight = Weight;
                            //BindCarTask(mServiceModel.rows.First());//绑定获取业务信息
                            var temp = mServiceModel.hardwarectrl.Where(p => p.name == "红外对射");
                            if (temp != null && temp.Count() > 0)
                            {
                                if (temp.First().check == "强制启用")
                                {
                                    IsCheckRedLine = true;//判断要看红外状态
                                }
                                else//强制禁用
                                {
                                    IsCheckRedLine = false;//判断不要看红外状态
                                }
                            }
                            var printTemp = mServiceModel.hardwarectrl.Where(p => p.name == "打印磅单");
                            if (printTemp != null && printTemp.Count() > 0)
                            {
                                if (printTemp.First().check == "强制启用")
                                {
                                    IsPrintPoundList = true;//判断要打磅单
                                }
                                else//强制禁用
                                {
                                    IsPrintPoundList = false;//判断不要打磅单
                                }
                            }
                            isEnterOK = false;
                            if (IsConfirmWeight)//是否确认重量
                            {
                                isAllowEnterOk = true;
                                SetShowInfoMsg(XTTSStr, "请按键盘确认键当前重量", true);
                            }
                            else
                            {
                                SetShowInfoMsg(XTTSStr, "倒计时保存重量", true);
                                isAllowEnterOk = false;
                                stdOpen();//开启倒计时,倒计时完毕调用第三个服务保存重量
                            }
                        }
                        else if (mServiceModel.mfunc == 3)//终止计量  0代表允许计量  1 代表进行提示 2 代表进行选择 3 代表终止 lt 2016-2-2 14:26:14……
                        {
                            //SetShowInfoMsg(XTTSStr, ZZStr, false);
                            //修改为提示服务返回信息……2016-2-2 14:49:39
                            string showInfo = GetShowInfos(mServiceModel.flags, ZZStr);
                            SetShowInfoMsg(XTTSStr, showInfo, false);
                        }
                        else//转坐席,手动计量
                        {
                            string showInfo = GetShowInfos(mServiceModel.flags, YCStr);
                            SetShowInfoMsg(XTTSStr, showInfo, false);
                            SendTaskToServer("手动计量任务", measureServiceResult, true, false);
                        }
                    }
                    else //执行
                    {
                        SetShowInfoMsg(XTTSStr, YCStr, false);
                        //发送给任务服务器，远程计量
                        SendTaskToServer(mServiceModel.mtype, measureServiceResult, true, false);
                    }
                }
                else
                {
                    SetShowInfoMsg(XTTSStr, YCStr, false);
                    //SendTaskToServer("查询业务信息为空或者失败手动计量任务", measureServiceResult, true, false);
                    SendTaskToServer("   ", measureServiceResult, true, false);
                }
            }
            catch (Exception ex)
            {
                SetShowInfoMsg(XTYCStr, "业务信息验证出错：请按自助求助远程", false);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_判断业务规则_getMeasureInfo回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "判断业务规则信息失败！原因：" + ex.Message + "堆栈：" + ex.StackTrace,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        public void TestOpenInputPlanCodeView()
        {
            inputPlanCodeView = new ConfirmInputPlanCodeView(this.Weight.ToString("#0.00"), measurementUnitName, configUrl, isStandardBoard, CarNumber);
            inputPlanCodeView.Owner = window;
            inputPlanCodeView.Closed += inputPlanCodeView_Closed;
            inputPlanCodeView.ShowDialog();
        }

        /// <summary>
        /// 输入业务号窗体关闭事件
        /// </summary>
        public void inputPlanCodeView_Closed(object sender, EventArgs e)
        {
            AfterCLoseInputPlanCodeView(inputPlanCodeView);
            inputPlanCodeView = null;
        }

        /// <summary>
        /// 输入业务号窗体关闭后的后续处理
        /// </summary>
        private void AfterCLoseInputPlanCodeView(ConfirmInputPlanCodeView inputPlanCodeView)
        {
            if (inputPlanCodeView != null)
            {
                if (inputPlanCodeView.IsHelp)//是否按了求助键
                {
                    isEnterHelp = true;
                    if (!string.IsNullOrEmpty(measureServiceResult) && measureServiceResult != null && !string.IsNullOrEmpty(CarNumber))
                    {
                        if (isEnterOnly)//摁过回车键  不允许再摁求助键
                        {
                            return;
                        }
                        isEnterOnly = true;
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Msg = "车号：" + CarNumber + "按下求助键远程求助",
                            FunctionName = "称点主窗体_输入业务号窗体关闭后的后续处理方法",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Level = LogConstParam.LogLevel_Info
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        SendTaskToServer("司机按键远程求助", measureServiceResult, true, true);
                    }
                }
                else if (inputPlanCodeView.IsClear)
                {
                    SendTaskToServer("【单独计皮】!!!", measureServiceResult, true, true);
                }
                else if (inputPlanCodeView.IsClose)
                {
                    inputPlanCodeView.Close();
                    inputPlanCodeView = null;
                }
                else if (inputPlanCodeView.IsOk)
                {
                    string planid = inputPlanCodeView.BussinessNo;
                    //调用判断业务规则的服务
                    BullInfo bi = new BullInfo();
                    bi.carno = CarNumber;
                    bi.taskcode = planid;
                    bi.icid = IcCardNo;
                    bi.operatype = "10";
                    bi.mflag = 0;
                    bi.caller = "0";
                    string bInfoStr = JsonConvert.SerializeObject(bi);
                    InputBllRulesServiceInfo(bInfoStr);
                }
            }
        }

        /// <summary>
        /// 判断业务规则
        /// </summary>
        /// <param name="carno">车号</param>
        /// <param name="icid">IC卡号</param>
        /// <param name="rfid">RFID卡号</param>
        /// <param name="caller">0终端1远程</param>
        /// <param name="planid">业务号</param>
        /// <param name="weightno">称点ID</param>
        /// <param name="carflag">业务查询标记(1:业务号不是录入的(条件中业务号可以为空);2:业务号录入的(业务号不能为空))</param>
        private void InputBllRulesServiceInfo(string inputStr)
        {
            if (IsShowWeightInfo == Visibility.Hidden)
            {
                isEnterOnly = true;
                SendTaskToServer("超过量程自动发送任务", measureServiceResult, true, true);
                return;
            }
            //SetShowInfoMsg("业务处理中", "验证业务信息中...", false);
            string serviceUrl = ConfigurationManager.AppSettings["inputPlanidInfo"].ToString().Replace('$', '&');

            HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, inputStr, "");
            request.BeginGetResponse(new AsyncCallback(bllRulesCallback1), request);
            #region 日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体_判断业务规则_inputPlanidInfo",
                Origin = "汽车衡_" + ClientInfo.Name,
                Data = inputStr,
                Level = LogConstParam.LogLevel_Info,
                Msg = "验证业务服务调用"
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        private void bllRulesCallback1(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                mServiceModel = new MeasureServiceModel();//初始化一下
                measureServiceResult = strResult;
                mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "称点主窗体_判断业务规则_inputPlanidInfo回调方法",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = mServiceModel,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "验证业务服务返回值"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                ////////////////////
                ////1.判断业务逻辑(异常转坐席)
                //// 2.提交保存
                ////GetCarTask();//获取业务信息
                if (mServiceModel != null && mServiceModel.success)
                {
                    if (mServiceModel.mtype != "远程计量" && mServiceModel.mtype != "强制远程计量")
                    {
                        if (mServiceModel.mfunc == 0 && mServiceModel.total == 1)//自动业务,数据正常
                        {
                            saveMatchid = mServiceModel.rows[0].matchid;
                            #region 根据业务字段初始化界面
                            RenderUI(mServiceModel);
                            #endregion
                            this.Weight = Weight;
                            var temp = mServiceModel.hardwarectrl.Where(p => p.name == "红外对射");
                            if (temp != null && temp.Count() > 0)
                            {
                                if (temp.First().check == "强制启用")
                                {
                                    IsCheckRedLine = true;//判断要看红外状态
                                }
                                else//强制禁用
                                {
                                    IsCheckRedLine = false;//判断不要看红外状态
                                }
                            }
                            var printTemp = mServiceModel.hardwarectrl.Where(p => p.name == "打印磅单");
                            if (printTemp != null && printTemp.Count() > 0)
                            {
                                if (printTemp.First().check == "强制启用")
                                {
                                    IsPrintPoundList = true;//判断要打磅单
                                }
                                else//强制禁用
                                {
                                    IsPrintPoundList = false;//判断不要打磅单
                                }
                            }
                            isEnterOK = false;
                            if (IsConfirmWeight)//是否确认重量
                            {
                                isAllowEnterOk = true;
                                SetShowInfoMsg(XTTSStr, "请按键盘确认键当前重量", true);
                            }
                            else
                            {
                                SetShowInfoMsg(XTTSStr, "倒计时保存重量", true);
                                isAllowEnterOk = false;
                                stdOpen();//开启倒计时,倒计时完毕调用第三个服务保存重量
                            }
                        }
                        else if (mServiceModel.mfunc == 3)//终止计量  0代表允许计量  1 代表进行提示 2 代表进行选择 3 代表终止 lt 2016-2-2 14:26:14……
                        {
                            this.Weight = Weight;
                            //SetShowInfoMsg(XTTSStr, ZZStr, false);
                            //修改为提示服务返回信息……2016-2-2 14:49:39
                            string showInfo = GetShowInfos(mServiceModel.flags, ZZStr);
                            SetShowInfoMsg(XTTSStr, showInfo, false);
                        }
                        else//转坐席,手动计量
                        {
                            //SetShowInfoMsg(XTTSStr, YCStr, false);
                            this.Weight = Weight;
                            string showInfo = GetShowInfos(mServiceModel.flags, YCStr);
                            SetShowInfoMsg(XTTSStr, showInfo, false);
                            SendTaskToServer("手动计量任务", measureServiceResult, true, false);
                        }
                    }
                    else
                    {
                        this.Weight = Weight;
                        SetShowInfoMsg(XTTSStr, YCStr, false);
                        SendTaskToServer("皮重超期", strResult, false, false);
                    }
                }
                else
                {
                    this.Weight = Weight;
                    SetShowInfoMsg(XTTSStr, YCStr, false);
                    SendTaskToServer("       ", measureServiceResult, true, false);
                }
            }
            catch (Exception ex)
            {
                this.Weight = Weight;
                SetShowInfoMsg(XTYCStr, "业务信息验证出错：请按自助求助远程", false);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_判断业务规则_inputPlanidInfo回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "判断业务规则信息失败！原因：" + ex.Message + "堆栈：" + ex.StackTrace,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 保存重量.  保存成功后，保存图片，保存打印，更换界面颜色
        /// 当前不执行
        /// </summary>
        public void saveWeightServiceInfo()
        {
            BullInfo bInfo = mServiceModel.rows[0];
            isEnterOK = true;
            //记录对应的操作人员信息 lt 2016-2-2 14:39:21……
            string opId = "SYSTEM";
            string opName = "系统自动";
            if (IsConfirmWeight)
            {
                opName = "司机自助";
            }

            //var getNowDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//不再取当前时间，启用服务的sysdate 2016-3-30 16:53:25……
            if (bInfo.suttle >= 0 && bInfo.measurestate == "G")
            {
                bInfo.grossweigh = ClientInfo.Name;
                bInfo.grossweighid = ClientInfo.ClientId;
                bInfo.grossoperacode = opId;
                bInfo.grossoperaname = opName;
                bInfo.suttleweigh = ClientInfo.Name;
                bInfo.suttleweighid = ClientInfo.ClientId;
                bInfo.suttleoperacode = opId;
                bInfo.suttleoperaname = opName;
            }
            else if (bInfo.suttle >= 0 && bInfo.measurestate == "T")
            {
                bInfo.tareweigh = ClientInfo.Name;
                bInfo.tareweighid = ClientInfo.ClientId;
                bInfo.tareoperacode = opId;
                bInfo.tareoperaname = opName;
                bInfo.suttleweigh = ClientInfo.Name;
                bInfo.suttleweighid = ClientInfo.ClientId;
                bInfo.suttleoperacode = opId;
                bInfo.suttleoperaname = opName;
            }
            else if (bInfo.gross >= 0 && bInfo.measurestate == "G")
            {
                bInfo.grossweigh = ClientInfo.Name;
                bInfo.grossweighid = ClientInfo.ClientId;
                bInfo.grossoperacode = opId;
                bInfo.grossoperaname = opName;

            }
            else if (bInfo.tare >= 0 && bInfo.measurestate == "T")
            {
                bInfo.tareweigh = ClientInfo.Name;
                bInfo.tareweighid = ClientInfo.ClientId;
                bInfo.tareoperacode = opId;
                bInfo.tareoperaname = opName;
            }
            else
            {
                CheckIsSave();
                return;
            }
            string rtError = string.Empty;
            if (Convert.ToDecimal(this.Weight) <= 0)//当前重量变为0或者负数时 提示……2016-2-29 14:36:32
            {
                rtError = "当前秤体重量为：" + this.Weight;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = rtError + "是否保存出问题，要求司机远程求助",
                    FunctionName = "称点主窗体_保存重量_当前重量变为0或者负数",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                isEnterOK = false;
                isEnterOnly = false;
                //SetShowInfoMsg("系统异常", rtError + "请远程求助或者拨打大厅电话", false);
                return;
            }
            if (bInfo.suttle == null && bInfo.gross > 0 && bInfo.tare > 0)//净重小于等于0的时候进行提示 ……2016-2-29 14:31:51
            {
                rtError = "净重为：" + (bInfo.gross - bInfo.tare - bInfo.deduction);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = rtError + "是否保存出问题，要求司机远程求助",
                    FunctionName = "称点主窗体_保存重量_净重小于等于0",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                isEnterOK = false;
                isEnterOnly = false;
                //SetShowInfoMsg("系统异常", rtError + "请远程求助或者拨打大厅电话", false);
                return;

            }
            else
            {
                if (bInfo.suttle <= 200)//净重小于等于200的时候进行提示 ……2016-2-29 14:31:51
                {
                    rtError = "净重为：" + bInfo.suttle;
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = rtError + "是否保存出问题，要求司机远程求助",
                        FunctionName = "称点主窗体_保存重量_净重小于等于200",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Info
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    isEnterOK = false;
                    isEnterOnly = false;
                    //SetShowInfoMsg("系统异常", rtError + "请远程求助或者拨打大厅电话", false);
                    return;
                }
            }
            string bInfoStr = JsonConvert.SerializeObject(bInfo);
            bool isAllowSave = CheckIsAllowSave(bInfoStr);
            if (!isAllowSave)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "调用CheckIsAllowSave，判断出不允许保存，系统自动发送远程求助",
                    FunctionName = "称点主窗体_保存重量_判断是否允许保存",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                SendTaskToServer("自动远程求助", measureServiceResult, true, true);
                return;
            }

            //自助计量完成之后 重量不再变化……2016-3-28 11:10:14……
            receiveBullInfo = bInfo;
            saveMatchid = receiveBullInfo.matchid;
            if ((this.IsRedLineLeft || this.IsRedLineRight))
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "红外对射遮挡，系统自动发送远程计量!",
                    FunctionName = "称点主窗体_保存重量_左/右红外被挡",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                string mServiceModelstr = JsonConvert.SerializeObject(mServiceModel);
                SendTaskToServer("红外对射遮挡!!!", mServiceModelstr, false, true);
                return;
            }
            //SetShowInfoMsg("业务处理中", "重量信息保存中...", false);
            string serviceUrl = ConfigurationManager.AppSettings["saveMeasureInfo"].ToString();
            #region 日志
            LogModel sLog = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体_保存重量_saveMeasureInfo",
                Origin = "汽车衡_" + ClientInfo.Name,
                Data = bInfo,
                Level = LogConstParam.LogLevel_Info,
                Msg = "调用保存重量服务",
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "URL", ParamValue = serviceUrl } }
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(sLog));
            #endregion
            HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, bInfoStr, "");
            request.BeginGetResponse(new AsyncCallback(saveWeightCallback), request);
        }
        private void saveWeightCallback(IAsyncResult asyc)
        {
            try
            {
                isAllowEnterOk = false;
                isEnterOnly = false;
                string strResult = ComHelpClass.ResponseStr(asyc);
                var getServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "称点主窗体_保存重量_saveMeasureInfo回调方法",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = getServiceModel,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "保存重量服务返回值"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                ////////////////////              
                //// 1.提交保存
                if (getServiceModel != null && getServiceModel.success)
                {
                    #region 写日志
                    string msg = "车号：" + CarNumber + "  过磅单号：" + receiveBullInfo.matchid + "  毛重：" + receiveBullInfo.gross + "  皮重："
                    + receiveBullInfo.tare + "  净重：" + receiveBullInfo.suttle + "  保存成功";
                    log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = msg,
                        FunctionName = "称点主窗体_保存重量_saveMeasureInfo回调方法",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Info
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    #region 保存图片
                    //SaveVideoPic(mServiceModel.rows[0].matchid, mServiceModel.rows[0].measurestate);
                    Thread thread = new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            if (mServiceModel == null || mServiceModel.rows == null)
                            {
                                SaveVideoPic("YC1", "YC1");
                            }
                            else
                            {
                                SaveVideoPic(mServiceModel.rows[0].matchid, mServiceModel.rows[0].measurestate);
                            }
                            #region 日志
                            LogModel log1 = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "称点主窗体_保存重量_saveMeasureInfo回调方法_保存抓拍的图片",
                                Origin = "汽车衡_" + ClientInfo.Name,
                                Data = mServiceModel,
                                Level = LogConstParam.LogLevel_Info,
                                Msg = "保存抓拍的图片"
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            #region 日志
                            LogModel log1 = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Direction = LogConstParam.Directions_In,
                                FunctionName = "称点主窗体_保存重量_saveMeasureInfo回调方法_保存抓拍的图片",
                                Origin = "汽车衡_" + ClientInfo.Name,
                                Data = mServiceModel,
                                Level = LogConstParam.LogLevel_Error,
                                Msg = "抓拍图片异常，原因：" + ex.Message
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                            #endregion
                        }



                    }));
                    thread.IsBackground = true;
                    thread.Start();

                    #endregion
                    #region 保存打印
                    if (IsPrintPoundList)
                    {
                        SetShowInfoMsg("业务处理中", "打印票据中...", true);
                        printModel = new PrintInfo
                        {
                            matchid = mServiceModel.rows[0].matchid,
                            carno = mServiceModel.rows[0].carno,
                            clientcode = ClientInfo.ClientCode,
                            clientname = ClientInfo.Name,
                            opcode = ClientInfo.ClientCode,
                            opname = ClientInfo.ClientCode,
                            printtype = "正常"
                        };
                        #region 日志
                        LogModel log2 = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_In,
                            FunctionName = "称点主窗体_保存重量_saveMeasureInfo回调方法_准备获取打印票据信息",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Data = printModel,
                            Level = LogConstParam.LogLevel_Info,
                            Msg = "准备获取打印票据的信息"
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                        #endregion
                        Thread t1 = new Thread(new ThreadStart(GetPrintModelData));
                        t1.IsBackground = true;
                        t1.Start();
                        //GetPrintModelData();
                    }
                    #endregion
                    #region 变换界面颜色
                    TopColor = "#FF0C215F";
                    TopColor1 = "#FF09236E";
                    TopColor2 = "#FF072C93";
                    LeftColor1 = "#FF060628";
                    LeftColor2 = "#FF5FEE4A";
                    RightColor1 = "#FF060628";
                    RightColor2 = "#FF5FEE4A";//ARGB，其中当A为FF时表示不透明，相反A为00时对象会完全透明，变得不可见
                    #endregion
                    SetShowInfoMsg(XTTSStr, JLWCStr, false);
                    BullState = eBullTag.end;
                    iwc.IsFinish = true;
                    saveWeight = true;
                }
                else
                {
                    SetShowInfoMsg(XTYCStr, "保存方法验证成功，返回失败：请按自助求助远程", false);
                }
            }
            catch (Exception ex)
            {
                SetShowInfoMsg(XTYCStr, "保存方法验证出现错误：请按自助求助远程", false);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_保存重量_saveMeasureInfo回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "保存重量失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }

        }
        public void tipMsg()
        {
            isEnterCancel = true;
            SetShowInfoMsg(XTTSStr, "计量暂停,请按键盘确认键继续计量或按求助键转远程坐席", true);
        }
        /// <summary>
        /// 保存称点重量记录服务
        /// </summary>
        private void saveWeightRecordServiceInfo()
        {
            string wRealDataStr = "";
            string serviceUrl = ConfigurationManager.AppSettings["saveWeightRecordInfo"].ToString();
            try
            {
                wRealData.realdata = JsonConvert.SerializeObject(recorddatalist);
                wRealData.clientid = ClientInfo.ClientId;
                wRealData.matchid = saveMatchid;
                wRealData.carno = saveCarNo;
                wRealDataStr = JsonConvert.SerializeObject(wRealData);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_保存称点重量记录",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = "过磅单号：" + saveMatchid + "  车号：" + saveCarNo,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "保存称点重量记录服务调用",
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "URL", ParamValue = serviceUrl } }
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
                    FunctionName = "称点主窗体_保存称点重量记录",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = wRealDataStr,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "保存称点重量记录服务调用异常" + ex.Message + ex.StackTrace,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "URL", ParamValue = serviceUrl } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            try
            {
                HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, wRealDataStr, "");
                request.BeginGetResponse(new AsyncCallback(saveWeightRecordCallback), request);
                //保存之后 将数据列表清空…… 2016-3-7 10:48:52……
                recorddatalist.Clear();
                saveMatchid = string.Empty;
                saveCarNo = string.Empty;
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_保存称点重量记录",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = wRealDataStr,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用重量保存服务调用异常" + ex.Message + ex.StackTrace,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "URL", ParamValue = serviceUrl } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            saveWeight = false;//再次保存……lt 2016-2-17 08:29:13……
        }
        private void saveWeightRecordCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                var getServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                recorddatalist.Clear();
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "称点主窗体_保存称点重量记录回调方法",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = getServiceModel,
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "保存称点重量记录服务调用返回值"
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
                    FunctionName = "称点主窗体_保存称点重量记录回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "称点重量记录服务失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + ClientInfo.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }

        }
        #endregion
        #region 根据业务字段动态构建UI
        private bool RenderUI(MeasureServiceModel mSM)
        {
            try
            {
                if (mSM.mores != null && mSM.mores.Count > 0)
                {
                    RenderMainUI ui = new RenderMainUI(0, gridReader, gridSupplier, gridMeasure, gridMeasureWeight, mSM.rows.First(), mSM.mores);
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => { ui.SetRenderMainUI(); }));
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "初始化界面信息出错：" + ex.StackTrace,
                    FunctionName = "称点主窗体_后台生成窗体控件",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Data = mSM,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return true;
        }
        #endregion
        #endregion
        #region 硬件控制命令
        /// <summary>
        /// 打开红灯关闭绿灯
        /// </summary>
        /// <returns></returns>
        private bool OpenRedLed()
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                Origin = "汽车衡_" + ClientInfo.Name,
                FunctionName = "称点主窗体_打开红灯关闭绿灯",
                Level = LogConstParam.LogLevel_Info,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Msg = "打开红灯关闭绿灯"
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            IsRedLedOpend = true;
            SendRealDataToServer("LR");
            //return true;
            try
            {
                if (ioc == null)
                {
                    return false;
                }
                List<IoCommand> ioclist = new List<IoCommand>();
                ioclist.Add(new IoCommand("2", DeviceConst.RedLight, "1") { });
                ioclist.Add(new IoCommand("2", DeviceConst.GreenLight, "0") { });
                ////io设备不全下注释////
                //bool rtSend = ioc.TestExecCommand("01");
                bool rtSend = ioc.ExecCommand(ioclist);
                if (rtSend)//IO不能初始化会发生异常
                {
                    leftLed = "Image/red.png";
                    rightLed = "Image/red.png";
                    IsRedLedOpend = true;
                    SendRealDataToServer("LR");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                log = new LogModel()
                {
                    Origin = "汽车衡_" + ClientInfo.Name,
                    FunctionName = "称点主窗体_打开红灯关闭绿灯",
                    Level = LogConstParam.LogLevel_Error,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "打开红灯关闭绿灯过程中异常:" + ex.Message
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return true;
        }
        /// <summary>
        /// 打开绿灯关闭红灯
        /// </summary>
        /// <returns></returns>
        private bool OpenGreenLed()
        {
            #region 写日志
            LogModel log = new LogModel()
            {
                Origin = "汽车衡_" + ClientInfo.Name,
                FunctionName = "称点主窗体_打开绿灯关闭红灯",
                Level = LogConstParam.LogLevel_Info,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Msg = "打开绿灯关闭红灯"
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion

            IsRedLedOpend = false;
            SendRealDataToServer("LR");
            //return true;
            try
            {
                if (ioc == null)
                {
                    return false;
                }
                List<IoCommand> ioclist = new List<IoCommand>();
                ioclist.Add(new IoCommand("2", DeviceConst.RedLight, "0") { });
                ioclist.Add(new IoCommand("2", DeviceConst.GreenLight, "1") { });
                ////io设备不全下注释////
                //bool rtSend = ioc.TestExecCommand("01");
                bool rtSend = ioc.ExecCommand(ioclist);
                if (rtSend)//IO不能初始化会发生异常
                {
                    IsRedLedOpend = false;
                    leftLed = "Image/green.png";
                    rightLed = "Image/green.png";
                    SendRealDataToServer("LR");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                log = new LogModel()
                {
                    Origin = "汽车衡_" + ClientInfo.Name,
                    FunctionName = "称点主窗体_打开绿灯关闭红灯",
                    Level = LogConstParam.LogLevel_Error,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "打开绿灯失败：" + ex.Message
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return true;
        }
        /// <summary>
        /// 读声音
        /// </summary>
        /// <param name="msg"></param>
        private void PlayVoice(string msg)
        {
            if (false == isCanReadText) return;
            if (!string.IsNullOrEmpty(msg))
            {
                SoundReading sr = new SoundReading(msg);
                sr.Voice();
            }
        }
        /// <summary>
        /// 打印票据(正常/补打)
        /// "matchid":"8018080100255","opname":"樊军","opcode":"14610","clientcode":"104","clientname":"原料80t","carno":"冀EF0579","printtype":"正常","TicketType":0},"msgid":643
        /// </summary>
        /// <param name="measureJsonStr">称量记录json字符串</param>
        private void SupplementTicket(string measureJsonStr)
        {
            printError = string.Empty;
            if (!string.IsNullOrEmpty(measureJsonStr))
            {
                if (CarNumber == null)
                {
                    CarNumber = string.Empty;
                }
                string printCarNo = string.Empty;
                string printState = string.Empty;
                try
                {
                    printModel = InfoExchange.DeConvert(typeof(PrintInfo), measureJsonStr) as PrintInfo;
                    //车号和打印类型（正常和补打）
                    printCarNo = printModel.carno;
                    printState = printModel.printtype;
                    #region 存在读取到的车号  校验读取到的车号和接收到的要打印的车号是否一致，不一致则回传给坐席,当前称体车号为空，不允许正常打印票据（只能是补打）
                    if (!string.IsNullOrEmpty(CarNumber))//存在读取到的车号  校验读取到的车号和接收到的要打印的车号是否一致，不一致则回传给坐席
                    {
                        if (!printCarNo.Equals(CarNumber))//存在读取车号与打印车号 不一致  返回
                        {
                            #region 写日志
                            LogModel log = new LogModel()
                            {
                                Origin = "汽车衡_" + ClientInfo.Name,
                                FunctionName = "称点主窗体_打印票据_读取的现场车号与打印车号不一致",
                                Level = LogConstParam.LogLevel_Info,
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Msg = " 车号：" + printCarNo + " 过磅单号：" + printModel.matchid + " 准备打印票据。打印类型：" + printState + " 系统提示：" + "打印票据车号" + printCarNo + "当前秤上车号为：" + CarNumber
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            #endregion
                            SendPrintState(ErrorType.Error, "打印票据车号" + printCarNo + "当前秤上车号为：" + CarNumber);
                            return;
                        }
                    }
                    else //当前称体车号为空，不允许正常打印票据（只能是补打）
                    {
                        if (printState.Equals("正常"))
                        {
                            #region 写日志
                            LogModel log = new LogModel()
                            {
                                Origin = "汽车衡_" + ClientInfo.Name,
                                FunctionName = "称点主窗体_打印票据_读取的现场车号不存在",
                                Level = LogConstParam.LogLevel_Info,
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Msg = " 车号：" + printCarNo + " 过磅单号：" + printModel.matchid + " 准备打印票据。打印类型：" + printState + " 系统提示：" + "当前秤体车号为空,不允许正常打印票据"
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            #endregion
                            SendPrintState(ErrorType.Error, "当前秤体车号为空,不允许正常打印票据");
                            return;
                        }
                    }
                    #endregion

                    #region 写日志
                    LogModel rLog = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = " 车号：" + printCarNo + " 过磅单号：" + printModel.matchid + " 准备打印票据。打印类型：" + printState,
                        FunctionName = "称点主窗体_打印票据(" + printState + ")",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Data = printModel,
                        Level = LogConstParam.LogLevel_Info
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(rLog));
                    #endregion
                    //打印方法
                    GetPrintModelData();
                    //向服务回写出票信息
                    //SavePrintBillToServer(printInfo);

                    SendPrintState(ErrorType.Error, printError);

                }
                catch (Exception ex)
                {
                    string msg = "打印票据异常,原因:" + ex.Message;
                    printError = msg;
                    SetShowInfoMsg("系统提示", msg, false);
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        Origin = "汽车衡_" + ClientInfo.Name,
                        FunctionName = "称点主窗体_打印票据",
                        Level = LogConstParam.LogLevel_Error,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_In,
                        Msg = msg,
                        Data = printModel,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        OperateUserName = printModel.opname
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }
        /// <summary>
        /// 获取打印信息
        /// </summary>
        private void GetPrintModelData()
        {
            try
            {
                #region get 方法
                string serviceUrl = ConfigurationManager.AppSettings["getPrintModelData"].ToString();
                var param = new
                {
                    matchid = printModel.matchid,
                    opname = printModel.opname,
                    opcode = printModel.opcode,
                    equcode = printModel.clientcode,
                    equname = printModel.clientname,
                    carno = printModel.carno,
                    printtype = printModel.printtype
                    //,TicketType = printModel.TicketType//票据类型(0:净重票据;1:毛重票据)
                };

                string url = string.Format(serviceUrl, System.Web.HttpUtility.UrlEncode("[" + JsonConvert.SerializeObject(param) + "]", System.Text.Encoding.UTF8));
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "获取打印票据的信息,URL:" + url,
                    FunctionName = "称点主窗体_获取打印票据的信息_getPrintModelData",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                string rtInfo = WebHttpHelpClass.HttpGet(url, string.Empty);
                GetPrintModelDataCallback(rtInfo);
                #endregion
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_获取打印票据的信息_getPrintModelData",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "调用服务，获取打印数据时出现错误。" + ex.Message,
                    Origin = "汽车衡_" + printModel.clientname,
                    OperateUserName = printModel.opname
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //throw ex;
            }

        }

        /// <summary>
        /// 获取打印信息回调方法
        /// </summary>
        /// <param name="ar"></param>
        //private void GetPrintModelDataCallback(IAsyncResult asyc)
        //{
        //    try
        //    {
        //        string printModelJsonStr = ComHelpClass.ResponseStr(asyc);
        //        #region 日志
        //        LogModel log = new LogModel()
        //        {
        //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //            Direction = LogConstParam.Directions_OutIn,
        //            FunctionName = "称点主窗体",
        //            Level = LogConstParam.LogLevel_Info,
        //            Msg = "通过服务读取打印信息成功!",
        //            Origin = "汽车衡_" +printModel.clientname,
        //            OperateUserName = printModel.opname,
        //            Data = "测试获取数据长度~" + printModelJsonStr.Length + "~" + printModelJsonStr,
        //            IsDataValid = LogConstParam.DataValid_Ok
        //        };
        //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
        //        #endregion
        //        getPrintDataCount = 0;
        //        if (!string.IsNullOrEmpty(printModelJsonStr))
        //        {
        //            //if (printModelJsonStr.Length>4)
        //            //{
        //            //    printModelJsonStr = printModelJsonStr.Substring(2);
        //            //    printModelJsonStr = printModelJsonStr.Substring(0, printModelJsonStr.Length - 2);
        //            //    printModelJsonStr.Replace("\r\n", " ");
        //            //    printModelJsonStr.Replace("GBK", "utf-8");
        //            //}
        //            List<string> prinList = new List<string>() { printModelJsonStr };
        //            PrinterController pc = new PrinterController(this.configUrl);
        //            pc.OnShowErrMsg += pc_OnShowErrMsg;
        //            if (Weight == 0)
        //            {
        //                Weight = 20;
        //            }
        //            SendRealDataToServer("W");
        //            pc.Print(prinList);
        //            logH.SaveLog(" 车号：" + printModel.carno + " 过磅单号：" + printModel.matchid + " 打印完成");
        //        }
        //        else
        //        {
        //            SendPrintState(ErrorType.Error, "获取打印数据失败");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = "通过服务读取打印信息失败！原因：" + ex.Message;
        //        printError = msg;
        //        SetShowInfoMsg("系统提示", msg, false);
        //        #region 日志
        //        LogModel log = new LogModel()
        //        {
        //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //            Direction = LogConstParam.Directions_OutIn,
        //            FunctionName = "称点主窗体",
        //            Level = LogConstParam.LogLevel_Error,
        //            Msg = msg,
        //            Origin = "汽车衡_" +printModel.clientname,
        //            OperateUserName = printModel.opname
        //        };
        //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
        //        #endregion
        //        if (getPrintDataCount < 3)
        //        {
        //            getPrintDataCount = getPrintDataCount + 1;
        //            FileHelpClass.WriteLog("获取打印数据出现异常" + ex.Message + "，系统再次调用服务");
        //            GetPrintModelData();
        //        }
        //    }
        //}
        /// <summary>
        /// 获取打印信息回调方法
        /// </summary>
        /// <param name="ar"></param>
        private void GetPrintModelDataCallback(string infos)
        {
            try
            {
                string printModelJsonStr = infos;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "称点主窗体_获取打印信息的回调方法",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "通过服务读取打印信息成功!",
                    Origin = "汽车衡_" + printModel.clientname,
                    OperateUserName = printModel.opname,
                    Data = "数据长度:【" + printModelJsonStr.Length + "】;信息内容:" + printModelJsonStr,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                List<string> prinList = new List<string>() { printModelJsonStr };
                PrinterController pc = new PrinterController(this.configUrl);
                pc.OnShowErrMsg += pc_OnShowErrMsg;
                pc.Print(prinList);
                #region 写日志
                LogModel sLog = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = " 车号：" + printModel.carno + " 过磅单号：" + printModel.matchid + " 打印完成",
                    FunctionName = "称点主窗体_获取打印信息的回调方法",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(sLog));
                #endregion
            }
            catch (Exception ex)
            {
                System.GC.Collect();
                string msg = "通过服务读取打印信息失败！原因：" + ex.Message;
                SetShowInfoMsg("系统提示", msg, false);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "称点主窗体_获取打印信息的回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = msg,
                    Origin = "汽车衡_" + printModel.clientname,
                    OperateUserName = printModel.opname
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //throw ex;
            }
        }
        /// <summary>
        /// 显示打印机错误信息
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void pc_OnShowErrMsg(ErrorType arg1, string arg2)
        {
            printError = arg2;
            //SendPrintState(arg1, arg2);
        }
        /// <summary>
        /// 抓拍照片
        /// </summary>
        /// <param name="pLogisticsId">物流ID</param>
        /// <param name="pBusinessType">业务类型</param>
        private void SaveVideoPic(string pLogisticsId, string pBusinessType)
        {
            string fileName = pLogisticsId + "_" + pBusinessType + "_SCREEN" + DateTime.Now.ToString("HHmmss") + ".jpg";
            #region 日志
            LogModel log4 = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体_抓拍照片",
                Level = LogConstParam.LogLevel_Info,
                Msg = "截屏开始。",
                Origin = "汽车衡_" + ClientInfo.Name,
                ParamList = new List<DataParam>() 
                { 
                    new DataParam() { ParamName = "pLogisticsId", ParamValue = pLogisticsId }, 
                    new DataParam() { ParamName = "pBusinessType", ParamValue = pBusinessType } 
                }
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log4));
            #endregion
            FileHelpClass.ScreenShot(picPath, fileName, ImageFormat.Jpeg);
            #region 日志
            LogModel log3 = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "称点主窗体_抓拍照片",
                Level = LogConstParam.LogLevel_Info,
                Msg = "截屏结束。",
                Origin = "汽车衡_" + ClientInfo.Name,
                ParamList = new List<DataParam>() 
                { 
                    new DataParam() { ParamName = "pLogisticsId", ParamValue = pLogisticsId }, 
                    new DataParam() { ParamName = "pBusinessType", ParamValue = pBusinessType } 
                }
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log3));
            #endregion
            try
            {
                VideoController tVideoController = new VideoController(this.configUrl);
                tVideoController.Open();
                tVideoController.Start();
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_抓拍照片",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "准备调用摄像头抓拍。",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    ParamList = new List<DataParam>() 
                    { 
                        new DataParam() { ParamName = "pLogisticsId", ParamValue = pLogisticsId }, 
                        new DataParam() { ParamName = "pBusinessType", ParamValue = pBusinessType } 
                    }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                tVideoController.CapturePicture(pLogisticsId, pBusinessType);
                #region 日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_抓拍照片",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用摄像头抓拍结束。",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    ParamList = new List<DataParam>() 
                    { 
                        new DataParam() { ParamName = "pLogisticsId", ParamValue = pLogisticsId }, 
                        new DataParam() { ParamName = "pBusinessType", ParamValue = pBusinessType } 
                    }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
                tVideoController.Stop();
                tVideoController.Close();

            }
            catch (Exception ex)
            {
                #region log
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_抓拍照片",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "调用摄像头，抓拍异常。" + ex.Message + "\r\t" + ex.StackTrace,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "ConfigFile", ParamValue = configUrl } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 同步DVR系统时间
        /// </summary>
        private void SyncDvrTime()
        {
            try
            {
                SynOracleTime();
                VideoController tVideoController = new VideoController(this.configUrl);
                tVideoController.Open();
                tVideoController.Start();

                #region log
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_同步DVR系统时间",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "同步DVR系统时间。",
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "ConfigFile", ParamValue = configUrl } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion

                bool rtn = tVideoController.SyncDateTime();
                tVideoController.Stop();
                tVideoController.Close();

            }
            catch (Exception ex)
            {
                #region log
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "称点主窗体_同步DVR系统时间",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "同步DVR系统时间异常:" + ex.Message + ";堆栈:" + ex.StackTrace,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "ConfigFile", ParamValue = configUrl } }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 表头清零IO方式 闭合开关
        /// </summary>
        /// <returns></returns>
        private bool ClearWeightOpenIo()
        {
            try
            {
                if (ioc == null)
                {
                    return false;
                }
                List<IoCommand> ioclist = new List<IoCommand>();
                ioclist.Add(new IoCommand("1", DeviceConst.IoSwitche, "0") { });
                return ioc.ExecCommand(ioclist);

            }
            catch //(Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 表头清零IO方式 打开开关
        /// </summary>
        /// <returns></returns>
        private bool ClearWeightCloseIo()
        {
            try
            {
                if (ioc == null)
                {
                    return false;
                }
                List<IoCommand> ioclist = new List<IoCommand>();
                ioclist.Add(new IoCommand("1", DeviceConst.IoSwitche, "1") { });
                return ioc.ExecCommand(ioclist);
            }
            catch //(Exception ex)
            {
                return false;
            }
        }
        #endregion
        /// <summary>
        /// 测试使用
        /// </summary>
        private string _TestCardNo;
        public string TestCardNo
        {
            get { return _TestCardNo; }
            set
            {
                _TestCardNo = value;
                this.RaisePropertyChanged("TestCardNo");
            }
        }
        #region 私有处理方法

        /// <summary>
        /// 关闭主窗体外的其他窗体
        /// </summary>
        private void CloseOtherForm()
        {
            for (int i = 0; i < Application.Current.Windows.Count; i++)
            {
                string windowName = Application.Current.Windows[i].Name;
                if (windowName != "mainWindow")
                {
                    Application.Current.Windows[i].Close();
                }
            }
        }

        /// <summary>
        /// 获取对应的显示信息 2016-2-2 15:05:17……
        /// </summary>
        /// <returns></returns>
        private string GetShowInfos(List<flagMsg> ListMsg, string configStr)
        {
            string rtInfos = string.Empty;
            for (int i = 0; i < ListMsg.Count; i++)
            {
                flagMsg flagM = ListMsg[i];
                if (flagM.flag != 0)
                {
                    rtInfos = flagM.Msg + ", " + rtInfos;
                }
            }
            rtInfos = rtInfos + " " + configStr;
            return rtInfos;
        }
        /// <summary>
        /// 处理界面显示小数点的问题
        /// </summary>
        private void DoDecimalShowInfos()
        {
            this.CarTaskModel.BullInfo.grossb = DoDecimal(this.CarTaskModel.BullInfo.grossb);
            this.CarTaskModel.BullInfo.tareb = DoDecimal(this.CarTaskModel.BullInfo.tareb);
            this.CarTaskModel.BullInfo.suttleb = DoDecimal(this.CarTaskModel.BullInfo.suttleb);
            this.CarTaskModel.BullInfo.gross = DoDecimal(this.CarTaskModel.BullInfo.gross);
            this.CarTaskModel.BullInfo.tare = DoDecimal(this.CarTaskModel.BullInfo.tare);
            this.CarTaskModel.BullInfo.suttle = DoDecimal(this.CarTaskModel.BullInfo.suttle);
            this.CarTaskModel.BullInfo.deduction = DoDecimal(this.CarTaskModel.BullInfo.deduction);
        }
        private decimal DoDecimal(decimal? inInfos)
        {
            decimal rtInfo = 0;
            try
            {
                rtInfo = Convert.ToDecimal((Convert.ToDecimal(inInfos.ToString()).ToString("F0")));
            }
            catch //(Exception ex)
            {
            }
            return rtInfo;
        }
        /// <summary>
        /// 处理页面颜色初始化
        /// </summary>
        private void DoGridInitColors()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        //计毛边框颜色恢复初始化颜色
                        var getTxtGrossWeight = gridMeasureWeight.FindName("txtGrossWeight") as TextBox;
                        getTxtGrossWeight.SetValue(TextBox.BorderBrushProperty, new SolidColorBrush(Color.FromArgb(255, 56, 69, 86)));

                        //计皮边框颜色恢复初始化颜色
                        var getTxtTaireWeight = gridMeasureWeight.FindName("txtTaireWeight") as TextBox;
                        getTxtTaireWeight.SetValue(TextBox.BorderBrushProperty, new SolidColorBrush(Color.FromArgb(255, 56, 69, 86)));
                    }
                    catch (Exception ex)
                    {
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Msg = "计毛/计皮边框颜色恢复初始化异常:" + ex.Message,
                            FunctionName = "称点主窗体_页面颜色初始化_计毛/计皮边框颜色恢复初始化",
                            Origin = "汽车衡_" + ClientInfo.Name,
                            Level = LogConstParam.LogLevel_Error
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                });
                //背景颜色变为初始化颜色…… lt 2016-2-17 08:47:03……
                TopColor = "#FF0C215F";
                TopColor1 = "#FF09236E";
                TopColor2 = "#FF072C93";
                LeftColor1 = "#FF060628";
                LeftColor2 = "#FF0B3D4A";
                RightColor1 = "#FF060628";
                RightColor2 = "#FF0B3D4A";
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "DoGridInitColors界面颜色初始化错误：" + ex.Message,
                    FunctionName = "称点主窗体_页面颜色初始化",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 清除动态生成的业务信息
        /// </summary>
        private void ClearGridReaderInfos()
        {
            try
            {
                //那么到底什么是Dispatcher呢？从字面上来说，它是所谓的接线员，或者调度员的意思。
                //这说明什么呢？每个线程都有一个唯一的调度员，我们在代码中所做的工作其实是向这个调度员发出指令，然后它再帮我们做
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    int childCount = gridReader.Children.Count;
                    for (int i = 0; i < childCount; i++)
                    {
                        try
                        {
                            Border bd = (Border)gridReader.Children[i];
                            Grid itemGD = (Grid)bd.Child;
                            for (int j = 0; j < itemGD.Children.Count; j++)
                            {
                                TextBlock tx = (TextBlock)itemGD.Children[j];
                                if (tx.Name.Contains("tx_"))
                                {
                                    tx.Text = string.Empty;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            #region 写日志
                            LogModel log = new LogModel()
                            {
                                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Msg = "清除动态生成的业务信息异常:" + ex.Message,
                                FunctionName = "称点主窗体_清除动态生成的业务信息",
                                Origin = "汽车衡_" + ClientInfo.Name,
                                Level = LogConstParam.LogLevel_Error
                            };
                            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                            #endregion
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "ClearGridReaderInfos清除业务信息出错:" + ex.Message,
                    FunctionName = "称点主窗体_清除动态生成的业务信息",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 系统暂停计量（关闭IC卡、RFID卡）
        /// </summary>
        private void ClientStop(CommandParam cp)
        {
            string msg = "接收到服务器命令【sendCMD】,业务类型【ClientStop】,称点暂停计量!";
            try
            {

                ///关闭视频抓拍
                //if (tVideoController != null)
                //{
                //    tVideoController.Close();
                //}
                CloseIc();
                CloseRFID();
                //if (iic != null)
                //{
                //    //iic.OnReadCardNo -= iic_OnReadCardNo;
                //    //iic.OnRemoveCard -= iic_OnRemoveCard;
                //    //iic.Stop();
                //    //iic.Close();
                //}
            }
            catch (Exception ex)
            {
                msg = "停止IC卡时出错：" + ex.Message;
            }
            SetShowInfoMsg("系统提示", "系统已停用", false);
            #region 写日志
            LogModel log = new LogModel()
            {
                Origin = "汽车衡_" + ClientInfo.Name,
                FunctionName = "称点主窗体_系统暂停计量",
                Level = LogConstParam.LogLevel_Info,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                Msg = msg,
                Data = cp,
                IsDataValid = LogConstParam.DataValid_Ok,
                OperateUserName = IcCardNo
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        /// <summary>
        /// 显示坐席接收到的消息
        /// </summary>
        /// <param name="cp"></param>
        private void GetUserNotice(CommandParam cp)
        {
            isClearInfos = false;
            string msg = "接收到服务器命令【sendCMD】,业务类型【UserNotice】,坐席发送通知!";
            try
            {
                string infos = cp.msg.msg.ToString();
                if (infos.Equals("ClearNoticeInfos"))
                {
                    infos = string.Empty;
                }
                if (infos.Contains("~$"))//跟车通知，车下称，通知消失
                {
                    infos = infos.Replace("~$", "");
                    isClearInfos = true;
                }
                SaveNotice(infos);
                ShowUserNotic();
            }
            catch (Exception ex)
            {
                msg = "显示内容或者记录信息时出错：" + ex.Message;
            }
            #region 写日志
            LogModel log = new LogModel()
            {
                Origin = "汽车衡_" + ClientInfo.Name,
                FunctionName = "称点主窗体_显示坐席发送过来的通知",
                Level = LogConstParam.LogLevel_Info,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                Msg = msg,
                Data = cp,
                IsDataValid = LogConstParam.DataValid_Ok,
                OperateUserName = IcCardNo
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }
        /// <summary>
        /// 将内容显示到界面上
        /// </summary>
        /// <param name="infos"></param>
        private void ShowUserNotic()
        {
            if (IsUserNotice)
            {
                string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
                string url = basePath + @"/ClientConfig/UserNotice.txt";
                if (!FileHelper.IsExistFile(url))
                {
                    FileHelper.CreateFile(url);
                }
                string infos = FileHelper.FileToString(url);
                Msg3 = infos;
            }
            else
            {

            }
        }
        /// <summary>
        /// 保存通知信息
        /// </summary>
        private void SaveNotice(string infos)
        {
            //ClientConfig/UserNotice.txt
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string url = basePath + @"/ClientConfig/UserNotice.txt";
            if (!FileHelper.IsExistFile(url))
            {
                FileHelper.CreateFile(url);
            }
            FileHelper.WriteText(url, infos);
        }
        /// <summary>
        /// 设置终端计量状态 2016-2-18 19:29:36……
        /// </summary>
        /// <param name="?"></param>
        private void SetClientState(CommandParam cp)
        {
            string msg = "接收到服务器命令【sendCMD】,业务类型【ClientState】,坐席正在处理任务!";
            try
            {
                string infos = cp.msg.msg.ToString();
                switch (infos)
                {
                    case "正在计量":
                        BullState = eBullTag.metering;
                        //TopColor = "#FF0C215F";
                        //TopColor1 = "#FF09236E";
                        //TopColor2 = "#FF072C93";
                        //LeftColor1 = "#FF060628";
                        //LeftColor2 = "#FF9AD4EF";
                        //RightColor1 = "#FF060628";
                        //RightColor2 = "#FF9AD4EF";

                        TopColor = "#FF0C215F";
                        TopColor1 = "#FF09236E";
                        TopColor2 = "#FF072C93";
                        LeftColor1 = "#FF060628";
                        LeftColor2 = "#FF060628";
                        RightColor1 = "#FF060628";
                        RightColor2 = "#FF060628";
                        break;
                }
            }
            catch (Exception ex)
            {
                msg = "设置终端状态时出错：" + ex.Message;
            }
            #region 写日志
            LogModel log = new LogModel()
            {
                Origin = "汽车衡_" + ClientInfo.Name,
                FunctionName = "称点主窗体_设置终端计量状态",
                Level = LogConstParam.LogLevel_Info,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                Msg = msg,
                Data = cp,
                IsDataValid = LogConstParam.DataValid_Ok,
                OperateUserName = IcCardNo
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion

        }
        /// <summary>
        /// 关闭IC卡
        /// </summary>
        public void CloseIc()
        {
            try
            {
                //if (iic != null)
                //{
                iic.OnRemoveCard -= iic_OnRemoveCard;
                iic.OnReadCardNo -= iic_OnReadCardNo;
                //}
                measureServiceResult = string.Empty;
                isMoveCardStopTask = false;//移卡之后司机直接开车下称 将移卡变为false 2016-3-23 09:44:34……             
                //if (iic != null)
                //{
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "注销IC卡事件，准备关闭IC卡。",
                    FunctionName = "称点主窗体_关闭IC卡",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                iic.Close();
                #region 写日志
                LogModel cLog = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "IC关闭完成。",
                    FunctionName = "称点主窗体_关闭IC卡",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(cLog));
                #endregion
                //}
                //iic = null;
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel eLog = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "IC卡关闭异常:" + ex.Message,
                    FunctionName = "称点主窗体_关闭IC卡",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(eLog));
                #endregion
            }
        }
        /// <summary>
        /// 暂停IC卡
        /// </summary>
        private void StopIc()
        {
            try
            {
                measureServiceResult = string.Empty;
                isMoveCardStopTask = false;//移卡之后司机直接开车下称 将移卡变为false 2016-3-23 09:44:34……   
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "准备停止IC卡。",
                    FunctionName = "称点主窗体_停止IC卡",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                iic.Stop();//读取卡后关闭IC卡
                #region 写日志
                LogModel cLog = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "IC卡停止完成。",
                    FunctionName = "称点主窗体_停止IC卡",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(cLog));
                #endregion
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel eLog = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "IC卡停止异常:" + ex.Message,
                    FunctionName = "称点主窗体_停止IC卡",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(eLog));
                #endregion
            }
        }
        /// <summary>
        /// 关闭RFID 设备
        /// </summary>
        private void CloseRFID()
        {
            if (rfid != null)
            {
                try
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = "准备关闭RFID。",
                        FunctionName = "称点主窗体_关闭RFID",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Info
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    rfid.Stop();
                    rfid.Close();
                    //rfid = null;
                    #region 写日志
                    log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = "关闭RFID完成。",
                        FunctionName = "称点主窗体_关闭RFID",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Info
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
                        Msg = "关闭RFID异常:" + ex.Message,
                        FunctionName = "称点主窗体_关闭RFID",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Error
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
            //if (rfid1 != null)
            //{
            //    try
            //    {
            //        #region 写日志
            //        LogModel log = new LogModel()
            //        {
            //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //            Msg = "准备关闭RFID1。",
            //            FunctionName = "称点主窗体_关闭RFID1",
            //            Origin = "汽车衡_" + ClientInfo.Name,
            //            Level = LogConstParam.LogLevel_Info
            //        };
            //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            //        #endregion
            //        rfid1.Stop();
            //        rfid1.Close();
            //        rfid1 = null;
            //        #region 写日志
            //        log = new LogModel()
            //        {
            //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //            Msg = "关闭RFID1完成。",
            //            FunctionName = "称点主窗体_关闭RFID1",
            //            Origin = "汽车衡_" + ClientInfo.Name,
            //            Level = LogConstParam.LogLevel_Info
            //        };
            //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            //        #endregion
            //    }
            //    catch (Exception ex)
            //    {
            //        #region 写日志
            //        LogModel log = new LogModel()
            //        {
            //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //            Msg = "关闭RFID1异常:" + ex.Message,
            //            FunctionName = "称点主窗体_关闭RFID1",
            //            Origin = "汽车衡_" + ClientInfo.Name,
            //            Level = LogConstParam.LogLevel_Error
            //        };
            //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            //        #endregion
            //    }
            //}
        }
        /// <summary>
        /// 暂停RFID 设备
        /// </summary>
        //private void StopRFID()
        //{
        //    if (rfid != null)
        //    {
        //        try
        //        {
        //            #region 写日志
        //            LogModel log = new LogModel()
        //            {
        //                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //                Msg = "准备停止RFID0。",
        //                FunctionName = "称点主窗体_停止RFID0",
        //                Origin = "汽车衡_" + ClientInfo.Name,
        //                Level = LogConstParam.LogLevel_Info
        //            };
        //            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
        //            #endregion
        //            rfid.Stop();
        //            //rfid = null;
        //            #region 写日志
        //            log = new LogModel()
        //            {
        //                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //                Msg = "停止RFID0完成。",
        //                FunctionName = "称点主窗体_停止RFID0",
        //                Origin = "汽车衡_" + ClientInfo.Name,
        //                Level = LogConstParam.LogLevel_Info
        //            };
        //            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
        //            #endregion
        //        }
        //        catch (Exception ex)
        //        {
        //            #region 写日志
        //            LogModel log = new LogModel()
        //            {
        //                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //                Msg = "停止RFID0时异常:" + ex.Message,
        //                FunctionName = "称点主窗体_停止RFID0",
        //                Origin = "汽车衡_" + ClientInfo.Name,
        //                Level = LogConstParam.LogLevel_Error
        //            };
        //            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
        //            #endregion
        //        }
        //    }

        //}
        /// <summary>
        /// 测试使用 将发送的任务全部保存到单独文件
        /// </summary>
        /// <param name="str"></param>
        private void SaveSendTask(string str)
        {
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string url = basePath + @"/Log/SysLog/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (!FileHelper.IsExistFile(url))
            {
                FileHelper.CreateFile(url);
            }
            FileHelper.AppendText(url, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "~" + str + "~" + "\r\n");
        }
        /// <summary>
        /// 往任务服务器发送任务清除命令
        /// </summary>
        public void SendBackTaskInfo()
        {
            //任务回退之前，将计时器停止
            resendTaskTimer.Stop();
            isLastGetListTask = true;
            System.Threading.Thread.Sleep(2000);

            MsgId = "b_" + CommonMethod.CommonMethod.GetRandom();
            Console.WriteLine("发送任务回退" + "_msgid:" + MsgId + "_" + DateTime.Now.ToString());
            //内部约定，回退任务时，参数从称点ID改为msgid，且magid是以"b_"开头,用以在redis监听的endreply中区分任务发送后的msgid和回退任务msgid
            SocketCls.Emit(ClientSendCmdEnum.backtask, MsgId);
            isBackTaskReply = false;
            backTaskTimer.Start();
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "称点主窗体_向任务服务器发送任务清除命令",
                Level = LogConstParam.LogLevel_Info,
                Msg = "调用命令【" + ClientSendCmdEnum.backtask + "】向任务服务器发送结束任务。",
                Origin = "汽车衡_" + ClientInfo.Name,
                Data = ClientInfo.ClientId,
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion

        }

        /// <summary>
        /// 将打印状态回发给坐席 2016-3-21 09:05:47……
        /// </summary>
        /// <param name="pc"></param>
        private void SendPrintState(ErrorType eType, string eStr)
        {
            //错误信息
            if (eType.Equals(ErrorType.Error))
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "打印机出现错误：" + eStr,
                    FunctionName = "称点主窗体_将打印状态回发给坐席",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            printError = eStr;
            //printError = eStr;
            //if (Weight == 0)
            //{
            //    Weight = 20;
            //}
            SendRealDataToServer("P");
            //if (Weight == 0)
            //{
            //    Weight = 40;
            //}
            //SendRealDataToServer("P");
            //if (Weight == 0)
            //{
            //    Weight = 20;
            //}
            //SendRealDataToServer("P");
            //SetShowInfoMsg("系统提示", eStr, false);
            #region 写日志
            LogModel cLog = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "秤点主窗体_将打印状态回发给坐席",
                Level = LogConstParam.LogLevel_Info,
                Msg = ClientInfo.Name + "秤点往坐席发送打印机状态",
                Data = eStr,
                IsDataValid = LogConstParam.DataValid_Ok,
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } }
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(cLog));
            #endregion
        }


        /// <summary>
        /// 处理移卡时，重量比较
        /// </summary>
        private void DoMoveCardWeight()
        {
            if (isMoveCardStopTask)
            {
                if (Weight < moveCardMinWeight)
                {
                    moveCardMinWeight = Weight;//获取移卡期间最小重量
                }
                if (Weight > moveCardMaxWeight)
                {
                    moveCardMaxWeight = Weight;//获取移卡期间最大重量
                }
            }
        }
        /// <summary>
        /// 判断是不是允许发任务
        /// 车上称之后最大重量：" + maxWeight + "与当前重量" + Weight + "差值是否小于 磅差
        /// </summary>
        /// <returns></returns>
        private bool CheckIsAllowSendTask()
        {
            bool rtB = true;
            #region 老的判断
            //if (isMoveCardStopTask)//出现移卡进行判断，未出现移卡直接返回空
            //{
            //    decimal maxMinWeight = moveCardMaxWeight - moveCardMinWeight;//移卡之后出现的最大重量与最小重量的差值
            //    //如果差值大于设定差值 则认为换卡状态重发任务 出现问题
            //    if (maxMinWeight > moreOrLessWeight)
            //    {
            //        rtB = false;
            //        decimal wD = Weight - moveCardWeight;
            //        //string showInfo="移卡时重量为：" + moveCardWeight + "移卡之后最大重量为：" + moveCardMaxWeight + "最小重量为：" + moveCardMinWeight+"，请拨打大厅电话";
            //        string showInfo = "移卡时重量为：" + moveCardWeight + "与当前重量差值为：" + wD + "，请拨打大厅电话";
            //        SetShowInfoMsg("异常提示", showInfo, false);
            //        #region 写日志
            //        LogModel log = new LogModel()
            //        {
            //            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //            Direction = LogConstParam.Directions_Out,
            //            FunctionName = "秤点主窗体",
            //            Level = LogConstParam.LogLevel_Info,
            //            Msg = IcCardNo + "：移卡之后不允许重新发送任务",
            //            Data = showInfo,
            //            IsDataValid = LogConstParam.DataValid_Ok,
            //            ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } }
            //        };
            //        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            //        #endregion

            //    }
            //}
            #endregion
            if ((maxWeight - Weight) > moreOrLessWeight)
            {
                //rtB = false;
                string showInfo = "车上称之后最大重量：" + maxWeight + "与当前重量" + Weight + "差值过大，请拨打大厅电话";
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "卡号：" + IcCardNo + showInfo,
                    FunctionName = "称点主窗体_判断是否允许发任务",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Info
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //SetShowInfoMsg("异常提示", showInfo, false);
            }
            return rtB;
        }
        /// <summary>
        /// 每次自动下载服务器配置文件
        /// </summary>
        private void DownLoadConfigFile()
        {
            try
            {
                //读取配置文件
                string clientCode = GetClientCode();
                if (string.IsNullOrEmpty(clientCode))
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Msg = "根据本机IP:" + ComputerInfoHelpClass.GetIPAddress() + "获取的秤体编码为空,退出下载配置文件业务处理。",
                        FunctionName = "称点主窗体_从服务器下载配置文件",
                        Origin = "汽车衡_" + ClientInfo.Name,
                        Level = LogConstParam.LogLevel_Warning
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    return;
                }
                if (FileHelper.IsExistFile(configUrl))
                {
                    FileHelper.DeleteFile(configUrl);
                }
                string serviceUrl = ConfigurationManager.AppSettings["getEquParamInfo"].ToString();
                var param = new
                {
                    versionnum = -1,
                    equcode = clientCode,
                    equname = ""
                };
                string getUrl = string.Format(serviceUrl, "[" + JsonConvert.SerializeObject(param) + "]");
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                String strResult = ComHelpClass.ResponseSynStr(request);
                List<EquModel> equModels = InfoExchange.DeConvert(typeof(List<EquModel>), strResult) as List<EquModel>;
                if (equModels.Count > 0)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(configUrl))
                    {
                        sw.WriteLine(equModels.First().paraminfos.Replace("GBK", "UTF-8"));
                        sw.Dispose();
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "服务器下载配置信息出错：" + ex.Message + "跟踪：" + ex.StackTrace,
                    FunctionName = "称点主窗体_从服务器下载配置文件",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //MessageBox.Show("请检查网络连接是否正常,尝试重新打开程序");
                ProcessHelpClass.ClearProcess("Talent.CarMeasureClient");
                //MessageBox.Show("服务器下载配置信息出错：" + ex.Message + "跟踪：" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 根据IP获取秤体编码
        /// </summary>
        /// <returns></returns>
        private string GetClientCode()
        {
            string clientCode = string.Empty;
            string cIp = ComputerInfoHelpClass.GetIPAddress();
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["getconfigbyip"].ToString();
                string getUrl = string.Format(serviceUrl, cIp);
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                String strResult = ComHelpClass.ResponseSynStr(request);
                if (!string.IsNullOrEmpty(strResult))
                {
                    var equipM = InfoExchange.DeConvert(typeof(Equipment), strResult) as Equipment;
                    clientCode = equipM.Equcode;
                }
            }
            catch (Exception ex)
            {
                clientCode = "911";
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "根据IP获取秤体编码信息失败：" + ex.Message,
                    FunctionName = "称点主窗体_根据IP获取秤体编码",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return clientCode;
        }
        /// <summary>
        /// 处理界面显示的当前的重量信息
        /// </summary>
        /// <param name="mType"></param>
        private void DoGrossTareWeightValue(string mType)
        {
            //计量完成或者终止计量之后，不再处理实时数据
            if (LeftColor2.Equals("#FF5FEE4A") || LeftColor2.Equals("FFFF3838"))
            {
                switch (mType)
                {
                    case "G":
                        mServiceModel.rows[0].gross = receiveBullInfo.gross;
                        break;
                    case "T":
                        mServiceModel.rows[0].tare = receiveBullInfo.tare;
                        break;
                }
            }
            else
            {
                decimal cWeight = Weight;
                switch (mType)
                {
                    case "G":
                        mServiceModel.rows[0].gross = cWeight;
                        break;
                    case "T":
                        mServiceModel.rows[0].tare = cWeight;
                        break;
                }
            }
            DoShowWeightInfo();
        }

        /// <summary>
        /// 同步数据库服务器时间
        /// </summary>
        private void SynOracleTime()
        {
            DateTime dbTime = GetDbDate();
            ComputerInfoHelpClass.SynchroTime(dbTime);
        }
        /// <summary>
        /// 获取数据库时间
        /// </summary>
        /// <returns></returns>
        private DateTime GetDbDate()
        {
            DateTime rtDtime = DateTime.Now;
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["getOracleDateTime"].ToString();
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(serviceUrl, 10);
                String strResult = ComHelpClass.ResponseSynStr(request);
                rtDtime = Convert.ToDateTime(strResult);
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "获取数据库时间异常:" + ex.Message,
                    FunctionName = "称点主窗体_获取数据库时间",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return rtDtime;
        }

        /// <summary>
        /// 清理
        /// </summary>
        /// <param name="fileInfos"></param>
        private void ClearPross(string fileInfos)
        {
            ProcessHelpClass.ClearProcessContainsName(fileInfos);
        }
        /// <summary>
        /// 清除进程
        /// </summary>
        private void ClearOldProcess()
        {
            Process process = Process.GetCurrentProcess();//获取当前程序进程
            ProcessHelpClass.ClearProcess(process);

        }

        /// <summary>
        /// 处理显示的重量信息 例如后台存储千克 前台显示吨的问题
        /// </summary>
        private void DoShowWeightInfo()
        {
            try
            {
                if (CarTaskModel.BullInfo != null)
                {
                    if (CarTaskModel.BullInfo.gross != null)
                    {
                        ShowGross = (Convert.ToDecimal(CarTaskModel.BullInfo.gross) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowGross = string.Empty;
                    }
                    if (CarTaskModel.BullInfo.tare != null)
                    {
                        ShowTare = (Convert.ToDecimal(CarTaskModel.BullInfo.tare) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowTare = string.Empty;
                    }
                    if (CarTaskModel.BullInfo.deduction != null)
                    {
                        ShowDedution = (Convert.ToDecimal(CarTaskModel.BullInfo.deduction) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowDedution = string.Empty;
                    }
                    if (CarTaskModel.BullInfo.suttle != null)
                    {
                        ShowSuttle = (Convert.ToDecimal(CarTaskModel.BullInfo.suttle) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowSuttle = string.Empty;
                    }
                    if (CarTaskModel.BullInfo.grossb != null)
                    {
                        ShowGrossb = (Convert.ToDecimal(CarTaskModel.BullInfo.grossb) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowGrossb = string.Empty;
                    }
                    if (CarTaskModel.BullInfo.tareb != null)
                    {
                        ShowTareb = (Convert.ToDecimal(CarTaskModel.BullInfo.tareb) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowTareb = string.Empty;
                    }
                    if (CarTaskModel.BullInfo.suttleb != null)
                    {
                        ShowSuttleb = (Convert.ToDecimal(CarTaskModel.BullInfo.suttleb) * weightKg).ToString("F2");
                    }
                    else
                    {
                        ShowSuttleb = string.Empty;
                    }
                }
                else
                {
                    ShowGross = string.Empty;
                    ShowTare = string.Empty;
                    ShowDedution = string.Empty;
                    ShowSuttle = string.Empty;
                    ShowGrossb = string.Empty;
                    ShowTareb = string.Empty;
                    ShowSuttleb = string.Empty;
                }
            }
            catch
            {
                ShowGross = string.Empty;
                ShowTare = string.Empty;
                ShowDedution = string.Empty;
                ShowSuttle = string.Empty;
                ShowGrossb = string.Empty;
                ShowTareb = string.Empty;
                ShowSuttleb = string.Empty;
            }
        }

        /// <summary>
        /// 将重量写入视频 2016-3-31 13:41:06……
        /// </summary>
        private void SaveWeightVideo()
        {
            try
            {
                //VideoConfig writeViedo = ConfigReader.ReadVideoConfig();
                //writeViedo.CameraList = (from r in writeViedo.CameraList where r.Position == "1" select r).ToList();
                //writeVideoController = new VideoController(writeViedo, null);
                //writeVideoController.Open();
                //writeVideoController.Start();
            }
            catch //(Exception ex)
            {

            }
        }
        /// <summary>
        /// 真正的记录数据
        /// </summary>
        private void SaveWeightToVideo()
        {
            try
            {
                if (lastSaveHKWeight == Weight)
                {
                    return;
                }
                #region 写日志
                //LogModel log = new LogModel()
                //{
                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    Msg = "向视频中写入的重量为:" + Weight,
                //    FunctionName = "称点主窗体_向视频中写入重量数据",
                //    Origin = "汽车衡_" + ClientInfo.Name,
                //    Level = LogConstParam.LogLevel_Info
                //};
                //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                //if (writeVideoController == null)
                //{
                //    return;
                //}
                //if (lastSaveHKWeight != Weight)
                //{
                //    writeVideoController.WriteTxtToVideo(Weight.ToString());
                //}
                lastSaveHKWeight = Weight;
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "向视频中写入重量时异常:" + ex.Message,
                    FunctionName = "称点主窗体_向视频中写入重量数据",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 清空用户通知信息
        /// </summary>
        private void CleatNoticeInfos()
        {
            if (isClearInfos)
            {
                isClearInfos = false;
                SaveNotice("");
                ShowUserNotic();
            }
        }
        /// <summary>
        /// 判断是不是允许保存重量
        /// </summary>
        private void CheckIsSave()
        {
            isEnterOK = false;
            isEnterOnly = false;
            //SetShowInfoMsg("系统异常", "请远程求助或者拨打大厅电话", false);
        }
        /// <summary>
        /// 判断是不是允许保存
        /// </summary>
        /// <returns></returns>
        private bool CheckIsAllowSave(string bInfoStr)
        {
            bool rtB = false;
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["beforesaveMeasureInfo"].ToString();
                HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, bInfoStr, "");
                string strResult = ComHelpClass.ResponseSynStr(request);
                var getServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "秤点主窗体_判断是否允许保存计量数据",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "调用服务判断是否保存计量数据,URL:" + serviceUrl,
                    Data = "POST数据：" + bInfoStr + "返回信息：" + strResult + ""
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                if (getServiceModel != null && getServiceModel.success)
                {
                    if (getServiceModel.mfunc != 0)
                    {
                        rtB = false;
                    }
                    else
                    {
                        rtB = true;
                    }
                }
                else
                {
                    rtB = false;
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "秤点主窗体_判断是否允许保存计量数据",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "调用服务判断是否保存计量数据时异常:" + ex.Message
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return rtB;
        }
        /// <summary>
        /// 获取秤体最大量程
        /// </summary>
        private void GetMaxAllowWeight()
        {
            string serviceUrl = ConfigurationManager.AppSettings["queryrange"].ToString();
            string url = string.Format(serviceUrl, ClientInfo.ClientCode);
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(url, 10);
            request.BeginGetResponse(new AsyncCallback(GetMaxAllowWeightCallback), request);
        }
        /// <summary>
        /// 量程服务返回值
        /// </summary>
        /// <param name="asyc"></param>
        private void GetMaxAllowWeightCallback(IAsyncResult asyc)
        {
            try
            {
                string strResultSResult = ComHelpClass.ResponseStr(asyc);
                JObject jobject = JsonHelpClass.JsonStringToJObject(strResultSResult);
                string rangStr = jobject["more"].ToString();//服务给 吨……
                if (!string.IsNullOrEmpty(rangStr))
                {
                    maxAllowWeight = Convert.ToDecimal(rangStr) * 1000;
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "获取最大重量时异常:" + ex.Message,
                    FunctionName = "称点主窗体_获取最大重量",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 判断重量信息是否显示
        /// </summary>
        private void CheckIsShowWeightControl()
        {
            try
            {
                if (Weight < maxAllowWeight)
                {
                    IsShowWeightInfo = Visibility.Visible;
                    return;
                }
                if (IsShowWeightInfo == Visibility.Hidden)
                {
                    return;
                }
                if (Weight > maxAllowWeight)
                {
                    SetShowInfoMsg("系统提示", " 当前重量已经超过最大量程：" + maxAllowWeight + "请拨打大厅电话", false);
                    IsShowWeightInfo = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Msg = "判断重量信息是否显示时异常:" + ex.Message,
                    FunctionName = "称点主窗体_判断重量信息是否显示",
                    Origin = "汽车衡_" + ClientInfo.Name,
                    Level = LogConstParam.LogLevel_Error
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 加载用户权限信息 类似于用户名密码
        /// </summary>
        private void LoadLoginUser()
        {
            LoginUser.Code = ClientInfo.ClientId;
            LoginUser.LoginName = ClientInfo.ClientCode;
            LoginUser.Name = ClientInfo.Name;
            LoginUser.Password = ClientInfo.ClientCode;
            LoginUser.Modules = new List<Module>();
        }
        /// <summary>
        /// 判断是不是记录重量数据 判断是否允许保存重量，同一时刻同一重量不记录
        /// </summary>
        /// <param name="wr"></param>
        /// <returns></returns>
        private bool CheckIsSaveWeightList(WeightRecordData wr)
        {
            bool rtB = true;
            int count = recorddatalist.Count;
            if (count > 0)
            {
                if (recorddatalist[count - 1].recordtime.Equals(wr.recordtime))
                {
                    if (recorddatalist[count - 1].recorddata.Equals(wr.recorddata))
                    {
                        rtB = false;
                    }
                }
            }
            return rtB;
        }
        /// <summary>
        /// 获取RFID集合
        /// </summary>
        /// <returns></returns>
        private string GetRfidStr()
        {
            if (rfid != null && rfid.IsAllDeviceEnable)
            {
                string rtStr = string.Empty;
                //CloseRFID();
                if (RFIDCardNo.Count > 0)
                {
                    for (int i = 0; i < RFIDCardNo.Count; i++)
                    {
                        rtStr = RFIDCardNo[i] + "," + rtStr;
                    }
                }
                //RFIDCardNo.Clear();
                //rtStr = "E2005128941100451150A072,E2005128941100451150A071,E2005128941100451150A073";
                return rtStr;
            }
            else
            {
                return "-1";
            }
        }

        #region 重启程序时关闭硬件
        /// <summary>
        /// 是否坐席发送命令重启程序
        /// </summary>
        public bool IsSeatRestart = false;
        public void ClosedHardWare()
        {
            //表头
            try
            {
                if (iwc != null)
                {
                    iwc.Stop();
                    iwc.Close();
                    iwc = null;
                }

            }
            catch { }
            //IC卡
            try
            {
                CloseIc();
            }
            catch { }
            //RFID卡
            try
            {
                CloseRFID();
            }
            catch { }

        }

        /// <summary>
        /// 重启秤点程序
        /// </summary>
        /// <returns></returns>
        private void RestartClient(CommandParam pCp)
        {
            LogModel log;
            #region 写日志
            log = new LogModel()
            {
                Origin = "汽车衡_" + ClientInfo.Name,
                FunctionName = "称点主窗体",
                Level = LogConstParam.LogLevel_Info,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                Msg = "接收到服务器命令~sendCMD~,业务类型~ClientRestart~,终端重启!",
                Data = pCp,
                IsDataValid = LogConstParam.DataValid_Ok,
                OperateUserName = IcCardNo
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            IsSeatRestart = true;
            //SendBackTaskInfo();
            //ClosedHardWare();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Messenger.Default.Send<object>(null, "ClearAll");
                Thread.Sleep(1000);
                System.Windows.Forms.Application.Restart();
                Thread.Sleep(500);
                Application.Current.Shutdown(0);
            });
            // return log;
        }
        #endregion
        #endregion
    }

}
