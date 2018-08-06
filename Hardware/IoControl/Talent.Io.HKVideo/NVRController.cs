using log4net;
/*
 * 2015-09-11  强浩
 * NVR,DVRX相关输入输出控制类
 * 控制红绿灯，红外，大灯
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Talent.Io;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.ClinetLog;
using Talent.CommonMethod;


namespace Talent.Io.HKVideo
{



    public class NVRController : IIoController
    {
        /// <summary>
        /// 是否初始化成功
        /// </summary>
        bool _isInitSuccess = false;
        ILog log;
        int _alarmHandel;

        string _userName, _password;

        /// <summary>
        /// 红外报警计时器
        /// </summary>
        System.Threading.Timer _alarmSignalTimer;
        /// <summary>
        /// 弹出错误消息
        /// </summary>
        public event Action<string> OnShowErrMsg;
        /// <summary>
        /// 报警事件
        /// </summary>
        public event ReceiveAlarmSignal OnReceiveAlarmSignal;
        CHCNetSDK.NET_DVR_DEVICEINFO _deviceInfo;
        private CHCNetSDK.MSGCallBack _falarmData = null;
        IOconfig _curIOconfig;//当前IO配置
        int _userId;
        Byte[] _alarmIn = new Byte[16];
        /// <summary>
        /// 输入端口
        /// </summary>
        public Byte[] AlarmIn
        {
            get { return _alarmIn; }
            set { _alarmIn = value; }
        }
        Byte[] _alarmOut = new Byte[16];
        /// <summary>
        /// 输出端口
        /// </summary>
        public Byte[] AlarmOut
        {
            get { return _alarmOut; }
            set { _alarmOut = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        ///<param name="pConfigFile">配置文件路径</param>
        //public NVRController(string pConfigFile)
        //{
        //    ConfigReader cfgReader = new ConfigReader(pConfigFile);
        //    IOconfig cfg = ConfigReader.ReadIoConfig();
        //    InnerInit(cfg.EquLoginName, cfg.EquLoginPwd, cfg);
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pIOconfig">IO配置</param>
        public NVRController(IOconfig pIOconfig)
        {
            InnerInit(pIOconfig.EquLoginName, pIOconfig.EquLoginPwd, pIOconfig);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pIp"></param>
        /// <param name="pPort"></param>
        /// <param name="pUserName"></param>
        /// <param name="pPassWord"></param>
        /// <param name="pIOconfig">IO配置</param>
        public NVRController(string pUserName, string pPassWord, IOconfig pIOconfig)
        {
            InnerInit(pUserName, pPassWord, pIOconfig);
        }

        private void InnerInit(string pUserName, string pPassWord, IOconfig pIOconfig)
        {
            log = LogHelper.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\Log4Net.config", "IO_LOG");
            _userName = pUserName;
            _password = pPassWord;
            _curIOconfig = pIOconfig;
            TimerCallback callBack = new TimerCallback(Alarm);
            _alarmSignalTimer = new Timer(callBack, null, Timeout.Infinite, -1);
            _isInitSuccess = Init();
        }

        /// <summary>
        /// 报警回调函数
        /// </summary>
        /// <param name="paramObject"></param>
        private void Alarm(object paramObject)
        {
            if (OnReceiveAlarmSignal != null)
            {
                foreach (Device di in _curIOconfig.InDeviceList)
                {
                   
                    if (di.ReceiveSignalTime != DateTime.MinValue)
                    {
                        DateTime dat = DateTime.Now;
                        if (dat.Subtract(di.ReceiveSignalTime).TotalMilliseconds >= 1000)
                        {
                            //log.Info(string.Format("报警,端口：{0}。", di.Port));
                            OnReceiveAlarmSignal(di.Code, "0");


                            //if (di.Port == "0")
                            //{
                            //    System.Diagnostics.Debug.WriteLine(string.Format("端口{0}，未遮挡", di.Port));
                            //}
                        }
                        else
                        {
                            OnReceiveAlarmSignal(di.Code, "1");
                            //if (di.Port == "0")
                            //{
                            //    System.Diagnostics.Debug.WriteLine(string.Format("端口{0}，被遮挡", di.Port));
                            //}
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private bool Init()
        {
            bool rtn = true;
            try
            {
                //初始化
                bool initResult = CHCNetSDK.NET_DVR_Init();
                log.Info("NET_DVR_Init()调用成功。initResult=" + initResult.ToString());
                if (initResult)
                {
                    //登录
                    _userId = CHCNetSDK.NET_DVR_Login(_curIOconfig.Ip, ushort.Parse(_curIOconfig.Port), _userName, _password, ref _deviceInfo);
                    if (_userId >= 0)
                    {
                        log.Error(string.Format("登录成功。IP:{0},端口：{1},用户名：{2},密码：{3}", _curIOconfig.Ip, _curIOconfig.Port, _userName, _password));
                        //设置报警回调函数
                        _falarmData = new CHCNetSDK.MSGCallBack(MsgCallback);
                        if (CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V30(_falarmData, IntPtr.Zero))
                        {
                            log.Info("设置回调成功。");
                            _alarmHandel = CHCNetSDK.NET_DVR_SetupAlarmChan(_userId);
                            log.Info("布防成功。");
                            _alarmSignalTimer.Change(0, 100);
                            log.Info("红外监控计时器启动成功。");
                        }
                        else
                        {
                            rtn = false;
                            log.Error("NET_DVR_SetDVRMessageCallBack_V30() 布防失败。");
                            ShowMsg("NET_DVR_SetDVRMessageCallBack_V30() 布防失败。");
                        }
                    }
                    else
                    {
                        rtn = false;
                        log.Error(string.Format("登录失败。IP:{0},端口：{1},用户名：{2},密码：{3}", _curIOconfig.Ip, _curIOconfig.Port, _userName, _password));
                        ShowMsg("NET_DVR_Login() 登录失败。");
                    }
                }
                else
                {
                    rtn = false;
                    log.Error("NET_DVR_Init() 初始化失败。");
                    ShowMsg("NET_DVR_Init() 初始化失败。");
                }
            }
            catch (Exception ex)
            {
                rtn = false;
                log.Error("未知异常。", ex);
                ShowMsg(ex.Message);
            }
            // ShowMsg("初始化成功。");
            return rtn;
        }


        /// <summary>
        /// 消息回调
        /// </summary>
        /// <param name="pMsg"></param>
        private void ShowMsg(string pMsg)
        {
            if (OnShowErrMsg != null)
            {
                OnShowErrMsg(pMsg);
            }
        }
        /// <summary>
        /// 消息回调
        /// </summary>
        /// <param name="lCommand"></param>
        /// <param name="pAlarmer"></param>
        /// <param name="pAlarmInfo"></param>
        /// <param name="dwBufLen"></param>
        /// <param name="pUser"></param>
        private void MsgCallback(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            switch (lCommand)
            {
                case CHCNetSDK.COMM_ALARM:
                case CHCNetSDK.COMM_ALARM_V30://
                    ProcessCommAlarm(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                default:
                    break;
            }
        }

        private void ProcessCommAlarm(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_DVR_ALARMINFO struAlarmInfo = new CHCNetSDK.NET_DVR_ALARMINFO();

            struAlarmInfo = (CHCNetSDK.NET_DVR_ALARMINFO)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_ALARMINFO));

            string strIP = pAlarmer.sDeviceIP;
            // string stringAlarm = "";
            int i = 0;

            switch (struAlarmInfo.dwAlarmType)
            {
                case 0:
                    log.Info(string.Format("端口{0}输出报警。", struAlarmInfo.dwAlarmInputNumber));
                    foreach (Device di in _curIOconfig.InDeviceList)
                    {
                        if (struAlarmInfo.dwAlarmInputNumber.ToString() == di.Port)
                        {
                            lock (di)
                            {
                                //System.Diagnostics.Debug.WriteLine(string.Format("报警端口：{0}。时间：{1}",di.Port, di.ReceiveSignalTime));
                                di.ReceiveSignalTime = DateTime.Now;
                            }
                            //if (di.Port == "0")
                            //{
                            //    System.Diagnostics.Debug.WriteLine(string.Format("报警时间：{0}", di.ReceiveSignalTime));
                            //}
                            break;
                        }
                    }
                    //stringAlarm = "信号量报警，报警报警输入口：" + struAlarmInfo.dwAlarmInputNumber + "，触发录像通道：";
                    //for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                    //{
                    //    if (struAlarmInfo.dwAlarmRelateChannel[i] == 1)
                    //    {
                    //        stringAlarm += (i + 1) + " \\ ";
                    //    }
                    //}
                    break;
                //case 1:
                //    stringAlarm = "硬盘满，报警硬盘号：";
                //    for (i = 0; i < CHCNetSDK.MAX_DISKNUM; i++)
                //    {
                //        if (struAlarmInfo.dwDiskNumber[i] == 1)
                //        {
                //            stringAlarm += (i + 1) + " \\ ";
                //        }
                //    }
                //    break;
                //case 2:
                //    stringAlarm = "信号丢失，报警通道：";
                //    for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                //    {
                //        if (struAlarmInfo.dwChannel[i] == 1)
                //        {
                //            stringAlarm += (i + 1) + " \\ ";
                //        }
                //    }
                //    break;
                //case 3:
                //    stringAlarm = "移动侦测，报警通道：";
                //    for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                //    {
                //        if (struAlarmInfo.dwChannel[i] == 1)
                //        {
                //            stringAlarm += (i + 1) + " \\ ";
                //        }
                //    }
                //    break;
                //case 4:
                //    stringAlarm = "硬盘未格式化，报警硬盘号：";
                //    for (i = 0; i < CHCNetSDK.MAX_DISKNUM; i++)
                //    {
                //        if (struAlarmInfo.dwDiskNumber[i] == 1)
                //        {
                //            stringAlarm += (i + 1) + " \\ ";
                //        }
                //    }
                //    break;
                //case 5:
                //    stringAlarm = "读写硬盘出错，报警硬盘号：";
                //    for (i = 0; i < CHCNetSDK.MAX_DISKNUM; i++)
                //    {
                //        if (struAlarmInfo.dwDiskNumber[i] == 1)
                //        {
                //            stringAlarm += (i + 1) + " \\ ";
                //        }
                //    }
                //    break;
                //case 6:
                //    stringAlarm = "遮挡报警，报警通道：";
                //    for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                //    {
                //        if (struAlarmInfo.dwChannel[i] == 1)
                //        {
                //            stringAlarm += (i + 1) + " \\ ";
                //        }
                //    }
                //    break;
                //case 7:
                //    stringAlarm = "制式不匹配，报警通道";
                //    for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                //    {
                //        if (struAlarmInfo.dwChannel[i] == 1)
                //        {
                //            stringAlarm += (i + 1) + " \\ ";
                //        }
                //    }
                //    break;
                //case 8:
                //    stringAlarm = "非法访问";
                //    break;
                default:
                    //stringAlarm = "其他未知报警信息";
                    break;
            }
        }

        /// <summary>
        /// 端口控制输出
        /// </summary>
        /// <param name="pIndex">输出端口对应的索引</param>
        /// <param name="pValue">输出端口对应的值</param>
        /// <returns></returns>
        private bool SetAlarmOut(int pIndex, Byte pValue)
        {
            bool rtn = false;

            try
            {
                if (pIndex < 0 || pIndex > 15) return false;
                _alarmOut[pIndex] = pValue;
                rtn = CHCNetSDK.NET_DVR_SetAlarmOut(_userId, pIndex, pValue);
            }
            catch (Exception ex)
            {
                log.Info(string.Format("用户：{0},端口：{1},值：{2}", _userId, pIndex, pValue), ex);
            }
            return rtn;

        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public void LogOut()
        {
            if (_alarmHandel >= 0)
            {
                CHCNetSDK.NET_DVR_CloseAlarmChan(_alarmHandel);
                log.Info("撤防成功。");
            }

            if (_userId >= 0)
            {
                // _alarmSignalTimer.Change(-1, -1); 
                CHCNetSDK.NET_DVR_Logout(_userId);
                log.Info("退出成功。");
            }
        }


        /// <summary>
        /// 内部执行命令方法
        /// </summary>
        /// <param name="pCommand"></param>
        /// <returns></returns>
        private bool ExecCommand(IoCommand pCommand)
        {
            bool rtn = false;
            if (!_isInitSuccess)
            {
                _isInitSuccess = Init();
            }
            if (!_isInitSuccess)
            {
                return false;
            }
            try
            {
                if (pCommand.Type == IoConfigParam.Type_EquType)//设备类型
                {
                    foreach (Device dv in _curIOconfig.OutDeviceList)
                    {
                        //2016-10-11 设备类型编码 qh
                        if (dv.EquTypeCode == pCommand.Code)
                        {
                            SetAlarmOut(int.Parse(dv.Port), byte.Parse(pCommand.Value));
                        }
                    }
                }
                else//设备
                {
                    foreach (Device dv in _curIOconfig.OutDeviceList)
                    {
                        if (dv.Code == pCommand.Code)
                        {
                            SetAlarmOut(int.Parse(dv.Port), byte.Parse(pCommand.Value));
                            break;
                        }
                    }
                }
                rtn = true;
            }
            catch (Exception ex)
            {
                rtn = false;
                log.Error("ExecCommand异常。", ex);
            }
            return rtn;
        }
        #region IIoController
        /// <summary>
        /// 执行红绿灯命令
        /// </summary>
        /// <param name="pDicCmd"></param>
        /// <returns></returns>
        public bool ExecCommand(List<IoCommand> pIoCommandList)
        {
            if (!_isInitSuccess)
            {
                _isInitSuccess = Init();
            }
            if (!_isInitSuccess)
            {
                return false;
            }

            bool rtn = false;
            foreach (IoCommand cmd in pIoCommandList)
            {
                log.Info(string.Format("执行命令.编码：{0}，类型：{1}，值：{2}", cmd.Code, cmd.Type, cmd.Value));
                rtn = ExecCommand(cmd);
                if (rtn == false)
                {
                    break;
                }
            }
            return rtn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pJsonCommand">json格式的命令</param>
        /// <returns></returns>
        public bool ExecCommand(string pJsonCommand)
        {
            bool rtn = false;
            List<IoCommand> tempCmdList = null;
            try
            {
                tempCmdList = InfoExchange.DeConvert(typeof(List<IoCommand>), pJsonCommand) as List<IoCommand>;
            }
            catch (Exception ex)
            {
                log.Info(string.Format("解析命令{0}时出错", pJsonCommand), ex);
            }
            if (tempCmdList != null && tempCmdList.Count > 0)
            {
                rtn = ExecCommand(tempCmdList);
            }
            return rtn;
        }
        /// <summary>
        /// 命令字符串转化为控制命令
        /// </summary>
        public bool ExecTestCommand(string pCommandStr)
        {
            //if (!_isInitSuccess)
            //{
            //    _isInitSuccess = Init();
            //}
            //if (!_isInitSuccess)
            //{
            //    return false;
            //}

            log.Info("发送指令：" + pCommandStr);
            char[] stats = pCommandStr.ToCharArray();
            //红灯
            foreach (Device item in _curIOconfig.OutDeviceList)
            {
                int temp = int.Parse(item.Port);
                log.Info(string.Format("绿灯端口：{0},值：{1}", item.Port, pCommandStr));
                SetAlarmOut(temp, byte.Parse(stats[temp].ToString()));
            }
            return true;
        }
        #endregion
    }
}
